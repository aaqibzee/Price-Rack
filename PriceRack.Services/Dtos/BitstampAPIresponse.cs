using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceRack.Services.Dtos
{
    public class Data
    {
        public List<Ohlc> ohlc { get; set; }
        public string pair { get; set; }
    }

    public class Ohlc
    {
        public string close { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string open { get; set; }
        public string timestamp { get; set; }
        public string volume { get; set; }
    }

    public class BitstampAPIresponse
    {
        public Data data { get; set; }
    }
}
