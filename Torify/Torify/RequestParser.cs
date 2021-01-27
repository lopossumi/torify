using System.Collections.Generic;
using System.Linq;

namespace Torify
{
    public class RequestParser
    {
        public List<SalesItem> ParseItems(string html)
        {
            var s1 = "<a id=\"item";
            var s2 = "<div id=\"ribbon\">";

            html = s1+GetBetween(html, s1, s2);
            
            var rawItems = new List<string>();
            while (true)
            {
                var seek = html.Substring(1).IndexOf(s1) + 1;
                if (seek == 0) break;
                rawItems.Add(html.Substring(0, seek));
                html = html.Substring(seek);
            }

            return rawItems.Select(ParseItem).ToList();
        }

        private SalesItem ParseItem(string rawItem)
        {
            var item = new SalesItem();
            item.Description = GetBetween(rawItem, "class=\"li-title\">", "</div>");
            item.Price = GetPrice(GetBetween(rawItem, "\"list_price ineuros\">", "&euro;"));
            item.LinkUri = GetBetween(rawItem, "href=\"", "\"");
            item.Date = GetBetween(rawItem, "date_image\">", "</div>")
                    .Trim()
                    .Replace("\r", "")
                    .Replace("\n", "")
                    .Replace("\t", "");
            return item;
        }

        private string GetBetween(string s, string pattern1, string pattern2)
        {
            try
            {
                var s1 = s.Substring(s.IndexOf(pattern1) + pattern1.Length);
                return s1.Remove(s1.IndexOf(pattern2));
            }
            catch
            {
                return "";
            }
        }

        private static int GetPrice(string input)
        {
            return int.TryParse(input.Replace(" ", "").Trim(), out var price)
                ? price
                : 0;
        }
    }
}