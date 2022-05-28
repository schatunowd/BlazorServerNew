using System;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace BlazorServerNew.Data
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Lang
    {
        public int id { get; set; }
        public string title { get; set; }
        public string name { get; set; }
        public string selfname { get; set; }
    }

    public class LocalizationType
    {
        public int id { get; set; }
        public string title { get; set; }
        public string short_title { get; set; }
        public string description { get; set; }
        public Lang lang { get; set; }
    }

    public class Storyboard
    {
        public string url { get; set; }
        public string content_format { get; set; }
        public int size_in_bytes { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Quality
    {
        public string quality { get; set; }
        public object size_in_bytes { get; set; }
    }

    public class Localization
    {
        public LocalizationType localization_type { get; set; }
        public int credits_begin_time { get; set; }
        public int duration { get; set; }
        public object forced_subs_id { get; set; }
        public Storyboard storyboard { get; set; }
        public List<Quality> qualities { get; set; }
        public int? id { get; set; }
    }

    public class SubtitleType
    {
        public int id { get; set; }
        public string title { get; set; }
        public string short_title { get; set; }
        public string description { get; set; }
        public Lang lang { get; set; }
    }

    public class Subtitle
    {
        public int? id { get; set; }
        public SubtitleType subtitle_type { get; set; }
        public object localization_id { get; set; }
    }

    public class Shield
    {
        public int id { get; set; }
        public string name { get; set; }
        public string short_name { get; set; }
        public string ds_style { get; set; }
        public string type { get; set; }
        public List<string> place { get; set; }
    }

    public class Ready
    {
        public int votes { get; set; }
        public double main { get; set; }
        public double director { get; set; }
        public double pretty { get; set; }
        public double actors { get; set; }
        public double story { get; set; }
    }

    public class Rating
    {
        public Ready ready { get; set; }
    }

    public class ExtraProperties
    {
    }

    public class Poster
    {
        public int width { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public int id { get; set; }
        public int height { get; set; }
        public string content_format { get; set; }
    }

    public class SubscriptionName
    {
        public int id { get; set; }
        public string name { get; set; }
        public string brand { get; set; }
        public string style { get; set; }
    }

    public class IviReleaseInfo
    {
        public object date_interval_min { get; set; }
        public object date_interval_max { get; set; }
    }

    public class Season
    {
        public int season_id { get; set; }
        public int number { get; set; }
        public bool purchasable { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string hru { get; set; }
        public List<int> subscription_ids { get; set; }
        public IviReleaseInfo ivi_release_info { get; set; }
        public bool used_to_be_paid { get; set; }
        public List<string> content_paid_types { get; set; }
        public bool allow_download { get; set; }
        public List<string> allow_download_paid_types { get; set; }
        public int min_episode { get; set; }
        public int max_episode { get; set; }
        public int episode_count { get; set; }
        public bool fake { get; set; }
    }

    public class Result
    {
        public int year { get; set; }
        public int id { get; set; }
        public int kind { get; set; }
        public string title { get; set; }
        public string orig_title { get; set; }
        public List<int> categories { get; set; }
        public List<int> genres { get; set; }
        public int country { get; set; }
        public string kp_rating { get; set; }
        public string imdb_rating { get; set; }
        public double ivi_rating_10 { get; set; }
        public int ivi_rating_10_count { get; set; }
        public List<Localization> localizations { get; set; }
        public int restrict { get; set; }
        public List<Subtitle> subtitles { get; set; }
        public bool has_awards { get; set; }
        public bool has_reviews { get; set; }
        public bool has_comments { get; set; }
        public List<Shield> shields { get; set; }
        public Rating rating { get; set; }
        public ExtraProperties extra_properties { get; set; }
        public List<Poster> posters { get; set; }
        public bool used_to_be_paid { get; set; }
        public bool has_5_1 { get; set; }
        public bool drm_only { get; set; }
        public bool hd_available { get; set; }
        public List<string> content_paid_types { get; set; }
        public List<SubscriptionName> subscription_names { get; set; }
        public int? budget { get; set; }
        public int? gross_usa { get; set; }
        public int? gross_world { get; set; }
        public string hru { get; set; }
        public List<int> years { get; set; }
        public bool? has_upcoming_episodes { get; set; }
        public List<Season> seasons { get; set; }
    }

    public class RootIvi
    {
        public List<Result> result { get; set; }
        public string session_data { get; set; }
    }

    public class ExecIvi
    {
        private static string retrieveData(string url)
        {
            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[16000];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 100000;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0";
            request.KeepAlive = false;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;
            do
            {
                count = resStream.Read(buf, 0, buf.Length);
                if (count != 0)
                {
                    tempString = Encoding.UTF8.GetString(buf, 0, count);
                    sb.Append(tempString);
                }
            }
            while (count > 0);
            return sb.ToString();
        }

        public static List<double> getScoreByFilmId(int id)
        {
            List<double> description = new List<double>();
            string url = "https://www.ivi.ru/watch/" + id;
            string htmlSource = retrieveData(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlSource);
            string xPath, tmpstr;
            double score;
            HtmlNode node = null;
            for (int i = 2; i < 10; i += 2)
            {
                xPath = "/html/body/div[3]/div/div[2]/section[1]/div/div/div/div[2]/div[1]/div[2]/div[" + i + "]/div[2]";
                node = doc.DocumentNode.SelectSingleNode(xPath);
                if (node == null)
                    break;
                tmpstr = node.GetAttributeValue("style", "def");
                if (tmpstr.Contains('.'))
                    tmpstr = tmpstr.Substring(6, tmpstr.IndexOf('.'));
                else
                    tmpstr = tmpstr.Substring(6, tmpstr.Length - 7);
                tmpstr = tmpstr.Replace('.', ',');
                score = Convert.ToDouble(tmpstr);
                description.Add(score);
            }
            return description;
        }

        private static int getIdByName(string Url)
        {
            WebRequest req = WebRequest.Create(Url);
            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
            RootIvi datalist = JsonConvert.DeserializeObject<RootIvi>(Out);
            int id = datalist.result[0].id;
            return id;
        }

        static public string Exec(string filmname)
        {
            List<double> scores = new List<double>();
            Dictionary<string, double> scoresDictionary = new Dictionary<string, double>();
            string url = "https://api2.ivi.ru/mobileapi/search/v7/?query=" + filmname + "&from=0&to=39&user_ab_bucket=13426&fields=id%2Ctitle%2Cimages%2Cfake%2Cpreorderable%2Chru%2Cposters%2Cextra_properties%2Ccontent_paid_types%2Csubscription_names%2Ckind%2Crestrict%2Chd_available%2C3d_available%2Cuhd_available%2Chdr10_available%2Chas_5_1%2Cartists%2Cbudget%2Ccategories%2Ccountry%2Cduration%2Clocalizations%2Cgenres%2Cgross_russia%2Cgross_usa%2Cgross_world%2Cimdb_rating%2Civi_rating_10%2Ckp_rating%2Crating%2Cseason%2Corig_title%2Cyear%2Cyears%2Cepisode%2Csubtitles%2Ccompilation%2Cdrm_only%2Chas_awards%2Cused_to_be_paid%2Cseasons_count%2Cseasons%2Cepisodes%2Civi_rating_10_count%2Chas_comments%2Chas_reviews%2Chas_upcoming_episodes%2Cyear%2Cyears%2Ccountry%2Cshields&app_version=870&session=0ac11fb24559933298829694_1666797808-0sqCcdbanGVwV_cPAxGa0tA&session_data=eyJ1aWQiOjQ1NTk5MzMyOTg4Mjk2OTR9.YmgoCQ.vj_b3cHO88iWBXLbisq9iJtOSLU";
            int id = getIdByName(url);
            scores = getScoreByFilmId(id);
            if (scores.Count == 4)
            {
                scoresDictionary.Add("Режиссура", scores[0]);
                scoresDictionary.Add("Зрелищность", scores[1]);
                scoresDictionary.Add("Актеры", scores[2]);
                scoresDictionary.Add("Сюжет", scores[3]);
            }
            else
                return "";
            var maxKey = "";
            double max = double.MinValue;
            foreach (var a in scoresDictionary)
            {
                if (a.Value > max)
                {
                    maxKey = a.Key;
                    max = a.Value;
                }
            }
            return maxKey;
        }
    }
}
