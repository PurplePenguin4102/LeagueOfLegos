using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegos
{

    public class FullConvertedChampData
    {
        public string type { get; set; }
        public string version { get; set; }
        public Dictionary<string, ChampData> data { get; set; }
    }

    public class ImageData
    {
        public string full { get; set; }
        public string sprite { get; set; }
        public string group { get; set; }
        public int h { get; set; }
        public int w { get; set; }
        public int y { get; set; }
        public int x { get; set; }
    }

    public class ChampData
    {
        public int id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public ImageData image { get; set; }
    }
}
