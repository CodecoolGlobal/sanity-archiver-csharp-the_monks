using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Data;
using WPF_Explorer_Tree;

namespace SanityArchiver
{
    /// <summary>
    /// Interaction logic for CommentWindow.xaml
    /// </summary>
    public partial class EditFileProperties : Window
    {

        public FileInfo_Class FileInfos { get; set; }
        public ObservableCollection<FileInfo_Class> Files { get; set; }

        public EditFileProperties(FileInfo_Class fileInfo, ObservableCollection<FileInfo_Class> filesCollection)
        {
            InitializeComponent();
            FileInfos = fileInfo;
        }

        private void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(FileInfos.FullName);

                FileSecurity fileSecurity = fileInfo.GetAccessControl();
                string user = Environment.UserName;
                fileSecurity.AddAccessRule(new FileSystemAccessRule(user, FileSystemRights.FullControl, AccessControlType.Allow));
                fileInfo.SetAccessControl(fileSecurity);
                FileInfo_Class f = new FileInfo_Class();
                var newFileName = FileInfos.DirectoryName + "\\" + FileName.Text + "." + Extension.Text;
                File.Move(FileInfos.FullName, newFileName);
                var myFile = Files.FirstOrDefault(fil => fil.FullName == fileInfo.FullName);
                myFile.FullName = newFileName;
                myFile.Name = "newfilename" + "." + Extension.Text;
                File.Move(f.Name, Path.ChangeExtension(f.FullName, ".jpg"));
                CollectionViewSource.GetDefaultView(Files).Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Close();
            }

        }

        private void CloseEdit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
