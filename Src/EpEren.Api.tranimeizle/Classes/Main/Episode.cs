using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace EpEren.Api.tranimeizle.Classes.Main
{
    public class Episode
    {
        public List<Fansub> FanSubs = new List<Fansub>();
        public string Next { get; set; }
        public string Prev { get; set; }
    }
}
