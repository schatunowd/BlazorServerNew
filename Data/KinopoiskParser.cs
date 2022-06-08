using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Text;

namespace BlazorServerNew.Data
{
    public class RootImdb
    {
        public int? kinopoiskId { get; set; }
        public string? imdbId { get; set; }
        public string? nameRu { get; set; }
        public object? nameEn { get; set; }
        public string nameOriginal { get; set; }
        public string posterUrl { get; set; }
        public string posterUrlPreview { get; set; }
        public string coverUrl { get; set; }
        public string logoUrl { get; set; }
        public int? reviewsCount { get; set; }
        public double? ratingGoodReview { get; set; }
        public int? ratingGoodReviewVoteCount { get; set; }
        public double? ratingKinopoisk { get; set; }
        public int? ratingKinopoiskVoteCount { get; set; }
        public double? ratingImdb { get; set; }
        public int? ratingImdbVoteCount { get; set; }
        public double? ratingFilmCritics { get; set; }
        public int? ratingFilmCriticsVoteCount { get; set; }
        public object ratingAwait { get; set; }
        public int? ratingAwaitCount { get; set; }
        public double? ratingRfCritics { get; set; }
        public int? ratingRfCriticsVoteCount { get; set; }
        public string webUrl { get; set; }
        public int? year { get; set; }
        public int? filmLength { get; set; }
        public string slogan { get; set; }
        public string description { get; set; }
        public string shortDescription { get; set; }
        public object editorAnnotation { get; set; }
        public bool? isTicketsAvailable { get; set; }
        public object productionStatus { get; set; }
        public string type { get; set; }
        public string ratingMpaa { get; set; }
        public string ratingAgeLimits { get; set; }
        public List<Country> countries { get; set; }
        public List<Genre> genres { get; set; }
        public object startYear { get; set; }
        public object endYear { get; set; }
        public bool? serial { get; set; }
        public bool? shortFilm { get; set; }
        public bool? completed { get; set; }
        public bool? hasImax { get; set; }
        public bool? has3D { get; set; }
        public DateTime lastSync { get; set; }
    }
    public class Country
    {
        public string country { get; set; }
    }

    public class Genre
    {
        public string genre { get; set; }
    }

    public class Film
    {
        public int? filmId { get; set; }
        public string nameRu { get; set; }
        public string nameEn { get; set; }
        public string type { get; set; }
        public string year { get; set; }
        public string description { get; set; }
        public string filmLength { get; set; }
        public List<Country> countries { get; set; }
        public List<Genre> genres { get; set; }
        public string rating { get; set; }
        public int? ratingVoteCount { get; set; }
        public string posterUrl { get; set; }
        public string posterUrlPreview { get; set; }
    }

    public class Root
    {
        public string keyword { get; set; }
        public int? pagesCount { get; set; }
        public List<Film> films { get; set; }
        public int? searchFilmsCountResult { get; set; }
    }

    public class Review
    {
        public int? reviewId { get; set; }
        public string reviewType { get; set; }
        public DateTime reviewData { get; set; }
        public int? userPositiveRating { get; set; }
        public int? userNegativeRating { get; set; }
        public string reviewAutor { get; set; }
        public string reviewTitle { get; set; }
        public string reviewDescription { get; set; }
    }

    public class Root2
    {
        public int? page { get; set; }
        public int? filmId { get; set; }
        public int? reviewAllCount { get; set; }
        public object reviewAllPositiveRatio { get; set; }
        public object reviewPositiveCount { get; set; }
        public object reviewNegativeCount { get; set; }
        public object reviewNeutralCount { get; set; }
        public int pagesCount { get; set; }
        public List<Review> reviews { get; set; }
    }

    public class Program
    {

        private static string[] apiKey = File.ReadAllLines(@"D:\study\4.2\diplom\apiKeyKinopoisk.txt");
        static async Task<string> getFilmIdByFilmName(string filmName)
        {
            WebRequest request = WebRequest.Create("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-keyword?keyword=" + filmName + "&page=1");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("X-API-KEY", apiKey[0]);
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
            Root datalist = JsonConvert.DeserializeObject<Root>(answer);
            string filmId = datalist.films[0].filmId.ToString();
            return filmId;
        }

        static async Task<List<string>> getCommentsByFilmId(string filmId, bool reviewParam)
        {
            int pagesCounter = 1, allPages = 1;
            List<string> comments = new List<string>();
            do
            {
                WebRequest request = WebRequest.Create("https://kinopoiskapiunofficial.tech/api/v1/reviews?filmId=" + filmId + "&page=" + pagesCounter);
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("X-API-KEY", apiKey[0]);
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
                Root2 datalist = JsonConvert.DeserializeObject<Root2>(answer);
                allPages = datalist.pagesCount;
                foreach (Review rev in datalist.reviews)
                {
                    if (reviewParam)
                        comments.Add(rev.reviewDescription);
                    else
                    {
                        if (rev.reviewType == "POSITIVE" || rev.reviewType == "NEGATIVE")
                            comments.Add(rev.reviewDescription);
                    }
                }
                pagesCounter++;
                response.Close();
            } while (pagesCounter <= allPages);
            return comments;
        }
        static async Task<List<string>> getImdbIdByFilmId(string filmId)
        {
            WebRequest request = WebRequest.Create("https://kinopoiskapiunofficial.tech/api/v2.2/films/" + filmId);
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("X-API-KEY", apiKey[0]);
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
            RootImdb datalist = JsonConvert.DeserializeObject<RootImdb>(answer);
            List<string> scores = new List<string>();
            scores.Add(datalist.ratingKinopoisk.ToString());
            scores.Add(datalist.imdbId);
            
            return scores;
        }


        public static async Task<List<string>> ParserExec(string filmName, bool reviewParam)
        {
            string filmId = await getFilmIdByFilmName(filmName);
            List<string> comments = await getCommentsByFilmId(filmId, reviewParam);
            List<string> imdbId = await getImdbIdByFilmId(filmId);
            foreach (string a in imdbId)
                comments.Add(a);
            return comments;
        }
    }
}


