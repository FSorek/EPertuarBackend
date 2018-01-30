using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using EPertuarWeb.Data.Download;
using EPertuarWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EPertuarWeb.Controllers
{
    //[Produces("application/json")]
    [Route("api/Movie")]
    public class MovieController : Controller
    {
        SqlConnection con = new SqlConnection(Startup.builder.ConnectionString);
        
        private MovieItem[] movies = new MovieItem[]
        {
            new MovieItem{Id = 3, Id_Movie = "Film", Cinematography = "", Director = "Michael Bay", Genre = new List<string>{"Action", "Horror"}, Length = 90},
        };

        [HttpGet("{id}")]
        public MovieItem GetProduct(int id)
        {
            var movie = movies.FirstOrDefault((p) => p.Id == id);
            if (movie == null)
            {
                return null;
            }
            return movie;
        }
    }
}