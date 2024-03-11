
using System.Diagnostics;
using System.Text;
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

        public static List<string> CreateMegaMarkdown(List<string> sourceFiles, string basesourceUrl, string targetFile, bool overWrite = false)
        {
            List<string> log = new List<string>();
            log.Add("\nMegaMarkdown begonne");
            if (File.Exists(targetFile) && !overWrite)
            {
                log.Add($"Bestand {targetFile} bestaat maar mag niet overschrijven.Sorry");
                return log;
            }
            if (File.Exists(targetFile) && overWrite) //overwrite check expres dubbel gezet, kwestie van niets te overschrijven dat we niet willen
            {
                log.Add($"Bestand {targetFile} bestaat maar mag overschrijven.Byebye file");
                File.Delete(targetFile); 
            }
            StringBuilder allText = new StringBuilder();
            foreach (var file in sourceFiles)
            {
                var fullFilePath = Path.Combine(basesourceUrl, file);
                if (File.Exists(fullFilePath))
                {
                    allText.Append(File.ReadAllText(fullFilePath));

                }
                else
                {
                    allText.Append($"***********!!!!!!!!!!!!!FILE NOT FOUND:{fullFilePath}");
                    log.Add($"***********!!!!!!!!!!!!!FILE NOT FOUND:{fullFilePath}");
                }

                allText.Append(Environment.NewLine);
            }


            File.WriteAllText(targetFile, allText.ToString());
            log.Add("MegaMarkdown geeindigd. Hoera");
            return log;
        }


    }
}
