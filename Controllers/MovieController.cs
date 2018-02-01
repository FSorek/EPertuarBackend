using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using EPertuarWeb.Data.Download;
using EPertuarWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EPertuarWeb.Controllers
{
    //[Produces("application/json")]
    [Route("api/Movie")]
    public class MovieController : Controller
    {
        SqlConnection con = new SqlConnection(Program.builder.ConnectionString);

        [HttpGet("{id}")]
        public MovieItem GetMovie(int id)
        {
            con.Open();
            
            var movie = new MovieItem();
            using (SqlCommand getMovie =
                new SqlCommand(@"Select * from Movie WHERE Id_Movie=" + id,
                    con)
            )
            {
                using (SqlDataReader reader = getMovie.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        movie = new MovieItem()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Original_Name = reader.GetString(2),
                            Length = reader.GetInt32(3),
                            Director = reader.GetString(4),
                            Writers = reader.GetString(5),
                            Stars = reader.GetString(6),
                            Storyline = reader.GetString(7),
                            Trailer = reader.GetString(8),
                            Music = reader.GetString(9),
                            Cinematography = reader.GetString(10),
                            Rating = reader.GetString(11),
                            Id_Movie = reader.GetString(12)
                        };
                    }
                }
            }

            con.Close();
            return movie;
        }

        [Route("Genres")]
        [HttpGet]
        public String[] GetAllGenres()
        {
            con.Open();
            List<String> genres = new List<string>();
            using (SqlCommand getMultikinos =
                new SqlCommand(@"Select Name from Genre",
                    con)
            )
            {
                using (SqlDataReader reader = getMultikinos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        genres.Add(reader.GetString(0));
                    }
                }
            }
            con.Close();
            return genres.ToArray();
        }
    }
}