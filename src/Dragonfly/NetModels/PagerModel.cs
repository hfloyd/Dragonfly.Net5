namespace Dragonfly.NetModels
{
    using System.Globalization;

    public class PagerModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PagerBaseUrl">Base Url without 'page' query string key</param>
        /// <param name="ItemsPerPage">Total number of items on each page</param>
        /// <param name="ThisPageNum">Current active page</param>
        /// <param name="TotalNumPages">Total number of pages</param>
        /// <param name="PagerQueryStringKey">Keyword used to represent page number in query string (Default="p")</param>
        public PagerModel(string PagerBaseUrl, int ItemsPerPage, int ThisPageNum, int TotalNumPages, string PagerQueryStringKey = "p")
        {
            PageSize = ItemsPerPage;
            CurrentPageNum = ThisPageNum;
            TotalPages = TotalNumPages;
            QueryStringKey = PagerQueryStringKey;

            if (PagerBaseUrl.Contains("?")&& !PagerBaseUrl.EndsWith("?"))
            {
                //Contained somewhere in the middle, so there is existing query string data, append page info
                BaseUrlWithQS = string.Format("{0}{1}=", PagerBaseUrl.EnsureEndsWith('&'), PagerQueryStringKey);
            }
            else
            {
                BaseUrlWithQS = string.Format("{0}{1}=", PagerBaseUrl.EnsureEndsWith('?'), PagerQueryStringKey);
            }

            FirstUrl = PagerBaseUrl;
            LastUrl = string.Format("{0}{1}", BaseUrlWithQS, TotalPages);

            var nextPageUrl = TotalPages > ThisPageNum ? string.Format("{0}{1}", BaseUrlWithQS, (ThisPageNum + 1)) : null;
            var prevPageUrl = ThisPageNum > 2 ? string.Format("{0}{1}", BaseUrlWithQS, (ThisPageNum - 1)) : ThisPageNum > 1 ? FirstUrl : null;

            NextUrl = nextPageUrl;
            PreviousUrl = prevPageUrl;
        }

        public int PageSize { get; private set; }

        public int TotalPages { get; private set; }

        public int CurrentPageNum { get; private set; }

        public string NextUrl { get; private set; }

        public string PreviousUrl { get; private set; }

        public string FirstUrl { get; private set; }

        public string QueryStringKey { get; private set; }

        public string BaseUrlWithQS { get; private set; }

        public string LastUrl { get; private set; }

        public bool HasNext
        {
            get { return NextUrl != null; }
        }

        public bool HasPrevious
        {
            get { return PreviousUrl != null; }
        }
    }

    internal static class StringExtensions
    {
        public static string EnsureStartsWith(this string input, char value)
        {
            return input.StartsWith(value.ToString(CultureInfo.InvariantCulture)) ? input : value + input;
        }

        public static string EnsureEndsWith(this string input, char value)
        {
            return input.EndsWith(value.ToString(CultureInfo.InvariantCulture)) ? input : input + value;
        }

        public static string EnsureEndsWith(this string input, string toEndWith)
        {
            return input.EndsWith(toEndWith.ToString(CultureInfo.InvariantCulture)) ? input : input + toEndWith;
        }
    }
}