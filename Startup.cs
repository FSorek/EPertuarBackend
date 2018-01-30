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
using System.Text.RegularExpressions;
using EPertuarWeb.Data.Download;

namespace EPertuarWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    DataSource = "epertuar-server.database.windows.net",
                    UserID = "okito",
                    Password = "Dragon12",
                    InitialCatalog = "EPertuarDB",
                       PersistSecurityInfo = false,
                     MultipleActiveResultSets = false,
                       Encrypt = true,
                     TrustServerCertificate = false,
                       ConnectTimeout = 30
                };

                DataRequestService request = new DataRequestService();

                using (SqlConnection con = new SqlConnection(builder.ConnectionString))
                {
                    con.Open();
                    String sql = "select * from Cinema Where CinemaType=" + ((int)CinemaType.multikino).ToString();
                    using (SqlCommand command = new SqlCommand(sql, con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                request.ProvideData(CinemaType.multikino, reader.GetInt32(1));
                                //ADD MOVIES TO DB
                                foreach (var movie in request.MovieList)
                                {
                                    if (movie.Original_Name == null) movie.Original_Name = movie.Name;
                                    movie.Name = Regex.Replace(movie.Name, "[^a-zA-Z]", "").ToLower();
                                    //Check if movie exists in DB
                                    sql = "Select * From Movie Where Name=" + movie.Name;
                                    using (SqlCommand movieCommand = new SqlCommand(sql, con))
                                    {
                                        using (SqlDataReader movieReader = movieCommand.ExecuteReader())
                                        {
                                            if (movieReader.HasRows)
                                            {
                                                sql = String.Format(@"UPDATE MOVIE SET 
                                                        Name=           (COALESCE(Name,{0})),
                                                        Original_Name=  (COALESCE(Original_Name,{1})), 
                                                        Length=         (COALESCE(Length,{2})), 
                                                        Director=       (COALESCE(Director,{3})),
                                                        Writers=        (COALESCE(Writers,{4})), 
                                                        Stars=          (COALESCE(Stars,{5})), 
                                                        Storyline=      (COALESCE(Storyline,{6})), 
                                                        Trailer=        (COALESCE(Trailer,{7})), 
                                                        Music=          (COALESCE(Music,{8})), 
                                                        Cinematography= (COALESCE(Cinematography,{9})), 
                                                        Rating=         (COALESCE(Rating,{10})), 
                                                        Id_Self=        (COALESCE(Id_Self,{11}))
                                                        WHERE           Name={12}",
                                                      movie.Name, movie.Original_Name, movie.Length, movie.Director, movie.Writers, movie.Stars,
                                                      movie.Storyline, movie.Trailer, movie.Music, movie.Cinematography, movie.Rating, movie.Id_Movie, movie.Name);

                                                SqlCommand update = new SqlCommand(sql, con);
                                                update.ExecuteNonQuery();


                                            }
                                        }
                                    }

                                    sql = String.Format(@"INSERT INTO Movie (Name, Original_Name, Length, Director,
                                                                             Writers, Stars, Storyline, Trailer, Music, 
                                                                             Cinematography, Rating, Id_Self)
                                                        VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})",
                                            movie.Name, movie.Original_Name, movie.Length, movie.Director, movie.Writers, movie.Stars, 
                                            movie.Storyline, movie.Trailer, movie.Music, movie.Cinematography, movie.Rating, movie.Id_Movie);

                                    //ADD SHOWS TO DB
                                }


                            }
                        }
                    }
                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
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
    }
}
