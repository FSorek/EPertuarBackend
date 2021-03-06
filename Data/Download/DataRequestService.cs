﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EPertuarWeb.Data.Deserialization;
using EPertuarWeb.Models;

namespace EPertuarWeb.Data.Download
{
    public enum CinemaType
    {
        multikino, cinemacity,
    }

    public class DataRequestService
    {
        public List<MovieItem> MovieList { get; set; }

        private static string GetMultikinoUri(int cinemaId) => String.Format("https://multikino.pl/data/filmswithshowings/{0}", cinemaId);
        private static string GetCinemaCityUri(int cinemaId)
        {
            var today = DateTime.Today;
            string day = today.Day.ToString();
            string month = today.Month.ToString();
            string zeroDay;
            string zeroMonth;

            if (today.Day < 10)
            {
                zeroDay = "0";
            }
            else
            {
                zeroDay = "";
            }

            if (today.Month < 10)
            {
                zeroMonth = "0";
            }
            else
            {
                zeroMonth = "";
            }

            return String.Format("https://www.cinema-city.pl/pl/data-api-service/v1/quickbook/10103/film-events/in-cinema/{0}/at-date/{1}-{2}-{3}?attr=&lang=pl_PL",
                cinemaId, today.Year, zeroMonth + month, zeroDay + day);
        }

        public String GetResponse(CinemaType cinemaType, int cinemaId)
        {
            var client = new HttpClient();
            string uri = null;

            switch (cinemaType)
            {
                case CinemaType.multikino: uri = GetMultikinoUri(cinemaId); break;
                case CinemaType.cinemacity: uri = GetCinemaCityUri(cinemaId); break;
            }

            var httpResponse = client.GetAsync(uri).Result;
            httpResponse.EnsureSuccessStatusCode();
            var responseStream = httpResponse.Content.ReadAsStringAsync();
            return responseStream.Result;
        }

        public void ProvideData(CinemaType cinemaType, int cinemaId)
        {
            string dataString = GetResponse(cinemaType, cinemaId);
            switch (cinemaType)
            {
                default: MovieList = new List<MovieItem>(); break;
                case CinemaType.multikino:
                    MultikinoDeserializer multikinoDeserializer = new MultikinoDeserializer();
                    MovieList = multikinoDeserializer.Deserialize(dataString, cinemaId);
                    break;
                case CinemaType.cinemacity:
                    CinemaCityDeserializer cinemaCityDeserializer = new CinemaCityDeserializer();
                    MovieList = cinemaCityDeserializer.Deserialize(dataString, cinemaId);
                    break;
            }
            
        }
    }
}
