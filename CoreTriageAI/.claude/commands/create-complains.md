---
description: Create multiple records in the Complains table in the CoreTriageAI database
argument-hint: "<count>"
allowed-tools: [Bash]
---

# create-complains

Create multiple complaint records in the `Complains` table of the `CoreTriageAI` database.

## Usage

```
/create-complains <count>
```

## Parameters

- `count` (integer): Number of complaints to create (e.g., 2, 5, 10, 20, 50)
- Maximum: 50 complaints

## Examples

```
/create-complains 5
/create-complains 10
/create-complains 20
```

---

## Step 1 — Parse Arguments

Extract and validate the `count` parameter from user input.

- If no count provided, default to 1
- Validate that count is a positive integer ≤ 50
- If invalid, display: `Please provide a positive integer between 1 and 50`
- Exit if validation fails

---

## Step 2 — Generate Records

For each record (1 to count), generate the following fields:

1. **Name**: Pick a random Islamic name from this pool (mix male/female):
   `Ahmed, Fatima, Omar, Aisha, Hassan, Zainab, Ali, Maryam, Yusuf, Khadija, Ibrahim, Noor, Khalid, Safiya, Tariq, Hafsa, Bilal, Sumaya, Idris, Ruqayyah, Usman, Amina, Salman, Rabia, Hamza, Asma, Faisal, Layla, Zaid, Hana, Mustafa, Sara, Dawood, Mariam, Imran, Yasmin, Adnan, Tasneem, Suleiman, Farida, Waleed, Najma, Rashid, Samira, Jabir, Nadia, Naeem, Hodan, Aamir, Zara`

2. **Email**: `{lowercase_name}@gmail.com`

3. **PhoneNumber**: `090078601`

4. **ComplainText**: Generate a realistic bank complaint (50–100 characters). Examples:
   - `My debit card was declined at the ATM despite having sufficient funds.`
   - `I was charged a fee twice for the same transaction this month.`
   - `Mobile banking app crashes every time I try to transfer money.`
   - `My account was blocked without any prior notice or explanation.`
   - `I have been waiting three weeks for my replacement credit card.`

5. **Department**: Random from:
   `Billing Team`, `Digital Banking Support`, `Relationship Management`, `Card Operations`

6. **Priority**: Random from: `High`, `Medium`, `Low`

7. **SentimentScore**: Random decimal between `0.00` and `1.00`

8. **SentimentLabel**: Derived from SentimentScore:
   - `0.00 – 0.25` → `Extremely Negative`
   - `0.26 – 0.50` → `Negative`
   - `0.51 – 0.75` → `Neutral`
   - `0.76 – 1.00` → `Positive`

9. **Category**: Random from: `Fraud`, `Fees`, `Service Downtime`, `Card Issue`

10. **Status**: Random from: `Open`, `In Progress`, `Resolved`

11. **AIDraftedResponse**: A short personalized response (50–100 characters) referencing the customer's name. Example:
    `Dear Ahmed, we apologise for the inconvenience and are resolving your issue promptly.`

---

## Step 3 — Insert Into Database

For each generated record, run a `sqlcmd` INSERT using this exact format:

```bash
sqlcmd -S localhost\SQLEXPRESS01 -d CoreTriageAI -E -Q "
INSERT INTO Complains ([Name], [Email], [PhoneNumber], [ComplainText], [Department], [Priority], [SentimentScore], [SentimentLabel], [AIDraftedResponse], [Category], [Status], [CreatedAt])
VALUES (N'{Name}', N'{Email}', N'{PhoneNumber}', N'{ComplainText}', N'{Department}', N'{Priority}', {SentimentScore}, N'{SentimentLabel}', N'{AIDraftedResponse}', N'{Category}', N'{Status}', GETDATE())
"
```

- Replace all `{placeholders}` with the generated values
- Escape any single quotes in text fields by doubling them (`''`)
- Run each INSERT as a separate `sqlcmd` call
- Track success and failure counts

---

## Step 4 — Confirm

After all inserts, display a summary:

```
Complaints created: {success_count}
Failed:             {fail_count}
Database:           CoreTriageAI
Table:              Complains
```
