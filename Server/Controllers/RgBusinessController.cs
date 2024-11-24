using HtmlAgilityPack;

namespace Server.Controllers
{
    public class RgBusinessController
    {
        private string _url;
        private string _baseurl;

        public RgBusinessController()
        {
            //_prompt = string.Empty;
            _url = "https://rg.ru/tema/ekonomika/business";
            _baseurl = "https://rg.ru";
        }

        private string _GetNewsBody(string href)
        {
            string body = "";

            try
            {
                var response = CallUrl(_baseurl + href);
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.Result);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'PageArticleContent_lead__l9TkG')]");
                body = nodes[0].InnerText;
            }
            catch (Exception ex)
            {
            }
            return body;
        }

        private string _GetNewsTitle(string href)
        {
            string news = "";

            try
            {
                var response = CallUrl(_baseurl + href);
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.Result);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//h1[contains(@class, 'PageArticleCommonTitle_title__fUDQW')]");
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
                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'ItemOfListStandard_wrapper__CkHBZ ItemOfListStandard_compilation__4SnZC ItemOfListStandard_imageRight__BPa4l ')]");

                foreach (var node in nodes)
                {
                    string href = node.ChildNodes[2].Attributes[0].Value;

                    news.Add(Tuple.Create(_GetNewsTitle(href), _GetNewsBody(href)));
                }

            }
            catch (Exception ex)
            {

            }

            return news;
        }
    }
}