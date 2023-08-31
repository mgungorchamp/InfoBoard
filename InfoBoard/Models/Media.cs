namespace InfoBoard.Models
{
    public class Media : IEquatable<Media>
    {
        public Media()
        {
            path = "uploadimage.png";
            s3key = "uploadimage.png";
            type = "file";
            display_width = -1;
            timing = 10;
            media_categories = new List<string>();
        }
        public int id { get; set; }
        public int user_id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int timing { get; set; }
        public string path { get; set; }
        public string created_at { get; set; }
        public string s3key { get; set; }
        public int size { get; set; }
        public string mime_type { get; set; }
        public int display_width { get; set; }
        public List<string> media_categories { get; set; }


        public bool Equals(Media other)
        {
            if (other is null)
                return false;

            return this.s3key == other.s3key && this.created_at == other.created_at;
        }
        public override bool Equals(object obj) => Equals(obj as Media);
        public override int GetHashCode() => (s3key, created_at).GetHashCode();
    }
}
