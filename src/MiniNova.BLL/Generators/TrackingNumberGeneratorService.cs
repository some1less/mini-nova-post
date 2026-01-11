using System.Text;

namespace MiniNova.BLL.Generators;

public class TrackingNumberGeneratorService : ITrackingNumberGeneratorService
{
    private static readonly int[] Weights = { 3, 7, 1, 5, 9, 3, 7, 1, 5, 9, 3};
    private static readonly Dictionary<string, string> EuroCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Ukraine", "UA" }, { "Poland", "PL" }, { "Germany", "DE" }, 
        { "France", "FR" }, { "Italy", "IT" }, { "Spain", "ES" }, 
        { "United Kingdom", "GB" }, { "Great Britain", "GB" }, { "England", "GB" },
        { "Netherlands", "NL" }, { "Belgium", "BE" }, { "Sweden", "SE" }, 
        { "Switzerland", "CH" }, { "Austria", "AT" }, { "Greece", "GR" }, 
        { "Czech Republic", "CZ" }, { "Czechia", "CZ" }, { "Portugal", "PT" }, 
        { "Hungary", "HU" }, { "Denmark", "DK" }, { "Finland", "FI" }, 
        { "Norway", "NO" }, { "Ireland", "IE" }, { "Romania", "RO" }, 
        { "Slovakia", "SK" }, { "Croatia", "HR" }, { "Bulgaria", "BG" }, 
        { "Lithuania", "LT" }, { "Slovenia", "SI" }, { "Latvia", "LV" }, 
        { "Estonia", "EE" }, { "Luxembourg", "LU" }, { "Cyprus", "CY" }, 
        { "Malta", "MT" }, { "Iceland", "IS" }, { "Moldova", "MD" }, 
        { "Bosnia and Herzegovina", "BA" }, { "Serbia", "RS" }, 
        { "Montenegro", "ME" }, { "North Macedonia", "MK" }, { "Albania", "AL" }
    };


    public string GenerateTrackingNumber(string countryName, int sizeId, decimal weight)
    {
        var stringBuilder = new StringBuilder();
        if (string.IsNullOrEmpty(countryName) || !EuroCodes.TryGetValue(countryName, out var code))
        {
            code = "EU";
        }
        stringBuilder.Append(code);
        
        int weightPart = (int)(weight*10);
        string specs = $"{sizeId}{weightPart:D3}";
        stringBuilder.Append(specs);
        
        var random = new Random();
        string part3 = random.Next(0,10000).ToString("D4");
        string part4 = random.Next(0,1000).ToString("D3");
        stringBuilder.Append(part3).Append(part4);
        
        string numericString = specs + part3 + part4;
        int checkDigit = CalculateControlNumber(numericString);
        
        stringBuilder.Append(checkDigit);
        
        return stringBuilder.ToString();
    }

    private int CalculateControlNumber(string numericString)
    {
        if (numericString.Length != Weights.Length)
            throw new ArgumentException("The numeric string must contain exactly " + Weights.Length + " elements.");
        
        int sum = 0;

        for (int i = 0; i < numericString.Length; i++)
        {
            int digit = numericString[i] - '0';
            sum += digit * Weights[i];
        }
        
        int remainder = sum % 10;
        int checkDigit = (10 - remainder) % 10;
        
        return checkDigit;
    }

    public bool ValidateTrackingNumber(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber)) return false;
        
        var cleanedNumber = trackingNumber.Replace(" ", "").Replace("-", "").ToUpper();

        if (cleanedNumber.Length != 14) return false;
        if(!char.IsLetter(cleanedNumber[0]) || !char.IsLetter(cleanedNumber[1])) return  false;

        for (int i = 2; i < cleanedNumber.Length; i++)
        {
            if (!char.IsDigit(cleanedNumber[i])) return false;
        }

        int providedControlNumber = cleanedNumber[^1] - '0'; // [cleanedNumber.Length - 1]
        string numericPart = cleanedNumber.Substring(2, 11);
        
        int checkControlNumber = CalculateControlNumber(numericPart);
        return checkControlNumber == providedControlNumber;
    }
}