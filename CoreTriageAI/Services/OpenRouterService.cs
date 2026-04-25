using System.Net.Http.Json;
using System.Text.Json;

namespace CoreTriageAI.Services;

public class ComplaintAnalysis
{
    public string Department { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal SentimentsScore { get; set; }
    public string SentimentsLabel { get; set; } = string.Empty;
    public string AIDraftedResponse { get; set; } = string.Empty;
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
        var userContent =
            $"Analyze the following bank customer complaint and return a JSON response.\n\n" +
            $"Customer Name: {name}\n" +
            $"Customer Email: {email}\n" +
            $"Complaint: {complaint}\n\n" +
            "Available Categories (pick exactly one): Fraud, Fees, Service Downtime, Card Issue\n" +
            "Available Departments (pick exactly one): Fraud / Billing Team, Digital Banking Support, Relationship Management, Card Operations\n\n" +
            "Return ONLY this JSON — no markdown, no extra keys, no explanation:\n" +
            "{\n" +
            "  \"category\": \"<one of the available categories>\",\n" +
            "  \"department\": \"<one of the available departments>\",\n" +
            "  \"priority\": \"<low | medium | high>\",\n" +
            "  \"sentiments_score\": <decimal between 0.00 and 1.00 based on the complaint tone>,\n" +
            "  \"sentiments_label\": \"<derive from sentiments_score: 0.00-0.25 = Extremely Negative, 0.26-0.50 = Negative, 0.51-0.75 = Neutral, 0.76-1.00 = Positive>\",\n" +
            $"  \"ai_drafted_response\": \"<a professional, empathetic, personalized reply addressed to {name} that acknowledges the complaint, apologises sincerely, explains the next steps the bank will take, and provides a realistic resolution timeline — 3 to 5 sentences, formal tone>\"\n" +
            "}";

        var payload = new
        {
            model = "openrouter/auto",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "You are a senior complaint management specialist at a bank. " +
                              "Analyse customer complaints and return ONLY a valid JSON object with no extra text, no markdown, and no explanation. " +
                              "The ai_drafted_response must be professional, empathetic, and personalized to the customer by name."
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
            Category = root.GetProperty("category").GetString()!,
            Department = root.GetProperty("department").GetString()!,
            Priority = root.GetProperty("priority").GetString()!,
            SentimentsScore = root.GetProperty("sentiments_score").GetDecimal(),
            SentimentsLabel = root.GetProperty("sentiments_label").GetString()!,
            AIDraftedResponse = root.GetProperty("ai_drafted_response").GetString()!
        };
    }
}
