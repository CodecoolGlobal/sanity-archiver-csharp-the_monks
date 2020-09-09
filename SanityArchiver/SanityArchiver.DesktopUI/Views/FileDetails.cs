using System.Runtime.InteropServices;

namespace WPF_Explorer_Tree
{
    public class FileDetails
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public string DirectoryName { get; set; }

        public string Extension { get; set; }

        public string CreationTime { get; set; }

        public string Path { get; set; }


        public bool IsSelected { get; set; } = false;


        public FileDetails()
        {


        }
    }
}