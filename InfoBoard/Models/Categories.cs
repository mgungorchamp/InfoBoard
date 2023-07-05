using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoBoard.Models
{
    internal class Categories
    {       
        public int id { get; set; }
        public string name { get; set; }
        public int user_id { get; set; }
        public string color { get; set; }
        public int random_order { get; set; }
        public List<MediaInformation> files { get; set; }         
    }
}
