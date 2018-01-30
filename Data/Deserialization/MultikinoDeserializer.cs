using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EPertuarWeb.Models;
using EPertuarWeb.Data.Access;

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

            foreach (Showing show in from.Showings)
            {
                foreach (Time time in show.Times)
                {
                    if (show.DateTime.Date.Equals(today.Date))
                    {
                        mappedList.Add(new ShowItem
                        {
                            Id_Movie = from.Id,
                            Id_Cinema = cinemaId,
                            ShowDate = show.DateTime,
                            Start = time.PurpleTime,
                            is3D = (time.ScreenType == "3D"),
                            Language = time.Tags[0].Name,
                            Room = -1
                        });
                    }
                }
            }
            return mappedList;
        }
    }
}
