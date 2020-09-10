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
using System.Collections.Generic;
using Ionic.Zip;
using System.Windows.Forms;
using System.Text;
using System.Text;
using SanityArchiver.DesktopUI.Views;

namespace WPF_Explorer_Tree
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        private ObservableCollection<FileDetails> Files = new ObservableCollection<FileDetails>();
        private object dummyNode = null;
        private long FolderSize { get; set; }
        private string FolderSizeText { get; set; }

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

            FolderSize = DirSize(dirInfo);
           
            if (FolderSize >= (1 << 30))
                FolderSizeText = string.Format("{0}Gb", FolderSize >> 30);
            else
                            if (FolderSize >= (1 << 20))
                FolderSizeText = string.Format("{0}Mb", FolderSize >> 20);
            else
                            if (FolderSize >= (1 << 10))
                FolderSizeText = string.Format("{0}Kb", FolderSize >> 10);
            totalRecording.Text = $"Folder size: {FolderSizeText}";


            try
            {
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
            }
            catch { }
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

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
		{
           
            var SelectedFile = Files.Where(f => f.IsSelected).ToList()[0];
            var text = File.ReadAllText(SelectedFile.Path, Encoding.Default);
            MessageBox.Show(text, SelectedFile.Name, MessageBoxButton.OK);
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
                


                using (ZipFile zip = new ZipFile())
                {
                    var zipFile = new FileDetails();
                    
                    
                    foreach (FileDetails file in filesToArchive)
                    {
                        zip.AddFile(file.Path);

                    }
                    zip.Save(filesToArchive[0].DirectoryName + "\\compresed.zip");
                    zipFile.Name = "Archive.zip";
                    var now = DateTime.Now;
                    zipFile.CreationTime = now.ToString();
                    Files.Add(zipFile);

                }
                
                CollectionViewSource.GetDefaultView(Files).Refresh();

            }
            else
            {
                // If 'No', do something here.
            }

        }


        private void Encrypt_MenuItem_Click(object sender, RoutedEventArgs e)

        {
            var listOfFiles = Files.Where(f => f.IsSelected).ToList();


            if (listOfFiles.Count() == 1 || listOfFiles[0].Extension == ".txt" )
            {
                try
                {
                    foreach (FileDetails file in listOfFiles)
                    {
                       
                        EncryptDecrypt.EncryptFile(file.Path, "12345678");
                        MessageBox.Show("Encryption succesfuly " + file.Name);

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                
            }
            else 
            {
                if (listOfFiles.Count() != 1)
                {
                    MessageBox.Show("Only 1 file can be encrypted");
                } else if (listOfFiles[0].Extension != ".txt")
                {
                    MessageBox.Show("Only .txt files can be encrypted");
                }
                

            }

        }

/*        private void Move_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var listOfFiles = Files.Where(f => f.IsSelected).ToList();

            listOfFiles[0].Name = "Massaaa";
        }*/



        private void Move_MenuItem_Click(object sender, RoutedEventArgs e) {


            var selectedFile = Files.FirstOrDefault(f => f.IsSelected);

            FileInfo fileInfo = new FileInfo(selectedFile.Path);

            // opens the dialog window to chose the move destination file
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;

                // getting access to edit the selected file in the system

                FileSecurity fileSecurity = fileInfo.GetAccessControl();
                string user = Environment.UserName;
                fileSecurity.AddAccessRule(new FileSystemAccessRule(user, FileSystemRights.FullControl, AccessControlType.Allow));
                fileInfo.SetAccessControl(fileSecurity);

                // composing the new file name and extension from the user inputs

                var newFilePath = path + "\\" + fileInfo.Name;

                // rename in the system the targeted file
                File.Move(selectedFile.Path, newFilePath);
                var myFile = Files.FirstOrDefault(fil => fil.Path == fileInfo.FullName);
                myFile.DirectoryName = newFilePath;
                myFile.Path = newFilePath;
                Files.Remove(myFile);
                CollectionViewSource.GetDefaultView(Files).Refresh();

            }
        }

        private void Copy_MenuItem_Click(object sender, RoutedEventArgs e)
        {

            var selectedFile = Files.FirstOrDefault(f => f.IsSelected);

            FileInfo fileInfo = new FileInfo(selectedFile.Path);

            // opens the dialog window to chose the move destination file
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;

                // getting access to edit the selected file in the system

                FileSecurity fileSecurity = fileInfo.GetAccessControl();
                string user = Environment.UserName;
                fileSecurity.AddAccessRule(new FileSystemAccessRule(user, FileSystemRights.FullControl, AccessControlType.Allow));
                fileInfo.SetAccessControl(fileSecurity);

                // composing the new file name and extension from the user inputs

                var newFilePath = path + "\\" + fileInfo.Name;

                // rename in the system the targeted file
                File.Copy(selectedFile.Path, newFilePath);
                var myFile = Files.FirstOrDefault(fil => fil.Path == fileInfo.FullName);
                myFile.Path = newFilePath;
                CollectionViewSource.GetDefaultView(Files).Refresh();

            }
        }


        private void Decrypt_MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var listOfFiles = Files.Where(f => f.IsSelected).ToList();


            if (listOfFiles.Count() == 1 || listOfFiles[0].Extension == ".txt")
            {
                try
                {
                    foreach (FileDetails file in listOfFiles)
                    {  
                        EncryptDecrypt.DecryptFile(file.Path, "12345678");
                        MessageBox.Show("Decryption succesfuly " + file.Name);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                
            }
            else
            {
                if (listOfFiles.Count() != 1)
                {
                    MessageBox.Show("Only 1 file can be encrypted");
                }
                else if (listOfFiles[0].Extension != ".ENC")
                {
                    MessageBox.Show("Only .ENC files can be encrypted");
                }


            }
        }

        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            try
            {
                FileInfo[] fis = d.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    size += fi.Length;
                }
                // Add subdirectory sizes.
                DirectoryInfo[] dis = d.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                    size += DirSize(di);
                }
                
            }
            catch { }
            return size;
        }

        private void Delete_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedFile = Files.FirstOrDefault(f => f.IsSelected);
            Files.Remove(selectedFile);
            CollectionViewSource.GetDefaultView(Files).Refresh();
        }
    }

}



   

