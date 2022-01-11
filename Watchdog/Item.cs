using System;

namespace Watchdog
{
    public class Item
    {
        public string Name { get; set; }

        public int Min { get; set; }
        public int Avg { get; set; }
        public int Max { get; set; }
        public int Volume { get; set; }
        public DateTime Date { get; set; }
    }

}
