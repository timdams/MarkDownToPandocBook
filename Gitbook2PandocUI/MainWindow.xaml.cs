using MD2PandocCL;
using Microsoft.Win32;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;


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
        ObservableCollection<string> items = new ObservableCollection<string>();
        string summaryPath = "";
        private void selSummaryFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "summary"; // Default file name
            dialog.DefaultExt = ".md"; // Default file extension
            dialog.Filter = "Markdown documents (.md)|*.md"; // Filter files by extension
            items.Clear();
            if (Properties.Settings.Default.lastSourceFolder != "empty")
            {
                dialog.InitialDirectory = Properties.Settings.Default.lastSourceFolder;
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
                    List<string> res = null;
                    SubSetSummary subSetSummary = new SubSetSummary();
                    subSetSummary.DocumentSummaryPath = dialog.FileName;
                    subSetSummary.ShowDialog();
                    if (subSetSummary.WantsSubset)
                    {
                        res = MD2PandocCL.Gitbook2PandocParser.SummaryParseFromText(subSetSummary.DocumentSummary);
                    }
                    else
                    {
                        res = MD2PandocCL.Gitbook2PandocParser.SummaryParser(dialog.FileName);
                    }
                    
                    foreach (var file in res)
                    {
                        items.Add(file);
                    }
                    summaryPath = dialog.FileName;

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

            if (Properties.Settings.Default.lastTargetFolder != "empty")
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
                string batch;
                var log = Gitbook2PandocParser.CreateMegaMarkdown(
                  items.ToList(),
                    System.IO.Path.GetDirectoryName(summaryPath),
                    texfile,
                    folder.FolderName,
                    metadatadata,
                    out batch,
                    true);
                if (MessageBox.Show("Donzo! Wil je dat ik ineens de pdf via pandoc genereer (werkt nog niet)?", "Hoera", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                //    Process.Start(batch);
                    // Process.Start("book.pdf");
                }
            }


        }

        private void onlykeepthisBtn(object sender, RoutedEventArgs e)
        {
            var s = (sender as Button).DataContext as string;  //TODO databinding...geen zin in
            items.Clear();
            items.Add(s);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbFiles.ItemsSource = items;
        }
    }
}