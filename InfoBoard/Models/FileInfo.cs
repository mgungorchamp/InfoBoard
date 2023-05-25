namespace InfoBoard.Models
{
    public record class FileInformation
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string s3key { get; set; }
        public int size { get; set; }
        public string uploaded_at { get; set; }
        public string presignedURL { get; set; }


    }
}
