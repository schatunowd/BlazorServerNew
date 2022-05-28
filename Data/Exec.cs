
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Acting_modeL_new;
using Music_model_new_ConsoleApp1;
using Plot_model_new_ConsoleApp1;
using Spec_model_new_ConsoleApp1;


namespace BlazorServerNew.Data
{
    public class Exec
    {
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
        public static async Task<List<string>> GetScoreAsync(string FilmName)
        {
            List<float> actingScores = new List<float>();
            List<float> musicScores = new List<float>();
            List<float> plotScores = new List<float>();
            List<float> specScores = new List<float>();
            List<string> scores = new List<string>() { };
            List<string> positive = new List<string>();
            List<string> negative = new List<string>();
            float last_score_acting = 0, last_score_music = 0, last_score_plot = 0, last_score_spec = 0;
            List<string> alldata = new List<string>();
            List<string> comments = await Program.ParserExec(FilmName, false);
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
                    negative.Add(result.Prediction);
                else
                    positive.Add(result.Prediction);
            }

            double last_score = 0;
            foreach (string score in scores)
                last_score += Convert.ToInt32(score);

            last_score /= scores.Count;
            last_score *= 10;
            alldata = new List<string>();
            alldata.Add(Math.Round(last_score, 1).ToString());
            alldata.Add(scores.Count.ToString());
            alldata.Add(positive.Count.ToString());
            alldata.Add(negative.Count.ToString());
           /* string scoresDescription = ExecIvi.Exec(FilmName);
            if (scoresDescription != "")
                alldata.Add(scoresDescription);*/

            foreach (string comment in comments_new)
            {
                var sampleData = new MLModel4.ModelInput()
                {
                    Reviews = comment
                };
                var result = MLModel4.Predict(sampleData);
                actingScores.Add(result.PredictedLabel);

                var sampleData2 = new Music_model_new.ModelInput()
                {
                    Reviews = comment
                };
                var result2 = Music_model_new.Predict(sampleData2);
                musicScores.Add(result2.PredictedLabel);

                var sampleData3 = new Plot_model_new.ModelInput()
                {
                    Reviews = comment
                };
                var result3 = Plot_model_new.Predict(sampleData3);
                plotScores.Add(result3.PredictedLabel);

                var sampleData4 = new Spec_model_new.ModelInput()
                {
                    Review = comment
                };
                var result4 = Spec_model_new.Predict(sampleData4);
                specScores.Add(result4.PredictedLabel);
            }
            foreach (float score in actingScores)
                last_score_acting += score;
            last_score_acting /= actingScores.Count;
            foreach (float score in musicScores)
                last_score_music += score;
            last_score_music /= musicScores.Count;
            foreach (float score in plotScores)
                last_score_plot += score;
            last_score_plot /= plotScores.Count;
            foreach (float score in specScores)
                last_score_spec += score;
            last_score_spec /= specScores.Count;
            alldata.Add(Math.Round(last_score_acting, 1).ToString());
            alldata.Add(Math.Round(last_score_music, 1).ToString());
            alldata.Add(Math.Round(last_score_spec, 1).ToString());
            alldata.Add(Math.Round(last_score_plot, 1).ToString());
            return alldata;
        }
    }
}
