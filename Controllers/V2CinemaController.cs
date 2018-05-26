using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using EPertuarWeb.Models;

namespace EPertuarWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/v2/Cinema")]
    public class V2CinemaController : Controller
    {
        SqlConnection con = new SqlConnection(Program.builder.ConnectionString);

        [HttpGet("All")]
        public V2CinemaItem[] GetAllCinemas(string City)
        {
            List<V2CinemaItem> Cinemas = new List<V2CinemaItem>();
            con.Open();
            using (SqlCommand getMultikinos = new SqlCommand(@"
                    SELECT Id_Self, [dbo].[Cinemav2].[Name] AS Cinema, Email, Phone, CinemaType, Street, Number, Postal_Code, Longtitude, Latitude, [dbo].[Cityv2].[Name] AS City
                    FROM [dbo].[Cinemav2]
                    INNER JOIN [dbo].[Adressv2] ON [dbo].[Cinemav2].[Id_Adress] = [dbo].[Adressv2].[Id_Adress]
                    INNER JOIN [dbo].[Cityv2] ON [dbo].[Adressv2].[Id_City] = [dbo].[Cityv2].[Id_City]", con))
            {
                using (SqlDataReader reader = getMultikinos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cinemas.Add(new V2CinemaItem
                        {
                            Id_Self = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Email = reader.GetString(2),
                            Phone = reader.GetString(3),
                            CinemaType = reader.GetInt32(4),

                            Street = reader.GetString(5),
                            Number = reader.GetString(6),
                            PostalCode = reader.GetString(7),
                            Longtitude = reader.GetDouble(8),
                            Latitude = reader.GetDouble(9),

                            City = reader.GetString(10)
                        });
                    }
                }
            }
            con.Close();
            return Cinemas.ToArray();
        }
    }
}