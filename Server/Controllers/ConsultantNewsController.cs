using HtmlAgilityPack;


namespace Server.Controllers
{
    public class ConsultantNewsController
    {
        //private string _prompt;
        private string _url;
        private int _lastNewsCount;

        public ConsultantNewsController()
        {
            //_prompt = string.Empty;
            _url = "https://www.consultant.ru/legalnews/";
            _lastNewsCount = 2;
        }

        private string _GetNewsBody(int newsID)
        {
            string body = "";

            try
            {
                var response = CallUrl(_url + newsID.ToString());
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.Result);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'news-page__text')]");
                body = nodes[0].InnerText;
            }
            catch (Exception ex)
            {
            }
            return body;
        }

        private string _GetNewsTitle(int newsID)
        {
            string news = "";

            try
            {
                var response = CallUrl(_url + newsID.ToString());
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.Result);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//h1[contains(@class, 'news-page__title')]");
                news = nodes[0].InnerText;
            }
            catch (Exception ex)
            {
            }

            return news;
        }

        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(fullUrl);
            return response;
        }

        public List<Tuple<string, string>> GetNewsInfo()
        {
            List<Tuple<string, string>> news = new List<Tuple<string, string>>();

            var response = CallUrl(_url + "?page=1");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response.Result);

            try
            {
                var nodes = htmlDoc.DocumentNode.SelectNodes("//a[contains(@class, 'listing-news__item-title')]");
                string lastNewsID = nodes[0].Attributes[1].Value;

                lastNewsID = lastNewsID.Split('/')[2];

                for (int newsID = Int32.Parse(lastNewsID); newsID > Int32.Parse(lastNewsID) - _lastNewsCount; newsID--)
                {
                    news.Add(Tuple.Create(_GetNewsTitle(newsID), _GetNewsBody(newsID)));
                }
            }
            catch (Exception ex)
            {

            }

            return news;
        }
    }
}