using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NFLDataScrapper.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var letters_to_scrape = new[]
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
                "V", "W", "X", "Y", "Z"
            };
            //var request = WebRequest.Create("http://www.nfl.com/player/tombrady/2504211/profile");
            //((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";
            //var response = request.GetResponse();
            //var strm = response.GetResponseStream();
            //var reader = new StreamReader(strm);
            //var htm = reader.ReadToEnd();
            //System.Console.Write(htm);
            var m = new Scraper(new List<string>{"A"});
            await m.ScrapeSite();
            System.Console.Read();
        }
    }
}
