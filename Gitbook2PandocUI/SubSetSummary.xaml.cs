using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gitbook2PandocUI
{
    /// <summary>
    /// Interaction logic for SubSetSummary.xaml
    /// </summary>
    public partial class SubSetSummary : Window
    {
        public SubSetSummary()
        {
            InitializeComponent();
        }

        public bool WantsSubset { get; internal set; }
        public string DocumentSummaryPath { get; internal set; }
        public string DocumentSummary { get; internal set; }

        private void lbSubset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lbSubset.SelectedIndex>=0)
            {
                DocumentSummary = subtext[lbSubset.SelectedIndex];
                WantsSubset = true;
                this.Close();
            }

        }


        List<string> subtext= new List<string>();
        private void lbSubset_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(DocumentSummaryPath))
            {
                string fileContent = File.ReadAllText(DocumentSummaryPath);
                string pattern = @"##\s*(?<header>[^#\r\n]+)[\r\n]+(?<content>(?:\*\s*\[[^\r\n]+\][^\r\n]*[\r\n]+)+)";

                MatchCollection matches = Regex.Matches(fileContent, pattern);
                subtext.Clear();
                foreach (Match match in matches)
                {
                    lbSubset.Items.Add(match.Groups["header"].Value);
                    subtext.Add(match.Groups["content"].Value);
                }
            }
        }
    }
}
