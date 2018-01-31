using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            List<MovieItem> movies = new List<MovieItem>();
            List<ShowItem> shows = new List<ShowItem>();
            con.Open();
            ShowsView = GetCinemasByCity(City, ShowsView);

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
                        using (SqlCommand getShows = new SqlCommand(@"Select ShowDate, [Start], is3D, [Language] from Show 
                                                                INNER JOIN Movie ON Show.Id_Movie=Movie.Id_Movie 
                                                                INNER JOIN Cinema ON Show.Id_Cinema=Cinema.Id_Cinema
                                                                WHERE Cinema.Id_Cinema=" + cinemaItem.Id_Cinema + @"
                                                                AND Movie.Original_Name='" + movie.Original_Name +"'", con)
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
                        cinemaItem.MoviesPlayed.Add(movie);
                    }
                }
            }

            con.Close();
            return ShowsView.ToArray();
        }




        private List<CinemaItem> GetCinemasByCity(String City, List<CinemaItem> ShowsView)
        {
            using (SqlCommand getMultikinos = new SqlCommand(@"Select Id_Cinema, Name, City from Cinema WHERE City = '" + City + "';", con)
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
                            MoviesPlayed = new List<MovieItem>()
                        });
                    }
                }
            }
            return ShowsView;
        }
    }
}