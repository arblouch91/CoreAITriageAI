using System.Net.Http.Json;
using System.Text.Json;

namespace CoreTriageAI.Services;

public class ComplaintAnalysis
{
    public string Department { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public decimal SentimentsScore { get; set; }
    public string SentimentsLabel { get; set; } = string.Empty;
}

public class OpenRouterService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public OpenRouterService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["OpenRouterApiKey"]!;
    }

    public async Task<ComplaintAnalysis> AnalyzeAsync(string name, string email, string complaint)
    {
        var userContent = $"Analyze this customer complaint and return a JSON response.\n\n" +
                          $"Name: {name}\nEmail: {email}\nComplaint: {complaint}\n" +
                          $"Available Departments: Fraud / Billing Team, Digital Banking Support, Relationship Management, Card Operations\n\n" +
                          $"Return ONLY this JSON format:\n" +
                          "{{\n" +
                          $"  \"name\": \"{name}\",\n" +
                          $"  \"email\": \"{email}\",\n" +
                          $"  \"complaint\": \"{complaint}\",\n" +
                          "  \"department\": \"<pick the best department from the available list>\",\n" +
                          "  \"priority\": \"<low | medium | high>\",\n" +
                          "  \"sentiments_score\": <number between 0.00 and 1.00>,\n" +
                          "  \"sentiments_label\": \"<Extremely Negative | Negative | Neutral | Positive>\"\n" +
                          "}}";

        var payload = new
        {
            model = "openrouter/auto",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "You are a complaint management assistant for a bank. Analyze customer complaints and return ONLY a valid JSON object with no extra text, no markdown, no explanation."
                },
                new { role = "user", content = userContent }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        request.Content = JsonContent.Create(payload);

        using var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(body);
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()!
            .Trim();

        // Strip markdown fences if the model wraps the JSON
        if (content.StartsWith("```"))
        {
            var firstNewline = content.IndexOf('\n');
            var lastFence = content.LastIndexOf("```");
            content = content[(firstNewline + 1)..lastFence].Trim();
        }

        using var analysisDoc = JsonDocument.Parse(content);
        var root = analysisDoc.RootElement;

        return new ComplaintAnalysis
        {
            Department = root.GetProperty("department").GetString()!,
            Priority = root.GetProperty("priority").GetString()!,
            SentimentsScore = root.GetProperty("sentiments_score").GetDecimal(),
            SentimentsLabel = root.GetProperty("sentiments_label").GetString()!
        };
    }
}
