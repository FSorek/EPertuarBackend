using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using EPertuarWeb.Data.Download;
using EPertuarWeb.Models;

namespace EPertuarWeb
{
    public class Startup
    {
        public static SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
        {
            DataSource = "epertuar-server.database.windows.net",
            UserID = "okito",
            Password = "Dragon12",
            InitialCatalog = "EPertuarDB",
            PersistSecurityInfo = false,
            MultipleActiveResultSets = true,
            Encrypt = true,
            TrustServerCertificate = false,
            ConnectTimeout = 30
        };
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            try
            {
                DataRequestService request = new DataRequestService();
                //UpdateDatabase(request);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.LineNumber);
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        private void UpdateDatabase(DataRequestService request)
        {
            using (SqlConnection con = new SqlConnection(builder.ConnectionString))
            {
                Debug.WriteLine("UpdataDB");
                con.Open();

                string sql = "TRUNCATE TABLE Show";
                using (SqlCommand sqlCommand = new SqlCommand(sql, con))
                    sqlCommand.ExecuteNonQuery();
              
                sql = "select * from Cinema Where CinemaType=" + ((int)CinemaType.multikino).ToString();

                AddMovies(sql, con, request);
                con.Close();
            }
        }

        private void AddMovies(string sql, SqlConnection con, DataRequestService request)
        {
            using (SqlCommand command = new SqlCommand(sql, con))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.WriteLine("Adding Movies..");
                        request.ProvideData(CinemaType.multikino, reader.GetInt32(1));
                        //ADD MOVIES TO DB
                        foreach (var movie in request.MovieList)
                        {
                            Debug.WriteLine("Adding " + movie.Name);
                            if (movie.Original_Name == null) movie.Original_Name = movie.Name;
                            movie.Name = Regex.Replace(movie.Name, "[^a-zA-Z]", "").ToLower();
                            //Check if movie exists in DB
                            UpdateOrInsertMovies(con, movie);
                        }
                    }
                }
            }
        }

        private void UpdateOrInsertMovies(SqlConnection con, MovieItem movie)
        {
            string sql = "Select * From Movie Where Name='" + movie.Name + "'";
            using (SqlCommand movieCommand = new SqlCommand(sql, con))
            {
                using (SqlDataReader movieReader = movieCommand.ExecuteReader())
                {
                    Debug.WriteLine("Is it in DB?");
                    if (movieReader.HasRows)
                    {
                        Debug.WriteLine("Yes");
                        UpdateMovie(con, movie);
                    }
                    else
                    {
                        Debug.WriteLine("No");
                        AddNewMovie(con, movie);
                    }
                }
            }
        }

        private void UpdateMovie(SqlConnection con, MovieItem movie)
        {
            string sql = String.Format(@"UPDATE MOVIE SET 
                                                        Name=           (COALESCE(Name,'{0}')),
                                                        Original_Name=  (COALESCE(Original_Name,'{1}')), 
                                                        Length=         (COALESCE(Length,{2})), 
                                                        Director=       (COALESCE(Director,'{3}')),
                                                        Writers=        (COALESCE(Writers,'{4}')), 
                                                        Stars=          (COALESCE(Stars,'{5}')), 
                                                        Storyline=      (COALESCE(Storyline,'{6}')), 
                                                        Trailer=        (COALESCE(Trailer,'{7}')), 
                                                        Music=          (COALESCE(Music,'{8}')), 
                                                        Cinematography= (COALESCE(Cinematography,'{9}')), 
                                                        Rating=         (COALESCE(Rating,'{10}')), 
                                                        Id_Self=        (COALESCE(Id_Self,'{11}'))
                                                        OUTPUT INSERTED.Id_Movie
                                                        WHERE           Name='{12}'",
                                     movie.Name, movie.Original_Name, movie.Length, movie.Director, movie.Writers, movie.Stars,
                                     movie.Storyline, movie.Trailer, movie.Music, movie.Cinematography, movie.Rating, movie.Id_Movie, movie.Name);

            SqlCommand sqlCommand = new SqlCommand(sql, con);
            sqlCommand.ExecuteNonQuery();

            sql = "SELECT Id_Movie from Movie Where Name='" + movie.Name + "'";
            sqlCommand = new SqlCommand(sql, con);
            using (SqlDataReader updateReader = sqlCommand.ExecuteReader())
            {
                Debug.WriteLine("Movie Updated, adding shows.");
                while (updateReader.Read())
                {
                    foreach (var movieShow in movie.Shows)
                     {
                        sql = String.Format(
                            @"INSERT INTO Show (Id_Cinema, Id_Movie, ShowDate, Start, Room, is3D, Language)
                                                            Values ({0},{1},'{2}','{3}','{4}',{5},'{6}')",
                            movieShow.Id_Cinema, updateReader.GetInt32(0), movieShow.ShowDate.Date,
                            movieShow.Start, movieShow.Room, movieShow.is3D ? 1 : 0, movieShow.Language);

                        sqlCommand = new SqlCommand(sql, con);
                        sqlCommand.ExecuteNonQuery();
                        Debug.WriteLine("Added Show from Update!");
                    }
                }
            }
            Debug.WriteLine("Updated movie!");
        }

        private void AddNewMovie(SqlConnection con, MovieItem movie)
        {
            string sql = String.Format(@"INSERT INTO Movie (Name, Original_Name, Length, Director,
                                                                             Writers, Stars, Storyline, Trailer, Music, 
                                                                             Cinematography, Rating, Id_Self)
                                                        OUTPUT INSERTED.Id_Movie
                                                        VALUES ('{0}', '{1}', {2}, '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')",
                movie.Name, movie.Original_Name, movie.Length, movie.Director, movie.Writers, movie.Stars,
                movie.Storyline, movie.Trailer, movie.Music, movie.Cinematography, movie.Rating, movie.Id_Movie);

            SqlCommand sqlCommand = new SqlCommand(sql, con);
            sqlCommand.ExecuteNonQuery();

            sql = "SELECT Id_Movie from Movie Where Name='" + movie.Name + "'";
            sqlCommand = new SqlCommand(sql, con);
            using (SqlDataReader updateReader = sqlCommand.ExecuteReader())
            {
                Debug.WriteLine("Movie Added, adding shows.");
                while (updateReader.Read())
                {
                    foreach (var movieShow in movie.Shows)
                    {
                        sql = String.Format(
                            @"INSERT INTO Show (Id_Cinema, Id_Movie, ShowDate, Start, Room, is3D, Language)
                                                            Values ({0},{1},'{2}','{3}','{4}',{5},'{6}')",
                            movieShow.Id_Cinema, updateReader.GetInt32(0), movieShow.ShowDate.Date,
                            movieShow.Start, movieShow.Room, movieShow.is3D ? 1 : 0, movieShow.Language);

                        sqlCommand = new SqlCommand(sql, con);
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }

            Debug.WriteLine("Added movie!");
        }
    }
}
