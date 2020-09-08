using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using System.Collections.ObjectModel;
using System.IO.Compression;

namespace WPF_Explorer_Tree
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        private ObservableCollection<FileDetails> Files = new ObservableCollection<FileDetails>();

        private object dummyNode = null;

        public Window1()
        {
            InitializeComponent();
        }

        public string SelectedImagePath { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string s in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = s;
                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                foldersItem.Items.Add(item);
            }
        }

        public void selectFolders(string filename)
        {

            Files.Clear();
            FileDetails fclass;
            DirectoryInfo dirInfo = new DirectoryInfo(filename);
            /*var obcinfo = new List<FileInfo_Class>();*/

            FileInfo[] info = dirInfo.GetFiles("*.*");
            foreach (FileInfo f in info)
            {
                fclass = new FileDetails();
                fclass.Name = f.Name;
                FileSizeFormat(fclass, f);
                fclass.DirectoryName = f.DirectoryName;
                fclass.CreationTime = f.CreationTime.ToString();
                fclass.Extension = f.Extension;
                fclass.Path = f.FullName;
             
                Files.Add(fclass);
            }
            /*DirectoryInfo[] subDirectories = dirInfo.GetDirectories();
            foreach (DirectoryInfo directory in subDirectories)
            {
                selectfolders(directory.FullName);
            }*/

            FilesSection.ItemsSource = Files;


        }

        private static void FileSizeFormat(FileDetails fclass, FileInfo f)
        {
            if (f.Length >= (1 << 30))
                fclass.Size = string.Format("{0}Gb", f.Length >> 30);
            else
                            if (f.Length >= (1 << 20))
                fclass.Size = string.Format("{0}Mb", f.Length >> 20);
            else
                            if (f.Length >= (1 << 10))
                fclass.Size = string.Format("{0}Kb", f.Length >> 10);
        }

        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        subitem.Items.Add(dummyNode);
                        subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception) { }
            }
        }

        private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);

            if (temp == null)
            {
                return;
            }

            SelectedImagePath = "";
            string temp1 = "";
            string temp2 = "";
            while (true)
            {
                temp1 = temp.Header.ToString();
                if (temp1.Contains(@"\"))
                {
                    temp2 = "";
                }
                SelectedImagePath = temp1 + temp2 + SelectedImagePath;
                if (temp.Parent.GetType().Equals(typeof(TreeView)))
                {
                    break;
                }
                temp = ((TreeViewItem)temp.Parent);
                temp2 = @"\";
            }

            selectFolders(SelectedImagePath);
            /*MessageBox.Show(SelectedImagePath);*/
        }

        private void Archive_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var listOfFiles = Files.Where(f => f.IsSelected).Select(f => f.Name).ToList();
            MessageBoxResult confirmResult = MessageBox.Show("Are you sure to archive this file ??", String.Join(",", listOfFiles), MessageBoxButton.OKCancel);

            var filesToArchive = new List<FileDetails>();
            if (confirmResult == MessageBoxResult.OK)
            {
                filesToArchive = Files.Where(f => f.IsSelected).ToList();
                foreach (FileDetails file in filesToArchive) 
                {
                    Directory.CreateDirectory(file.DirectoryName + "\\CompresedFiles");
                    string zipPath =  file.DirectoryName +"\\CompresedFiles"  + "\\" + file.Name.Replace(file.Extension, ".zip");
                    ZipFile.CreateFromDirectory(file.DirectoryName, zipPath);
                }
                
            }
            else
            {
                // If 'No', do something here.
            }

        }

        private void FilePath_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
