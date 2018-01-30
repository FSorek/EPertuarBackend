using System;
using System.Collections.Generic;
using System.Linq;
using EPertuarWeb.Models;
using EPertuarWeb.Data.Access;

namespace EPertuarWeb.Data.Deserialization
{
    public class CinemaCityDeserializer
    {
        private CinemaCity root;

        public List<MovieItem> Deserialize(string json, int cinemaId)
        {
            root = CinemaCity.FromJson(json);

            return MapMovie(root, cinemaId);
        }

        public List<MovieItem> MapMovie(CinemaCity from, int cinemaId)
        {
            List<MovieItem> mappedList = new List<MovieItem>();

            foreach (CityFilm film in from.Body.Films)
            {
                List<string> genres = new List<string>();
                foreach(var genre in film.AttributeIds)
                {
                    genres.Add(genre.ToLower());
                }
                mappedList.Add(new MovieItem
                {
                    Id_Movie = film.Id,
                    Name = film.Name,
                    Director = null,
                    Storyline = null,
                    Trailer = film.VideoLink,
                    Length = (int)film.Length,
                    Original_Name = null,
                    Writers = null,
                    Stars = null,
                    Music = null,
                    Cinematography = null,
                    Rating = null,
                    Shows = MapShow(from, film.Id, cinemaId),
                    Genre = genres
                });

            }
            return mappedList;
        }

        private static List<ShowItem> MapShow(CinemaCity from, string id, int cinemaId)
        {
            List<ShowItem> mappedList = new List<ShowItem>();

            foreach (Event show in from.Body.Events)
            {
                if (show.FilmId != id) continue;
                    mappedList.Add(new ShowItem
                    {
                        Id_Movie = Int32.Parse(id),
                        Id_Cinema = cinemaId,
                        ShowDate = show.BusinessDay,
                        Start = show.EventDateTime.Remove(0,10),
                        is3D = (show.AttributeIds.Contains("2d")),
                        Language = LanguageFinder(show.AttributeIds),

                        Room = -1
                    });
            }
            return mappedList;
        }

        private static string LanguageFinder(string[] toSearch)
        {
            if (toSearch.Contains("dubbed") || toSearch.Contains("original-lang-pl"))
                return "PL";
            else return "EN";
        }
    }
}
