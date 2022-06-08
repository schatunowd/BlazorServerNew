
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Plot_model_new_ConsoleApp1;


namespace BlazorServerNew.Data
{
    public class Exec
    {
        private static string path_to_comments = @"D:\study\4.2\diplom\BlazorServerNew\wwwroot\comments\";
        public static string FilmName { get; set; }
        static List<string> Norm(List<string> comments)
        {
            List<string> all_str = new List<string>();
            foreach (string str in comments)
            {
                string new_str = Regex.Replace(str, "[^а-яА-Я;0-9 a-zA-Z.]", "");
                all_str.Add(new_str.ToLower());
            }
            return all_str;
        }
        public static async Task<List<List<string>>> GetScoreAsync(string FilmName)
        {
            List<float> actingScores_50 = new List<float>();
            List<float> musicScores_50 = new List<float>();
            List<float> plotScores = new List<float>();
            List<float> specScores_50 = new List<float>();
            List<string> scores = new List<string>() { };
            List<string> positive = new List<string>();
            List<string> negative = new List<string>();
            List<string> imdbRatings = new List<string>();
            List<string> alldata_metrics = new List<string>();
            List<string> alldata_comments_negative = new List<string>();
            List<string> alldata_comments_positive = new List<string>();
            float last_score_acting_50 = 0, last_score_music_50 = 0, last_score_plot = 0, last_score_spec_50 = 0;
            List<List<string>> alldata = new List<List<string>>();
            List<string> comments = await Program.ParserExec(FilmName, false);
            string fileName = FilmName + "_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
            string positive_path = path_to_comments + fileName + "_positive.csv";
            string negative_path = path_to_comments + fileName + "_negative.csv";
            string truncate_positive_path = "comments/" + fileName + "_positive.csv";
            string truncate_negative_path = "comments/" + fileName + "_negative.csv";
            if (comments.Last() != null)
            {
                imdbRatings = ImdbParser.getRatingsByFilmId(comments.Last());
            }
            comments.Remove(comments.Last());
            string kinopoiskRating = comments.Last();
            comments.Remove(comments.Last());
            List<string> comments_new = Norm(comments);
            foreach (string comment in comments_new)
            {
                ModelInput input = new ModelInput()
                {
                    Review = comment
                };
                ModelOutput result = ConsumeModel.Predict(input);
                scores.Add(result.Prediction);
                if (result.Prediction == "0")
                {
                    negative.Add(result.Prediction);
                    alldata_comments_negative.Add(comment);
                }
                else
                {
                    alldata_comments_positive.Add(comment);
                    positive.Add(result.Prediction);
                }
            }

            File.WriteAllLines(positive_path, alldata_comments_positive);
            File.WriteAllLines(negative_path, alldata_comments_negative);

            double last_score = 0;
            foreach (string score in scores)
                last_score += Convert.ToInt32(score);

            last_score /= scores.Count;
            last_score *= 10;
            alldata_metrics.Add(Math.Round(last_score, 1).ToString());
            alldata_metrics.Add(scores.Count.ToString());
            alldata_metrics.Add(positive.Count.ToString());
            alldata_metrics.Add(negative.Count.ToString());
            /* string scoresDescription = ExecIvi.Exec(FilmName);
             if (scoresDescription != "")
                 alldata.Add(scoresDescription);*/

            Acting_model_50.Acting_model_50.ModelInput sampleData_acting_50;
            Acting_model_50.Acting_model_50.ModelOutput result_acting_50;
            Music_model_50.Music_model_50.ModelInput sampleData_music_50;
            Music_model_50.Music_model_50.ModelOutput result_music_50;
            Plot_model_new.ModelInput sampleData3;
            Plot_model_new.ModelOutput result3;
            Spec_model_50.Spec_model_50.ModelInput sampleData_spec_50;
            Spec_model_50.Spec_model_50.ModelOutput result_spec_50;

            foreach (string comment in comments_new)
            {
                sampleData_acting_50 = new Acting_model_50.Acting_model_50.ModelInput() { Reviews = comment };
                result_acting_50 = Acting_model_50.Acting_model_50.Predict(sampleData_acting_50);
                actingScores_50.Add(result_acting_50.PredictedLabel);


                sampleData_music_50 = new Music_model_50.Music_model_50.ModelInput() { Reviews = comment };
                result_music_50 = Music_model_50.Music_model_50.Predict(sampleData_music_50);
                musicScores_50.Add(result_music_50.PredictedLabel);


                //plot
                sampleData3 = new Plot_model_new.ModelInput() { Reviews = comment };
                result3 = Plot_model_new.Predict(sampleData3);
                plotScores.Add(result3.PredictedLabel);


                sampleData_spec_50 = new Spec_model_50.Spec_model_50.ModelInput() { Review = comment };
                result_spec_50 = Spec_model_50.Spec_model_50.Predict(sampleData_spec_50);
                specScores_50.Add(result_spec_50.PredictedLabel);
            }
            foreach (float score in plotScores)
                last_score_plot += score;
            last_score_plot /= plotScores.Count;
            last_score_plot *= 2;

            foreach (float score in actingScores_50)
                last_score_acting_50 += score;
            last_score_acting_50 /= actingScores_50.Count;
            last_score_acting_50 *= 2;

            foreach (float score in specScores_50)
                last_score_spec_50 += score;
            last_score_spec_50 /= specScores_50.Count;
            last_score_spec_50 *= 2;

            foreach (float score in musicScores_50)
                last_score_music_50 += score;
            last_score_music_50 /= musicScores_50.Count;
            last_score_music_50 *= 2;

            alldata_metrics.Add(Math.Round(last_score_acting_50, 1).ToString());
            alldata_metrics.Add(Math.Round(last_score_music_50, 1).ToString());
            alldata_metrics.Add(Math.Round(last_score_spec_50, 1).ToString());
            alldata_metrics.Add(Math.Round(last_score_plot, 1).ToString());
            alldata_metrics.Add(truncate_positive_path);
            alldata_metrics.Add(truncate_negative_path);
            alldata_metrics.Add(kinopoiskRating);
            if (imdbRatings.Count == 5)
            {
                alldata_metrics.Add(imdbRatings[0]);
                alldata_metrics.Add(imdbRatings[1]);
                alldata_metrics.Add(imdbRatings[2]);
                alldata_metrics.Add(imdbRatings[3]);
                alldata_metrics.Add(imdbRatings[4]);
            }
            alldata.Add(alldata_metrics);
            return alldata;
        }
    }
}
