using System;

namespace EPertuarWeb.Models
{
    public class ShowItem
    {
        public int Id_Show { get; set; }
        public int Id_Cinema { get; set; }
        public int Id_Movie { get; set; }
        public DateTime ShowDate { get; set; }
        public string Start { get; set; }
        public int Room { get; set; }
        public bool is3D { get; set; }
        public String Language { get; set; }
    }
}
