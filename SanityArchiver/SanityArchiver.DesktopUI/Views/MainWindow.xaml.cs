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

namespace WPF_Explorer_Tree
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        private ObservableCollection<FileInfo_Class> Files = new ObservableCollection<FileInfo_Class>();
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
            FileInfo_Class fclass;
            DirectoryInfo dirInfo = new DirectoryInfo(filename);
            /*var obcinfo = new List<FileInfo_Class>();*/

            FileInfo[] info = dirInfo.GetFiles("*.*");
            foreach (FileInfo f in info)
            {
                fclass = new FileInfo_Class();
                fclass.FullName = f.FullName;
                fclass.Name = f.Name;
                FileSizeFormat(fclass, f);
                fclass.DirectoryName = f.DirectoryName;
                fclass.CreationTime = f.CreationTime.ToString();
                fclass.Extension = f.Extension;
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

            FileInfo_Class x = (FileInfo_Class)selectedFile.SelectedItem;


            MessageBoxResult result = MessageBox.Show($"Would you like to greet the world with a \"Hello, world\"?", "My App", MessageBoxButton.OKCancel);
            switch (result)
            {
                case MessageBoxResult.OK:
                    MessageBox.Show("Change the File name");
                    FileInfo fileInfo = new FileInfo(x.FullName);

                    FileSecurity fileSecurity = fileInfo.GetAccessControl();
                    string user = System.Environment.UserName;
                    fileSecurity.AddAccessRule(new FileSystemAccessRule(user, FileSystemRights.FullControl, AccessControlType.Allow));
                    fileInfo.SetAccessControl(fileSecurity);
                    FileInfo_Class f = new FileInfo_Class();
                    var newFileName = x.DirectoryName + "\\" + "newfilename" + x.Extension;
                    File.Move(x.FullName, newFileName);
                    var myFile = Files.FirstOrDefault(fil => fil.FullName == x.FullName);
                    myFile.FullName = newFileName;
                    myFile.Name = "newfilename" + x.Extension;
                    CollectionViewSource.GetDefaultView(Files).Refresh();
                    break;
                case MessageBoxResult.Cancel:
                    MessageBox.Show("Oh well, too bad!", "My App");
                    break;
            }
        }

        private static void FileSizeFormat(FileInfo_Class fclass, FileInfo f)
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
    }
}
