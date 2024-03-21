using MD2PandocCL;
using Microsoft.Win32;

using System.Windows;


namespace Gitbook2PandocUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        List<string> items = new List<string>();
        string summaryPath = "";
        private void selSummaryFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "summary"; // Default file name
            dialog.DefaultExt = ".md"; // Default file extension
            dialog.Filter = "Markdown documents (.md)|*.md"; // Filter files by extension

            if(Properties.Settings.Default.lastSourceFolder!="empty")
            {
                dialog.InitialDirectory= Properties.Settings.Default.lastSourceFolder;
            }

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                Properties.Settings.Default.lastSourceFolder = System.IO.Path.GetDirectoryName(dialog.FileName);
                Properties.Settings.Default.Save();
                // Open document
                try
                {
                    items = MD2PandocCL.Gitbook2PandocParser.SummaryParser(dialog.FileName);
                    summaryPath = dialog.FileName;
                    lbFiles.ItemsSource = items;
                    btnCreateSinglePandocFile.IsEnabled = true;

                    //kijken of er yaml aanwezig is en vragen of die gebruikt moet worden
                    var yamlPath = System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(summaryPath),
                        "metadata.yaml"
                        );
                    if (System.IO.File.Exists(yamlPath))
                    {
                        if (MessageBox.Show("Yaml metadata bestand gevonden. Deze gebruiken? (bij nee zal ik er een lege voor je maken)", "Metadata", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            metadatadata = System.IO.File.ReadAllText(yamlPath);
                        }

                    }
                }
                catch (Exception ex)
                {

                    btnCreateSinglePandocFile.IsEnabled = false;
                    MessageBox.Show(ex.Message);
                }

            }
            else

                btnCreateSinglePandocFile.IsEnabled = false;
        }
        string metadatadata = "";
        private void btnCreateSinglePandocFile_Click(object sender, RoutedEventArgs e)
        {

            var folder = new OpenFolderDialog
            {
                Title = "Kies locatie"
            };

            if(Properties.Settings.Default.lastTargetFolder!="empty")
            {
                folder.InitialDirectory = Properties.Settings.Default.lastTargetFolder;
            }

            if (folder.ShowDialog() == true)
            {
                Properties.Settings.Default.lastTargetFolder = folder.FolderName;
                Properties.Settings.Default.Save();
                string texfile = "myfile.md";
                string fullFile = System.IO.Path.Combine(folder.FolderName, texfile);
                if (System.IO.File.Exists(fullFile))
                {
                    if (MessageBox.Show("Hier staat reeds een myfile.md bestand. Dit wordt overschreven. Ben je zeker?", "Opgelet", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation) == MessageBoxResult.No)
                    { return; }
                }
             
                var log = Gitbook2PandocParser.CreateMegaMarkdown(
                    items,
                    System.IO.Path.GetDirectoryName(summaryPath),
                    texfile,
                    folder.FolderName,
                    metadatadata,
                    true);
                MessageBox.Show("Donzo!", "Hoera", MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }
    }
}