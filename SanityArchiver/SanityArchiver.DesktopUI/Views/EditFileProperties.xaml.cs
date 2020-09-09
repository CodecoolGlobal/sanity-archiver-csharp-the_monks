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

        public FileDetails FileInfos { get; set; }
        public ObservableCollection<FileDetails> Files { get; set; }

        public EditFileProperties(FileDetails fileInfo, ObservableCollection<FileDetails> filesCollection)
        {
            InitializeComponent();
            FileInfos = fileInfo;
        }

        private void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(FileInfos.Path);

                FileSecurity fileSecurity = fileInfo.GetAccessControl();
                string user = Environment.UserName;
                fileSecurity.AddAccessRule(new FileSystemAccessRule(user, FileSystemRights.FullControl, AccessControlType.Allow));
                fileInfo.SetAccessControl(fileSecurity);
                FileDetails f = new FileDetails();
                var newFileName = FileInfos.DirectoryName + "\\" + FileName.Text + "." + Extension.Text;
                File.Move(FileInfos.Path, newFileName);
                var myFile = Files.FirstOrDefault(fil => fil.Path == fileInfo.FullName);
                myFile.Path = newFileName;
                myFile.Name = "newfilename" + "." + Extension.Text;
                File.Move(f.Name, Path.ChangeExtension(f.Path, ".jpg"));
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
