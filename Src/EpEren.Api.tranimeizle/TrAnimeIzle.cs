using EpEren.Api.tranimeizle.Helpers;
using EpEren.Api.tranimeizle.Processors;
using RestSharp;
using System;

namespace EpEren.Api.tranimeizle
{
    public class TrAnimeIzle
    {
        private RestClient _HttpClient = new RestClient("https://www.tranimeizle.net");
        public SessionHelper SessionHelper { get; set; }
        public MainProcessor MainProcessor { get; set; }
        public ShowProcessor ShowProcessor { get; set; }
        public TrAnimeIzle()
        {
            SessionHelper = new SessionHelper(this._HttpClient);
            MainProcessor = new MainProcessor(this._HttpClient);
            ShowProcessor = new ShowProcessor(this._HttpClient);
        }
    }
}
