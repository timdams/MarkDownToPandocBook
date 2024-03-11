
using System.Text.RegularExpressions;

namespace MD2PandocCL
{
    public class Gitbook2PandocParser
    {
        public static List<string> SummaryParser(string filePath)
        {
            if (File.Exists(filePath))
            {
                string fileContent = File.ReadAllText(filePath);

                //we gaan ervan uit dat iedere file die we nodig hebben tussen haakjes staat in de summary.md file
                var links = Regex.Matches(fileContent, @"\[.*?\]\((.*?)\)");
                List<string> resultFiles = new List<string>();
                foreach (Match link in links)
                {
                    resultFiles.Add(link.Groups[1].Value.ToString());
                }
                return resultFiles;
            }
            else throw new FileNotFoundException($"File not found: {filePath}");
            
        }


    }
}
