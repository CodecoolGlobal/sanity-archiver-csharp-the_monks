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
    /// Interaction logic for EditFileProperties.xaml
    /// </summary>
    public partial class EditFileProperties : Window
    {

        public FileDetails FileInfos { get; set; }
        public ObservableCollection<FileDetails> Files { get; set; }

        public EditFileProperties(FileDetails fileInfo, ObservableCollection<FileDetails> filesCollection)
        {
            InitializeComponent();
            FileInfos = fileInfo;
            Files = filesCollection;
        }

        private void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            // obtained a FileInfo object with infos about the targeted file

            FileInfo fileInfo = new FileInfo(FileInfos.Path);

            // getting access to edit the selected file in the system

            FileSecurity fileSecurity = fileInfo.GetAccessControl();
            string user = Environment.UserName;
            fileSecurity.AddAccessRule(new FileSystemAccessRule(user, FileSystemRights.FullControl, AccessControlType.Allow));
            fileInfo.SetAccessControl(fileSecurity);

            // composing the new file name and extension from the user inputs


            if (FileName.Text != "" && Extension.Text == "") {

                var oldExtension = fileInfo.Name.Substring(fileInfo.Name.Length - 4);
                var newFileName = FileInfos.DirectoryName + "\\" + FileName.Text + oldExtension;

                // rename in the system the targeted file
                File.Move(FileInfos.Path, newFileName);

                // rename the target file in the Files ObservableCollection
                var myFile = Files.FirstOrDefault(fil => fil.Path == fileInfo.FullName);
                myFile.Path = newFileName;
                myFile.Name = FileName.Text + oldExtension;
            }


            else if (FileName.Text == "" && Extension.Text != "")
            {
                var oldFileNameNoExtension = fileInfo.Name.Substring(0, fileInfo.Name.Length - 3);
                var newFileName = FileInfos.DirectoryName + "\\" + oldFileNameNoExtension + Extension.Text;

                // rename in the system the targeted file
                File.Move(FileInfos.Path, newFileName);

                // rename the target file in the Files ObservableCollection
                var myFile = Files.FirstOrDefault(fil => fil.Path == fileInfo.FullName);
                myFile.Path = newFileName;
                myFile.Name = oldFileNameNoExtension + Extension.Text;

            }

            else if (FileName.Text != "" && Extension.Text != "") {

                var newFileName = FileInfos.DirectoryName + "\\" + FileName.Text + "." + Extension.Text;

                // rename in the system the targeted file
                File.Move(FileInfos.Path, newFileName);

                // rename the target file in the Files ObservableCollection
                var myFile = Files.FirstOrDefault(fil => fil.Path == fileInfo.FullName);
                myFile.Path = newFileName;
                myFile.Name = FileName.Text + "." + Extension.Text;

            }

            Close();

        }

        private void CloseEdit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
