using MD2PandocCL;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                try
                {
                    items = MD2PandocCL.Gitbook2PandocParser.SummaryParser(dialog.FileName);
                    summaryPath = dialog.FileName;
                    lbFiles.ItemsSource = items;
                    btnCreateSinglePandocFile.IsEnabled = true;
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

        private void btnCreateSinglePandocFile_Click(object sender, RoutedEventArgs e)
        {
            //Todo iets meer controle op verwijederen stuff
            //Todo vragen aan gebruiker waar het moet gezet worden
            //Todo "done" tonen
            var log = Gitbook2PandocParser.CreateMegaMarkdown(items, System.IO.Path.GetDirectoryName(summaryPath), "myfile.md", "", true);
        }
    }
}