using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPertuarWeb.Models
{
    public class ShowsViewItem
    {
        public int IdCinema { get; set; }
        public string CinemaName { get; set; }
        public string CinemaCity { get; set; }
        public string CinemaType { get; set; }
        public List<CompactMovie> Movies { get; set; }
    }

    public class CompactMovie
    {
        public int id { get; set; }
        public string MovieName { get; set; }
        public List<CompactShow> ShowList { get; set; }
        public List<String> Genres { get; set; }
        public double averageRating { get; set; }
    }

    public class CompactShow
    {
        public DateTime ShowDate { get; set; }
        public String Start { get; set; }
        public bool Is3D { get; set; }
        public String Language { get; set; }
    }
}
