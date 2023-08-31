namespace InfoBoard.Models
{
    public class MediaCategory
    {
        public MediaCategory()
        {
            media = new List<Media>();
        }
        public int id { get; set; }
        public string name { get; set; }
        public int user_id { get; set; }
        public string color { get; set; }
        public int random_order { get; set; }
        public List<Media> media { get; set; }
    }
}
