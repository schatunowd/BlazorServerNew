using Microsoft.ML.Data;


namespace BlazorServerNew.Data
{
    public class ModelInput
    {
        [ColumnName("review"), LoadColumn(0)]
        public string Review { get; set; }


        [ColumnName("rating"), LoadColumn(1)]
        public string Rating { get; set; }


    }
}

