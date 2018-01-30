using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPertuarWeb.Models
{
    public class MovieItem
    {
        public int Id { get; set; }
        public String Id_Movie { get; set; }
        public String Name { get; set; }
        public String Original_Name { get; set; }
        public int Length { get; set; }
        public String Director { get; set; }
        public String Writers { get; set; }
        public String Stars { get; set; }
        public String Storyline { get; set; }
        public String Trailer { get; set; }
        public String Music { get; set; }
        public String Cinematography { get; set; }
        public String Rating { get; set; }
        public List<ShowItem> Shows { get; set; }
        public List<String> Genre { get; set; }
    }
}
