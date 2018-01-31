using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPertuarWeb.Models
{
    public class ShowsViewItem
    {
        public string CinemaName { get; set; }
        public string CinemaCity { get; set; }
        public List<MovieItem> Movies { get; set; }
    }
}
