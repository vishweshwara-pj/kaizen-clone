// Entities/KeywordRequest.cs
namespace MyWebApi.Entities
{
    public class KeywordRequest
    {
        public string Keyword { get; set; }
    }

    public class SearchResult
    {
        public string ThemeTitle { get; set; }
        public string KaizenWorkArea { get; set; }
        public string TypeOfKaizen { get; set; }
        public string BeforeKaizen { get; set; }
        public string IdealSituation { get; set; }
        public string CurrentSituation { get; set; }
    }
}
