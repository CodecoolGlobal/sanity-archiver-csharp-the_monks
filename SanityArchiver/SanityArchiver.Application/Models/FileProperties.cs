using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SanityArchiver.Application.Models
{
    public class FileProperties
    {
        public string FileName { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Size { get; set; }
        public string FullName { get; set; }
        public bool isHidden { get; set; }
        public string Extension { get; set; }
        public string CheckboxName { get; set; }

        public void GetFileName(string path)
        {
            FileName = Path.GetFileNameWithoutExtension(path);
            Extension = Path.GetExtension(path);
            FullName = CheckboxName;
        }
    }
}
