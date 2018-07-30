using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFLDataScrapper.Console
{
    public class Player
    {
        public string Name { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string DateOfBirth { get; set; }

        public int? GamePlayedLastYear { get; set; }
        public int GamesPlayedCareer { get; set; }
    }
}
