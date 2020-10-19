using System;
using System.Collections.Generic;
using System.Text;

namespace EpEren.Api.tranimeizle.Classes.Main
{
    public class Fansub
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public List<Player> Players = new List<Player>();

    }
}
