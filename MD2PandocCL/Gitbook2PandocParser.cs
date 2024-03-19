
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            List<string> log = ["\nMegaMarkdown begonne"];
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
                    allText.Append(Environment.NewLine);
                    allText.Append(@"\newpage");
                    allText.Append(Environment.NewLine);
                }
                else
                {
                    allText.Append($"***********!!!!!!!!!!!!!FILE NOT FOUND:{fullFilePath}");
                    log.Add($"***********!!!!!!!!!!!!!FILE NOT FOUND:{fullFilePath}");
                }

                allText.Append(Environment.NewLine);
            }

            var cleanedUP = CleanUpMarkdown(allText.ToString());

            cleanedUP = ConvertBlurbs(cleanedUP);

            //assets
            var targetassetFolder = Path.Combine(targetFolder, "assets");
            if (Directory.Exists(targetassetFolder))
                Directory.Delete(targetassetFolder, true);
             CopyDirectory(Path.Combine(basesourceUrl, "assets"),targetassetFolder , true);
            cleanedUP = cleanedUP.Replace("../assets/", "");

            File.WriteAllText(fullTargetFilePath, cleanedUP);
            //template eisvogel
            Directory.CreateDirectory(System.IO.Path.Combine(targetFolder, "templates"));
            string fileNameTemplate = "templates\\eisvogel.latex";
            var fullPathTemplate = System.IO.Path.Combine(targetFolder, fileNameTemplate);
            File.WriteAllBytes(fullPathTemplate, Resource1.eisvogel);

            //TODO: yamlfile

            //batchfile
            AddBatchscript(targetFolder, targetFile, System.IO.Path.GetFileName(fileNameTemplate));

            log.Add("MegaMarkdown geeindigd. Hoera");
            return log;
        }
        private static void AddBatchscript(string targetFolder, string mdfilepath, string fileNameTemplate)
        {
            string fullscript= $"pandoc metadata.yaml {mdfilepath} -o book.pdf " +
                $"--resource-path=assets " +
                $"--template templates\\{fileNameTemplate} " +
                $"--number-sections " +
                $"--from markdown " +
              //  $"--listings " +
                $"--variable toc-own-page=true " +
                $"--variable book=true " +
                $"--top-level-division=chapter " +
                $"--filter pandoc-latex-environment " +
                $"--self-contained";

            var scriptPath = System.IO.Path.Combine(targetFolder, "makebook.bat");
            File.WriteAllText(scriptPath, fullscript);

            var scriptPathVerbose = System.IO.Path.Combine(targetFolder, "makebook-verbose.bat");
            File.WriteAllText(scriptPath, fullscript+ " --verbose");
        }
        private static string ConvertBlurbs(string text)
        {
            text = text.Replace("{% hint style='danger' %}", "::: important");
            text = text.Replace("{% hint style='warning' %}", "::: warning");
            text = text.Replace("{% hint style='tip' %}", "::: tip");
            text = text.Replace("{% endhint %}", ":::" + Environment.NewLine);
            return text;
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
