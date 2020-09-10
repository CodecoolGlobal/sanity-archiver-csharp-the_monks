using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanityArchiver.Application.Models
{
    public static class MoveModel
    {
        public static string File { get; set; }
        public static string SourceFileName { get; set; }
        public static string DestinationDirectory { get; set; }
        public static int CopyClickCounter { get; set; } = 0;
        public static int MoveClickCounter { get; set; } = 0;

        public static void SetDestinationDirectory(string directory)
        {
            DestinationDirectory = $"{directory}/{File}";
        }


    }
}
