using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace NFLDataScrapper.Console
{
    class Scraper
    {
        private readonly List<string> _lettersToScrpe;
        private readonly bool _clearOldData;
        private readonly int _firstPlayerId;
        private const string BASE_URL = @"https://www.pro-football-reference.com";
        const string PLAYER_LIST_URL = @"https://www.pro-football-reference.com/players/";
        const string PLAYER_PROFILE_URL = @"https://www.pro-football-reference.com/players/{0}/{1}";
        const string PLAYER_GAMELOG_URL = @"https://www.pro-football-reference.com/players/{0}/{1}/gamelog/{2}";

        public Scraper(List<string> lettersToScrpe, bool clearOldData = false, int firstPlayerId = 1)
        {
            _lettersToScrpe = lettersToScrpe;
            _clearOldData = clearOldData;
            _firstPlayerId = firstPlayerId;
        }

        public async Task ScrapeSite()
        {
            var players = new List<Player>();
            var playerSraper = new PlayerScraper();
            
            foreach (var letter in _lettersToScrpe)
            {
                // Ln 64
                var playerLinks = await GetPlayersForLetter(letter);
                // Ln 66\
                playerLinks.ForEach(async q =>
                {
                    var player = await playerSraper.GetPlayerProfile(q);
                    players.Add(player);
                });
                
            }
        }

        protected async Task<List<string>> GetPlayersForLetter(string letter)
        {
            // Ln 149
            var response = await GetPage($"{PLAYER_LIST_URL}{letter}");
            var htmlDock = new HtmlDocument();
            htmlDock.LoadHtml(response);
            
            var htmlPlayers = htmlDock.DocumentNode.SelectSingleNode("//div[@id='div_players']");
            var playerLink = htmlPlayers.ChildNodes.Where(t => t.Name.Equals("p", StringComparison.CurrentCultureIgnoreCase))
                .Where(t => t.FirstChild.Name.Equals("a", StringComparison.CurrentCultureIgnoreCase))
                .Select(t=>t.FirstChild).Select(t=>t.GetAttributeValue("href", "not-found")).ToList();
            var playersWithBtag = htmlPlayers.ChildNodes
                .Where(t => t.Name.Equals("p", StringComparison.CurrentCultureIgnoreCase))
                .Where(t => t.FirstChild.Name.Equals("b", StringComparison.CurrentCultureIgnoreCase))
                .Select(t => t.FirstChild).Where(t => t.FirstChild.Name.Equals("a", StringComparison.CurrentCultureIgnoreCase))
                .Select(t => t.FirstChild).Select(t => t.GetAttributeValue("href", "not-found")).ToList();
            playerLink.AddRange(playersWithBtag);
            var distinct = playerLink.Distinct().Select(q=> $"{BASE_URL}{q}").ToList();
            return distinct;
           
        }

        protected async Task<string> GetPage(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var getResponse = await httpClient.GetAsync(new Uri(url));
                if (getResponse.IsSuccessStatusCode)
                {
                    using (var content = getResponse.Content)
                    {
                        var result = await content.ReadAsStringAsync();
                        return result;
                    }
                }
            }

            return string.Empty;
        }
    }
}
