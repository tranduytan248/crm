namespace Modules.API.Models
{
    public class APIBaseSearchModel
    {
        public string SearchContent { get; set; } = string.Empty;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchNewsModel: APIBaseSearchModel
    {
        public int NewsCategoriesId { get; set; }
    }
}