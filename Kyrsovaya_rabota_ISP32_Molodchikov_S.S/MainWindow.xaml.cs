using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Text.Json;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;


namespace Kyrsovaya_rabota_ISP32_Molodchikov_S.S_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        string jsonString = @"[{""BatchId"":0,""AccessionChanges"":[{""LabId"":8675309,""InstanceChanges"":[{""Property"":""Note"",""ChangedTo"":""Jabberwocky"",""UniqueId"":null,""SummaryInstance"":null},{""Property"":""Instrument"",""ChangedTo"":""instrumented"",""UniqueId"":null,""SummaryInstance"":null}],""DetailChanges"":[{""Property"":""Comments"",""ChangedTo"":""2nd Comment"",""UniqueId"":null,""SummaryInstance"":null},{""Property"":""CCC"",""ChangedTo"":""XR71"",""UniqueId"":null,""SummaryInstance"":null}]}]}]";
        string SavePath = String.Empty;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void CreateNewFile(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            folderBrowserDialog.ShowDialog();
            SavePath = folderBrowserDialog.SelectedPath;
            

        }
        private void FileLoad(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"c:\";
            ofd.Filter = "JSON file (*.json)|*.json";
            if (ofd.ShowDialog() == true)
            {
                TextRange doc = new TextRange(jBox.Document.ContentStart, jBox.Document.ContentEnd);
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                {
                    doc.Load(fs, DataFormats.Text);
                }
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {

                    JObject o = (JObject)JToken.ReadFrom(new JsonTextReader(sr));
                    string js = o.ToString();
                    jTree.Items.Add(JSONOperation.Json2Tree(JArray.Parse(jsonString), "Root"));
                }
            }
            
        }
        private void TreeViewItem_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.Focus();
                e.Handled = true;
            }
        }
    }
    class JSONOperation
    {
        public static TreeViewItem Json2Tree(JToken root, string rootName = "")
        {
            var parent = new TreeViewItem() { Header = rootName };

            foreach (JToken obj in root)
                foreach (KeyValuePair<string, JToken> token in (JObject)obj)
                    switch (token.Value.Type)
                    {
                        case JTokenType.Array:
                            var jArray = token.Value as JArray;

                            if (jArray?.Any() ?? false)
                                parent.Items.Add(Json2Tree(token.Value as JArray, token.Key));
                            else
                                parent.Items.Add($"\x22{token.Key}\x22 : [ ]"); // Empty array   
                            break;

                        case JTokenType.Object:
                            parent.Items.Add(Json2Tree((JObject)token.Value, token.Key));
                            break;

                        default:
                            parent.Items.Add(GetChild(token));
                            break;
                    }

            return parent;
        }

        private static TreeViewItem GetChild(KeyValuePair<string, JToken> token)
        {
            var value = token.Value.ToString();
            var outputValue = string.IsNullOrEmpty(value) ? "null" : value;
            return new TreeViewItem() { Header = $" \x22{token.Key}\x22 : \x22{outputValue}\x22" };
        }
    }
}
