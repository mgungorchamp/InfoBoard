using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoBoard.Models
{
    public class FileInformation
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Name{ get; set; }
        public string Path { get; set; }
        public string S3Key { get; set; }
        public int Size { get; set; }
        public DateTime UploadedAt { get; set; }
        public string PresignedURL { get; set; }
    }
}
