using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using EPertuarWeb.Data.Access;
using EPertuarWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EPertuarWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/Cinema")]
    public class CinemaController : Controller
    {
        SqlConnection con = new SqlConnection(Program.builder.ConnectionString);

        [HttpGet("{City}")]
        public CinemaItem[] GetAllCinemas(string City)
        {
            List<CinemaItem> Cinemas = new List<CinemaItem>();
            con.Open();
            using (SqlCommand getMultikinos = new SqlCommand("Select * from Cinema WHERE City='"+City+"'", con))
            {
                using (SqlDataReader reader = getMultikinos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cinemas.Add(new CinemaItem
                        {
                            Id_Cinema = reader.GetInt32(0),
                            Id_Self = reader.GetInt32(1),
                            Name = reader.GetString(2),
                            Phone = reader.GetString(3),
                            Longtitude = reader.GetDouble(4),
                            Latitude = reader.GetDouble(5),
                            City = reader.GetString(6),
                            CinemaType = reader.GetInt32(7).ToString(),
                        });
                    }
                }
            }
            con.Close();
            return Cinemas.ToArray();
        }




        [Route("Cities")]
        [HttpGet]
        public string[] GetAllCities()
        {
            List<String> Cities = new List<string>();
            con.Open();
            using (SqlCommand getCities = new SqlCommand("Select City from Cinema", con))
            {
                using (SqlDataReader reader = getCities.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!Cities.Contains(reader.GetString(0)))
                            Cities.Add(reader.GetString(0));
                    }
                }
            }
            con.Close();
            return Cities.ToArray();
        }
    }
}