using System;
using System.Collections.Generic;
using EPertuarWeb.Models;

namespace EPertuarWeb.Models
{
    public class CinemaItem
    {
        public int Id_Cinema { get; set; }
        public int Id_Self { get; set; }
        public String Name { get; set; }
        public String Phone { get; set; }
        public double Longtitude { get; set; }
        public double Latitude { get; set; }
        public string City { get; set; }
        public string CinemaType { get; set; }
        public List<MovieItem> MoviesPlayed { get; set; }
    }
}
