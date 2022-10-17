using AngleSharp;
using AngleSharp.Dom;

namespace IncidentAPI
{
    public class GetHtmlNodesByTag
    {
        static public async Task<List<IElement>> FromHtmlText(string htmlText, string tag)
        {
            //Create a new context for evaluating webpages with the default config
            IBrowsingContext context = BrowsingContext.New(Configuration.Default);

            //Create a document from a virtual request / response pattern
            IDocument document = await context.OpenAsync(req => req.Content(htmlText));

            return document.All.Where(m => m.LocalName == tag).ToList();
        }

    }
}
