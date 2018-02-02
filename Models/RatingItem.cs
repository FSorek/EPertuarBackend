using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPertuarWeb.Models
{
    public class RatingItem
    {
        public int Id_User { get; set; }
        public int Id_Cinema { get; set; }
        public int Id_Movie { get; set; }
        public String Id_StringUser { get; set; }
        public int Screen { get; set; }
        public int Seat { get; set; }
        public int Sound { get; set; }
        public int Popcorn { get; set; }
        public int Cleanliness { get; set; }
    }
}
