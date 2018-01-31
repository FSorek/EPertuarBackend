using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EPertuarWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EPertuarWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/Show")]
    public class ShowController : Controller
    {
        SqlConnection con = new SqlConnection(Program.builder.ConnectionString);

        [HttpGet("{City}")]
        public CinemaItem[] GetShowViewByCity(string City)
        {
            List<CinemaItem> ShowsView = new List<CinemaItem>();
            con.Open();
            ShowsView = GetCinemasByCity(City, ShowsView);
            ShowsView = GetMovies(ShowsView);

            con.Close();
            return ShowsView.ToArray();
        }

        [Route("Distance")]
        [HttpGet]
        public CinemaItem[] GetShowViewByDistance(double Lng, double Lat, double range)
        {
            List<CinemaItem> ShowsView = new List<CinemaItem>();
            con.Open();
            ShowsView = GetCinemasByDistance(ShowsView, Lat, Lng, range);
            ShowsView = GetMovies(ShowsView);
            con.Close();
            return ShowsView.ToArray();
        }




        private List<CinemaItem> GetCinemasByCity(String City, List<CinemaItem> ShowsView)
        {
            using (SqlCommand getMultikinos = new SqlCommand(@"Select Id_Cinema, Name, City, CinemaType from Cinema WHERE City = '" + City + "';", con)
            )
            {
                using (SqlDataReader reader = getMultikinos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ShowsView.Add(new CinemaItem()
                        {
                            Id_Cinema = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            City = reader.GetString(2),
                            CinemaType = reader.GetInt32(3).ToString(),
                            MoviesPlayed = new List<MovieItem>()
                        });
                    }
                }
            }
            return ShowsView;
        }

        private List<CinemaItem> GetCinemasByDistance(List<CinemaItem> ShowsView, double Lat, double Lng, double range)
        {
            using (SqlCommand getMultikinos =
                new SqlCommand(@"Select Longtitude, Latitude, Id_Cinema, Name, City, CinemaType from Cinema", con)
            )
            {
                using (SqlDataReader reader = getMultikinos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        double cinemaLat = reader.GetDouble(1);
                        double cinemaLng = reader.GetDouble(0);
                        double distance = CalculateDistance(cinemaLat, cinemaLng, Lat, Lng);
                        if (distance <= range)
                        {
                            ShowsView.Add(new CinemaItem()
                            {
                                Id_Cinema = reader.GetInt32(2),
                                Name = reader.GetString(3),
                                City = reader.GetString(4),
                                CinemaType = reader.GetInt32(5).ToString(),
                                MoviesPlayed = new List<MovieItem>()
                            });
                        }
                    }
                }
            }

            return ShowsView;
        }

        private MovieItem AddShows(CinemaItem cinemaItem, MovieItem movie)
        {
            using (SqlCommand getShows = new SqlCommand(@"Select ShowDate, [Start], is3D, [Language] from Show 
                                                                INNER JOIN Movie ON Show.Id_Movie=Movie.Id_Movie 
                                                                INNER JOIN Cinema ON Show.Id_Cinema=Cinema.Id_Cinema
                                                                WHERE Cinema.Id_Cinema=" + cinemaItem.Id_Cinema + @"
                                                                AND Movie.Original_Name='" + movie.Original_Name + "'", con)
            )
            {
                using (SqlDataReader reader = getShows.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        movie.Shows.Add(new ShowItem()
                        {
                            ShowDate = reader.GetDateTime(0),
                            Start = reader.GetString(1),
                            is3D = reader.GetInt32(2) == 1,
                            Language = reader.GetString(3)
                        });
                    }
                }
            }
            return movie;
        }

        private MovieItem AddGenres(MovieItem movie)
        {
            using (SqlCommand getGenres = new SqlCommand(@"select Genre.[Name] from MovieGotGenre 
                                                                        INNER JOIN Genre ON MovieGotGenre.Id_Genre=Genre.Id_Genre 
                                                                        INNER JOIN Movie ON MovieGotGenre.Id_Movie=Movie.Id_Movie 
                                                                        WHERE Movie.[Original_Name] ='" + movie.Original_Name + "'", con)
            )
            {
                using (SqlDataReader reader = getGenres.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        movie.Genre.Add(reader.GetString(0));
                    }
                }
            }

            return movie;
        }

        private List<CinemaItem> GetMovies(List<CinemaItem> ShowsView)
        {
            foreach (var cinemaItem in ShowsView)
                using (SqlCommand getMoviesPlayed = new SqlCommand(@"Select Movie.[Original_Name] from Show 
                                                                INNER JOIN Movie ON Show.Id_Movie=Movie.Id_Movie 
                                                                INNER JOIN Cinema ON Show.Id_Cinema=Cinema.Id_Cinema
                                                                WHERE Cinema.Id_Cinema=" + cinemaItem.Id_Cinema + @"
                                                                GROUP BY Movie.[Original_Name]", con)
            )
            {
                using (SqlDataReader movieReader = getMoviesPlayed.ExecuteReader())
                {
                    while (movieReader.Read())
                    {
                        MovieItem movie = new MovieItem();
                        movie.Original_Name = movieReader.GetString(0);
                        movie.Shows = new List<ShowItem>();
                        movie.Genre = new List<string>();
                        movie = AddShows(cinemaItem, movie);
                        movie = AddGenres(movie);
                        cinemaItem.MoviesPlayed.Add(movie);
                    }
                }
            }
            return ShowsView;
        }

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return dist * 1.609344;
        }
    }
}