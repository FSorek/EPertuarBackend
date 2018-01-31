using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EPertuarWeb.Data.Download;
using EPertuarWeb.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace EPertuarWeb
{
    public class Program
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

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
