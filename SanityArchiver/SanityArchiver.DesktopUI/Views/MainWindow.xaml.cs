using System;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;

using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using System.Security.AccessControl;
using System.Linq;
using System.Windows.Data;
using SanityArchiver;
using System.Windows.Forms.VisualStyles;

namespace WPF_Explorer_Tree
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        private ObservableCollection<FileDetails> Files = new ObservableCollection<FileDetails>();
        private object dummyNode = null;

        public delegate void RefreshList();
        public event RefreshList RefreshListEvent;
        private void RefreshListView()
        {
            foldersItem.Items.Refresh();
        }

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
            /*var obcinfo = new List<FileDetails>();*/

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


        void files_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            var selectedFile = (System.Windows.Controls.ListView)sender;

            FileDetails x = (FileDetails)selectedFile.SelectedItem;

            EditFileProperties window = new EditFileProperties(x, Files);
            window.ShowDialog();
            if(window.DialogResult == true)
            {
                this.Files = window.Files;
                CollectionViewSource.GetDefaultView(Files).Refresh();
            }


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
            System.Windows.Controls.TreeView tree = (System.Windows.Controls.TreeView)sender;
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
                if (temp.Parent.GetType().Equals(typeof(System.Windows.Controls.TreeView)))
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
                var archiveDirectory = Directory.CreateDirectory(filesToArchive[0].DirectoryName + "\\CompresedFiles");
                string zipPath = archiveDirectory.FullName +  "\\Archive.zip";
                /*createZipFile(filesToArchive[0].DirectoryName, "Archive");*/


                using (ZipFile zip = new ZipFile())
                {

                    foreach (FileDetails file in filesToArchive)
                    {
                        zip.AddFile(file.Path);

                    }
                    zip.Save(filesToArchive[0].DirectoryName + "\\Archive.zip");

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

        

       /* public void createZipFile(string zipPath, string archiveFileName)
        {
            //DirectoryToBeArchive = "C:\\Temp\\myZipFile"
            string DirectoryToBeArchive = zipPath + "\\" + archiveFileName;

            //Some logical error here, you probably meant to use File.Exists()
            //Basically, as you can't find a directory with name C:\\Temp\\myZipFile.zip, you always jump into else
            if (Directory.Exists(DirectoryToBeArchive + ".zip"))
            {
                File.Delete(DirectoryToBeArchive);
                ZipFile.CreateFromDirectory(zipPath, DirectoryToBeArchive + ".zip", CompressionLevel.Fastest, false);
            }
            else
                //It will try to overwrite your existing "DirectoryToBeArchive".zip file 
                ZipFile.CreateFromDirectory(zipPath, DirectoryToBeArchive + ".zip", CompressionLevel.Fastest, false);

            //This won't work as well btw, as there probably is no directory 
            //with name C:\\Temp\\myZipFile
            Directory.Delete(DirectoryToBeArchive);
        }*/
    }
}
