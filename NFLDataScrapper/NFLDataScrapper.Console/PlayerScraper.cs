﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace NFLDataScrapper.Console
{
    class PlayerScraper
    {
        
        
        public async Task<Player> GetPlayerProfile(string url)
        {
            return await ScrapPlayerProfile(url);
        }
        protected async Task<string> GetPlayerProfileHtml(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var playerResponse = await httpClient.GetAsync(url);
                if (playerResponse.IsSuccessStatusCode)
                {
                    using (var content = playerResponse.Content)
                    {
                        var result = await content.ReadAsStringAsync();
                        return result;
                    }
                }
            }

            return string.Empty;
        }

        protected async Task<Player> ScrapPlayerProfile(string url)
        {
            try
            {
                var playerProfilehtml = await GetPlayerProfileHtml(url);
                var playerProfileHtmlDoc = new HtmlDocument();
                playerProfileHtmlDoc.LoadHtml(playerProfilehtml);
                var physicalProfileNodeHtml = playerProfileHtmlDoc.DocumentNode.SelectSingleNode("//div[@id='meta']").InnerHtml;
                var ppHtmlDocumet = new HtmlDocument();
                ppHtmlDocumet.LoadHtml(physicalProfileNodeHtml);
                var physicalProfileNode =
                    ppHtmlDocumet.DocumentNode.SelectSingleNode("//div[@itemtype='https://schema.org/Person']");
                System.Console.WriteLine(url);
                var actualName = physicalProfileNode.ChildNodes
                    .FirstOrDefault(t => t.Name.Equals("h1", StringComparison.CurrentCultureIgnoreCase))
                    .InnerHtml;
                System.Console.WriteLine(actualName);

                var allOtherAttributes = physicalProfileNode.ChildNodes
                    .Where(t => t.Name.Equals("p", StringComparison.CurrentCultureIgnoreCase)).Select(t => t.ChildNodes).ToList();
                var player = new Player {Name = actualName};
                var r = physicalProfileNode.InnerHtml;
                var otherProfileStatsHtmlDoc = new HtmlDocument();
                otherProfileStatsHtmlDoc.LoadHtml(r);
                var heightNode =
                    otherProfileStatsHtmlDoc.DocumentNode.SelectSingleNode("//p//span[@itemprop='height']");

                if (heightNode != null)
                {
                    player.Height = heightNode.InnerHtml;
                }

                var weightNode =
                    otherProfileStatsHtmlDoc.DocumentNode.SelectSingleNode("//p//span[@itemprop='weight']");
                if (weightNode != null)
                {
                    player.Weight = weightNode.InnerHtml;
                }
                
                var dobNode = otherProfileStatsHtmlDoc.DocumentNode.SelectSingleNode("//p//span[@itemprop='birthDate']");
                if (dobNode != null)
                {
                    player.DateOfBirth = dobNode.GetAttributeValue("data-birth", "notfound");
                }

                // Game Stats
                var gameStatsNode = playerProfileHtmlDoc.DocumentNode.SelectSingleNode("//div[@class='stats_pullout']");
                if (gameStatsNode != null)
                {

                }

                return player;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
            
        }
    }
}
