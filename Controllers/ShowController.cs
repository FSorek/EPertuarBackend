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
        public ShowsViewItem[] GetShowViewByCity(string City)
        {
            List<ShowsViewItem> ShowsView = new List<ShowsViewItem>();
            con.Open();
            ShowsView = GetCinemasByCity(City, ShowsView);
            ShowsView = GetMovies(ShowsView);

            con.Close();
            return ShowsView.ToArray();
        }

        [Route("Distance")]
        [HttpGet]
        public ShowsViewItem[] GetShowViewByDistance(double Lng, double Lat, double range)
        {
            List<ShowsViewItem> ShowsView = new List<ShowsViewItem>();
            con.Open();
            ShowsView = GetCinemasByDistance(ShowsView, Lat, Lng, range);
            ShowsView = GetMovies(ShowsView);
            con.Close();
            return ShowsView.ToArray();
        }




        private List<ShowsViewItem> GetCinemasByCity(String City, List<ShowsViewItem> ShowsView)
        {
            using (SqlCommand getMultikinos = new SqlCommand(@"Select Id_Cinema, Name, City, CinemaType from Cinema WHERE City = '" + City + "';", con)
            )
            {
                using (SqlDataReader reader = getMultikinos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ShowsView.Add(new ShowsViewItem()
                        {
                            IdCinema = reader.GetInt32(0),
                            CinemaName = reader.GetString(1),
                            CinemaCity = reader.GetString(2),
                            CinemaType = reader.GetInt32(3).ToString(),
                            Movies = new List<CompactMovie>()
                        });
                    }
                }
            }
            return ShowsView;
        }

        private List<ShowsViewItem> GetCinemasByDistance(List<ShowsViewItem> ShowsView, double Lat, double Lng, double range)
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
                            ShowsView.Add(new ShowsViewItem()
                            {
                                IdCinema = reader.GetInt32(2),
                                CinemaName = reader.GetString(3),
                                CinemaCity = reader.GetString(4),
                                CinemaType = reader.GetInt32(5).ToString(),
                                Movies = new List<CompactMovie>()
                            });
                        }
                    }
                }
            }

            return ShowsView;
        }

        private CompactMovie AddShows(ShowsViewItem cinemaItem, CompactMovie movie)
        {
            using (SqlCommand getShows = new SqlCommand(@"Select ShowDate, [Start], is3D, [Language] from Show 
                                                                INNER JOIN Movie ON Show.Id_Movie=Movie.Id_Movie 
                                                                INNER JOIN Cinema ON Show.Id_Cinema=Cinema.Id_Cinema
                                                                WHERE Cinema.Id_Cinema=" + cinemaItem.IdCinema + @"
                                                                AND Movie.Original_Name='" + movie.MovieName + "'", con)
            )
            {
                using (SqlDataReader reader = getShows.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        movie.ShowList.Add(new CompactShow()
                        {
                            ShowDate = reader.GetDateTime(0),
                            Start = reader.GetString(1),
                            Is3D = reader.GetInt32(2) == 1,
                            Language = reader.GetString(3)
                        });
                    }
                }
            }
            return movie;
        }

        private CompactMovie AddGenres(CompactMovie movie)
        {
            using (SqlCommand getGenres = new SqlCommand(@"select Genre.[Name] from MovieGotGenre 
                                                                        INNER JOIN Genre ON MovieGotGenre.Id_Genre=Genre.Id_Genre 
                                                                        INNER JOIN Movie ON MovieGotGenre.Id_Movie=Movie.Id_Movie 
                                                                        WHERE Movie.[Original_Name] ='" + movie.MovieName + "'", con)
            )
            {
                using (SqlDataReader reader = getGenres.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        movie.Genres.Add(reader.GetString(0));
                    }
                }
            }

            return movie;
        }

        private List<ShowsViewItem> GetMovies(List<ShowsViewItem> ShowsView)
        {
            foreach (var cinemaItem in ShowsView)
                using (SqlCommand getMoviesPlayed = new SqlCommand(@"Select Movie.[Original_Name] from Show 
                                                                INNER JOIN Movie ON Show.Id_Movie=Movie.Id_Movie 
                                                                INNER JOIN Cinema ON Show.Id_Cinema=Cinema.Id_Cinema
                                                                WHERE Cinema.Id_Cinema=" + cinemaItem.IdCinema + @"
                                                                GROUP BY Movie.[Original_Name]", con)
            )
            {
                using (SqlDataReader movieReader = getMoviesPlayed.ExecuteReader())
                {
                    while (movieReader.Read())
                    {
                        CompactMovie movie = new CompactMovie();
                        movie.MovieName = movieReader.GetString(0);
                        movie.ShowList = new List<CompactShow>();
                        movie.Genres = new List<string>();
                        movie = AddShows(cinemaItem, movie);
                        movie = AddGenres(movie);
                        movie = getMovieInfo(movie);
                        cinemaItem.Movies.Add(movie);
                    }
                }
            }
            return ShowsView;
        }

        private CompactMovie getMovieInfo(CompactMovie movie)
        {
            using (SqlCommand getMoviesPlayed = new SqlCommand(@"SELECT * FROM MOVIE WHERE Original_Name='"+movie.MovieName+"'", con)
            )
            {
                using (SqlDataReader movieReader = getMoviesPlayed.ExecuteReader())
                {
                    while (movieReader.Read())
                    {
                        movie.id = movieReader.GetInt32(0);
                    }
                }
            }
            return movie;
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