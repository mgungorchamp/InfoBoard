namespace InfoBoard.Models
{
    public class MiniMedia
    {
        public MiniMedia()
        {
            path = "uploadimage.png";
            display_width = -1;
            timing = 10;
        }
        public MiniMedia(Media media)
        {
            name = media.name;
            path = media.path;
            display_width = media.display_width;
            timing = media.timing;
        }
        public string name { get; set; }
        public int timing { get; set; }
        public int display_width { get; set; }
        public string path { get; set; }
    }
}
