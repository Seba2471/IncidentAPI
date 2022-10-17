namespace IncidentAPI
{
    public class GetWebsiteBody
    {
        public static async Task<string> GetHtmlContextByUrl(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            var pageContents = await response.Content.ReadAsStringAsync();

            return pageContents;
        }

    }
}
