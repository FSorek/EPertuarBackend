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
                    String sql = "select Id_Self from Cinema Where CinemaType=" + ((int)CinemaType.multikino).ToString();
                    using (SqlCommand command = new SqlCommand(sql, con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                request.ProvideData(CinemaType.multikino, reader.GetInt32(0));
                                
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
