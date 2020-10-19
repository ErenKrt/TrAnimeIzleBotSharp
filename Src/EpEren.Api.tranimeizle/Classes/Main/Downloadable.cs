using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EpEren.Api.tranimeizle.Classes.Main
{
    public class Downloadable
    {
        [JsonProperty("label")]
        public string Name { get; set; }
        [JsonProperty("file")]
        public string Url { get; set; }
    }
}
