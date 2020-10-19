using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EpEren.Api.tranimeizle.Classes
{
    public static class Extensions
    {
        public async static Task<IElement> ConvertHTML(this String html)
        {
            var Context = BrowsingContext.New(Configuration.Default);
            return (await Context.OpenAsync(req => req.Content(html))).DocumentElement;
        }
        public static string ClearHtmlTags(this string text)
        {
            return text.Replace("\n", "").Replace("\r", "");
        }
    }
}
