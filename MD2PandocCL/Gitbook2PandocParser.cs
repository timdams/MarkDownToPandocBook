
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

        public static List<string> CreateMegaMarkdown(List<string> sourceFiles, string basesourceUrl, string targetFile, string targetFolder, bool overWrite = false)
        {
            List<string> log = new List<string>();
            log.Add("\nMegaMarkdown begonne");
            var fullTargetFilePath = Path.Combine(targetFolder, targetFile);
            if (File.Exists(fullTargetFilePath) && !overWrite)
            {
                log.Add($"Bestand {fullTargetFilePath} bestaat maar mag niet overschrijven.Sorry");
                return log;
            }
            if (File.Exists(fullTargetFilePath) && overWrite) //overwrite check expres dubbel gezet, kwestie van niets te overschrijven dat we niet willen
            {
                log.Add($"Bestand {fullTargetFilePath} bestaat maar mag overschrijven.Byebye file");
                File.Delete(fullTargetFilePath);
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

            var cleanedUP = CleanUpMarkdown(allText.ToString());



            //assets
            var targetassetFolder = Path.Combine(targetFolder, "assets");
            if (Directory.Exists(targetassetFolder))
                Directory.Delete(targetassetFolder, true);
             CopyDirectory(Path.Combine(basesourceUrl, "assets"),targetassetFolder , true);
            cleanedUP = cleanedUP.Replace("../assets/", "");

            File.WriteAllText(fullTargetFilePath, cleanedUP);
            log.Add("MegaMarkdown geeindigd. Hoera");
            return log;
        }

        private static string? CleanUpMarkdown(string v)
        {

            return v;
        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}
