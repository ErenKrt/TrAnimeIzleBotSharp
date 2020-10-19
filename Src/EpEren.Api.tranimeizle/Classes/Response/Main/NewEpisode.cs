using EpEren.Api.tranimeizle.Classes.Main;
using System;
using System.Collections.Generic;
using System.Text;

namespace EpEren.Api.tranimeizle.Classes.Response.Main
{
    public class NewEpisode
    {
        public List<ShortEpisode> Episodes = new List<ShortEpisode>();
        public Pagination Pagination = new Pagination();
    }
}
