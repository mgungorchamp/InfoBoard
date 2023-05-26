namespace InfoBoard.Models
{
    public class FileInformation : IEquatable<FileInformation>
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string s3key { get; set; }
        public int size { get; set; }
        public string uploaded_at { get; set; }
        public string presignedURL { get; set; }

        public bool Equals(FileInformation other)
        {
            if (other is null)
                return false;

            return this.s3key == other.s3key && this.uploaded_at == other.uploaded_at;
        }
        public override bool Equals(object obj) => Equals(obj as FileInformation);
        public override int GetHashCode() => (s3key, uploaded_at).GetHashCode();
    }
}
