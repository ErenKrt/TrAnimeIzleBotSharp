using System;
using System.Collections.Generic;
using System.Text;

namespace EpEren.Api.tranimeizle.Classes.Main
{
    public class Show
    {
        public string Image { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }

        public List<string> OtherNames = new List<string>();

        public List<ShortCategory> Categories = new List<ShortCategory>();
        public int EpisodeCounter { get; set; }
        public string MinutePerEpisode { get; set; }
        public string StartedDay { get; set; }
        public string EndedDay { get; set; }
        public string LastAddedDay { get; set; }
        public string YearOfShow { get; set; }

        public List<string> Translators = new List<string>();

        public List<ShortEpisode> Episodes = new List<ShortEpisode>();
        public string Description { get; set; }
    }
}
