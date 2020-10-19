using System;
using System.Collections.Generic;
using System.Text;

namespace EpEren.Api.tranimeizle.Classes.Main
{
    public class Player
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Embed { get; set; }
        public List<Downloadable> Downloadables = new List<Downloadable>();

    }
}
