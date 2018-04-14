using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EPertuarWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Tracing;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using J = Newtonsoft.Json.JsonPropertyAttribute;
namespace EPertuarWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/Rating")]
    public class RatingController : Controller
    {
        SqlConnection con = new SqlConnection(Program.builder.ConnectionString);
        [HttpGet]
        public List<RatingItem> Get(int idC, int idMovie)
        {
            con.Open();
            List<RatingItem> rating = new List<RatingItem>();

            using (SqlCommand getMovie =
                new SqlCommand(@"Select * from UserRating WHERE Id_Cinema=" + idC + "AND Id_Movie=" + idMovie,
                    con)
            )
            {
                using (SqlDataReader reader = getMovie.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rating.Add(new RatingItem()
                        {
                            Id_User = reader.GetInt32(0),
                            Id_Movie = idMovie,
                            Id_Cinema = idC,
                            Id_StringUser = reader.GetString(3),
                            Cleanliness = reader.GetInt32(4),
                            Popcorn = reader.GetInt32(5),
                            Screen = reader.GetInt32(6),
                            Seat = reader.GetInt32(7),
                            Sound = reader.GetInt32(8)
                        });
                    }
                }
            }

            con.Close();
            return rating;
        }

        [HttpPost]
        public IActionResult Post([FromBody] RatingItem item)
        {
            try
            {
                if (item == null)
                    return BadRequest();
                con.Open();
                item = GetUser(item);
                if(CheckExistingRating(item)) // if the user has already rated the movie in the specific cinema then dont insert
                    return BadRequest();

                using (SqlCommand addRating =
                    new SqlCommand(String.Format(@"INSERT INTO USERRATING
                              (ID_USER, ID_MOVIE, ID_CINEMA, ID_STRINGUSER, CLEANLINESS, POPCORN, SCREEN, SEAT, SOUND)
                        VALUES({0}, {1}, {2}, '{3}', {4}, {5}, {6}, {7}, {8})", item.Id_User, item.Id_Movie,
                            item.Id_Cinema
                            , item.Id_StringUser, item.Cleanliness, item.Popcorn, item.Screen, item.Seat, item.Sound),
                        con)
                )
                {
                    addRating.ExecuteNonQuery();
                }


                con.Close();
            }
            catch (SqlException ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Message);
            }

            return Ok();
        }

        private bool CheckExistingRating(RatingItem item)
        {
            using (SqlCommand getRating =
                new SqlCommand(String.Format(@"SELECT * FROM [USERRATING] WHERE Id_User={0} AND Id_Cinema={1} AND Id_Movie={2}", item.Id_User, item.Id_Cinema, item.Id_Movie),
                    con)
            )
            {
                using (SqlDataReader reader = getRating.ExecuteReader())
                {
                    if (reader.HasRows)
                        return true;
                }
            }

            return false;
        }

        private int AddNewUser(RatingItem item)
        {
            int id=0;
            using (SqlCommand addUser =
                new SqlCommand(String.Format(@"INSERT INTO [User](Id_String) VALUES('{0}')", item.Id_StringUser),
                    con)
            )
            {
                addUser.ExecuteNonQuery();
            }

            using (SqlCommand getUser =
                new SqlCommand(String.Format(@"SELECT * FROM [USER] WHERE Id_String='{0}'", item.Id_StringUser),
                    con)
            )
            {
                using (SqlDataReader reader = getUser.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                    }
                }
            }

            return id;
        }

        private RatingItem GetUser(RatingItem item)
        {
            using (SqlCommand getUser =
                new SqlCommand(String.Format(@"SELECT * FROM [USER] WHERE Id_String='{0}'", item.Id_StringUser),
                    con)
            )
            {
                using (SqlDataReader reader = getUser.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            item.Id_User = reader.GetInt32(0);
                        }
                    }
                    else
                    {
                        item.Id_User = AddNewUser(item);
                    }
                }
            }
            return item;
        }
    }
}