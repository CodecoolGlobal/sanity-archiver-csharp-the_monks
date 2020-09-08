using System.Runtime.InteropServices;

namespace WPF_Explorer_Tree
{
    internal class FileInfo_Class
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Size { get; set; }
        public string DirectoryName { get; set; }

        public string Extension { get; set; }

        public string CreationTime { get; set; }


        public FileInfo_Class()
        {

                
        }
    }
}