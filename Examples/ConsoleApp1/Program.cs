using System;
using System.Collections.Generic;
using EpEren.Api.tranimeizle;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Program.Go();
        }
        public static async void Go()
        {
            var Cookies = new List<EpEren.Api.tranimeizle.Classes.Cookie>();

            Cookies.Add(new EpEren.Api.tranimeizle.Classes.Cookie()
            {
                Name = "__cfduid",
                Value = "{YOUR COOKIE}"
            });
            Cookies.Add(new EpEren.Api.tranimeizle.Classes.Cookie()
            {
                Name = "__cf_bm",
                Value = "{YOUR COOKIE}"
            });
            Cookies.Add(new EpEren.Api.tranimeizle.Classes.Cookie()
            {
                Name = "cf_clearance",
                Value = "{YOUR COOKIE}"
            });
            Cookies.Add(new EpEren.Api.tranimeizle.Classes.Cookie()
            {
                Name = ".AitrWeb.Verification.",
                Value = "{YOUR COOKIE}"
            });
            Cookies.Add(new EpEren.Api.tranimeizle.Classes.Cookie()
            {
                Name = ".AitrWeb.Session",
                Value = "{YOUR COOKIE}"
            });
            Cookies.Add(new EpEren.Api.tranimeizle.Classes.Cookie()
            {
                Name = "__cfduid",
                Value = "{YOUR COOKIE}"
            });

            var TrAnime = new TrAnimeIzle();
            TrAnime.SessionHelper.SetCookies(Cookies);
            TrAnime.SessionHelper.SetUserAgent("{YOUR USERAGENT}");

            var AreWeOkay = TrAnime.SessionHelper.TestConnection();

            if (AreWeOkay)
            {
                //var zaa= await TrAnime.MainProcessor.GetNewEpisodes();
                //var zaa2 = await TrAnime.MainProcessor.GetNewShows();
                //var zaa3 = await TrAnime.MainProcessor.GetPopularShows();
                //var zaa4 = await TrAnime.MainProcessor.GetLastComments();
                //var zaa5 = await TrAnime.ShowProcessor.GetShow("tokyo-ghoul-izle-hd");
                //var zaa6 = await TrAnime.ShowProcessor.GetEpisode("boku-no-hero-academia-all-might-rising-the-animation-izle");
                //var zaa7 = await TrAnime.ShowProcessor.PrepareDownloadEpisode("boku-no-hero-academia-all-might-rising-the-animation-izle");
                //var zaa8 = await TrAnime.ShowProcessor.PrepareDownloadShow("tokyo-ghoul-izle-hd");
                //var zaa9 = await TrAnime.MainProcessor.GetSearch("1");
            }
        }
    }
}
