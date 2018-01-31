using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EPertuarWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/Cinema")]
    public class CinemaController : Controller
    {
        SqlConnection con = new SqlConnection(Program.builder.ConnectionString);

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