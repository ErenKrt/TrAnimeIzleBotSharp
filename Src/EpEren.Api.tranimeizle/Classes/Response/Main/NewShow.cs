using EpEren.Api.tranimeizle.Classes.Main;
using System;
using System.Collections.Generic;
using System.Text;

namespace EpEren.Api.tranimeizle.Classes.Response.Main
{
    public class NewShow
    {
        public List<ShortShow> Shows = new List<ShortShow>();
        public Pagination Pagination = new Pagination();
    }
}
