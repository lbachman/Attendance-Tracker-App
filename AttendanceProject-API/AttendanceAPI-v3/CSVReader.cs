using System.Text.RegularExpressions;

public static class CSVFormExtractor
{
    public static (string, List<string>) ExtractDataFromCSV(IFormFile csvFile, string classId)
    {
        List<string> usernames = new List<string>();

        using (var reader = new StreamReader(csvFile.OpenReadStream()))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("#"))
                {
                    // Extract username from each line
                    string username = ExtractUsername(line);
                    if (!string.IsNullOrEmpty(username))
                    {
                        usernames.Add(username);
                    }
                }
            }
        }

        return (classId, usernames);
    }

    private static string ExtractUsername(string line)
    {
        // Use regex to extract the username from the line
        string pattern = @"#([^\s,]+)";
        Match match = Regex.Match(line, pattern);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        else
        {
            return null;
        }
    }
}
