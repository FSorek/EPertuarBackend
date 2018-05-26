using System;
using System.Collections.Generic;

namespace EPertuarWeb.Models
{
    public class V2CinemaItem
    {
        //--------- CinemaV2
        public int Id_Self { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
        public int CinemaType { get; set; }

        //---------- AdressV2
        public String Street { get; set; }
        public String Number { get; set; }
        public String PostalCode { get; set; }
        public double Longtitude { get; set; }
        public double Latitude { get; set; }

        //---------- CityV2
        public string City { get; set; }
    }

}
