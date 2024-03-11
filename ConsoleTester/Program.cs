

namespace ConsoleTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            //https://github.com/timdams/GitbookMD_to_LeanPubMarkua/blob/main/Program.cs
            string basedir = "C:\\Users\\damst\\Dropbox\\PROGPROJECTS\\cursus\\ziescherpscherper_gitbook\\";
            string testfile = @"summary.md";

           var res=  MD2PandocCL.Gitbook2PandocParser.SummaryParser(Path.Combine(basedir,testfile));

        }


    }
}
