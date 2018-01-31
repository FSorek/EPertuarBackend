using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EPertuarWeb.Models;
using EPertuarWeb.Data.Access;
using EPertuarWeb.Data.Download;

namespace EPertuarWeb.Data.Deserialization
{
    public class MultikinoDeserializer
    {
        private Multikino root;
        private static readonly Regex rxNonDigits = new Regex(@"[^\d]+");

        public List<MovieItem> Deserialize(string json, int cinemaId)
        {
            root = Multikino.FromJson(json);

            return MapMovie(root, cinemaId);
        }

        public List<MovieItem> MapMovie(Multikino from, int cinemaId)
        {
            List<MovieItem> mappedList = new List<MovieItem>();
            foreach (MultiFilm film in from.Films)
            {
                List<string> genres = new List<string>();
                foreach (var genre in film.Genres.Names)
                {
                    genres.Add(genre.Name.ToLower());
                }
                mappedList.Add(new MovieItem
                {
                    Id_Movie = film.Id,
                    Name = film.Title,
                    Director = film.InfoDirector,
                    Storyline = film.SynopsisShort,
                    Trailer = film.Videolink,
                    Length = (film.InfoRunningtime.Length > 0) ? Int32.Parse(rxNonDigits.Replace(film.InfoRunningtime, "")) : 0,

                    Original_Name = null,
                    Writers = null,
                    Stars = null,
                    Music = null,
                    Cinematography = null,
                    Rating = null,
                    Shows = MapShow(film, cinemaId),
                    Genre = genres
            });
                
            }
            return mappedList;
        }

        private static List<ShowItem> MapShow(MultiFilm from, int cinemaId)
        {
            List<ShowItem> mappedList = new List<ShowItem>();
            var today = DateTime.Today;
            String sql = String.Format("select Id_Cinema from Cinema Where Id_Self = {0} AND CinemaType = {1}", cinemaId, (int)CinemaType.multikino);
            SqlConnection con = new SqlConnection(Program.builder.ConnectionString);
            con.Open();

            foreach (Showing show in from.Showings)
            {
                foreach (Time time in show.Times)
                {
                    if (show.DateTime.Date.Equals(today.Date))
                    {
                        using (SqlCommand command = new SqlCommand(sql, con))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    mappedList.Add(new ShowItem
                                    {
                                        Id_Movie = Int32.Parse(from.Id),
                                        Id_Cinema = reader.GetInt32(0),
                                        ShowDate = show.DateTime,
                                        Start = time.PurpleTime,
                                        is3D = (time.ScreenType == "3D"),
                                        Language = time.Tags[0].Name,
                                        Room = -1
                                    });
                                }
                            }
                            
                        }

                    }
                }
            }
            con.Close();
            return mappedList;
        }
    }
}
