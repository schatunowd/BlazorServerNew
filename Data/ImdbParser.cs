using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BlazorServerNew.Data
{
    public class Imdb
    {
        public string imDbId { get; set; }
        public string title { get; set; }
        public string fullTitle { get; set; }
        public string type { get; set; }
        public string year { get; set; }
        public string imDb { get; set; }
        public string metacritic { get; set; }
        public string theMovieDb { get; set; }
        public string rottenTomatoes { get; set; }
        public string filmAffinity { get; set; }
        public string errorMessage { get; set; }
    }
    public class ImdbParser
    {
        static string[] apiKeyImdb = File.ReadAllLines(@"D:\study\4.2\diplom\apiKeyImdb.txt");
        public static List<string> getRatingsByFilmId(string filmId)
        {
            List<string> ratings = new List<string>();
            WebRequest request = WebRequest.Create("https://imdb-api.com/en/API/Ratings/" + apiKeyImdb[0] + "/" + filmId);
            request.Headers.Add("accept", "application/json");
            WebResponse response = request.GetResponse();
            string answer = "";
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        answer += line;
                    }
                }
            }
            response.Close();
            Imdb listOfRatings = JsonConvert.DeserializeObject<Imdb>(answer);
            ratings.Add(listOfRatings.filmAffinity);
            ratings.Add(listOfRatings.imDb);
            ratings.Add(listOfRatings.metacritic);
            ratings.Add(listOfRatings.rottenTomatoes);
            ratings.Add(listOfRatings.theMovieDb);
            return ratings;
        }
    }
}
