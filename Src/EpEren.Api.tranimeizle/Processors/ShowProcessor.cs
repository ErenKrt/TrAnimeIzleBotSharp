using AngleSharp;
using AngleSharp.Dom;
using EpEren.Api.tranimeizle.Classes;
using EpEren.Api.tranimeizle.Classes.Main;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using EpEren.Api.tranimeizle.Classes.Response.Main;
using RestSharp.Extensions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

namespace EpEren.Api.tranimeizle.Processors
{
    public class ShowProcessor
    {
        private readonly RestClient _HttpClient;

        public ShowProcessor(RestClient cl)
        {
            this._HttpClient = cl;
        }
        
        async public Task<IResult<Show>> GetShow(string ShowUri)
        {
            IResult<Show> Donut = null;

            var Req = new RestRequest("/anime/"+ShowUri);
            var Res = this._HttpClient.Get(Req);

            if (Res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Donut = Result.Fail<Show>(Res.StatusDescription);
            }
            else
            {
                var Document = await Res.Content.ConvertHTML();
                var Show = new Show();

                var TagInner = Document.QuerySelector(".tags-inner");

                var TagsHTML = TagInner.QuerySelectorAll(".tag");

                foreach (var TagHTML in TagsHTML)
                {
                    Show.Categories.Add(new ShortCategory()
                    {
                        Name = TagHTML.Text(),
                        Uri = TagHTML.Attributes["href"].Value
                    });
                }

                TagInner.ParentElement.PreviousElementSibling.Remove();

                var OtherNamesHTML = TagInner.ParentElement.ParentElement;

                TagInner.ParentElement.Remove();

                OtherNamesHTML.QuerySelectorAll("dd")[0].Remove();

                var NamesHTML = OtherNamesHTML.QuerySelectorAll("dt");

                foreach (var NameHTML in NamesHTML)
                {
                    Show.OtherNames.Add(NameHTML.Text().Trim().Replace(" ","").ClearHtmlTags());
                }

                var Bilgiler = OtherNamesHTML.NextElementSibling.QuerySelector("dl");
                var Bilgiler2 = OtherNamesHTML.NextElementSibling.NextElementSibling.QuerySelector("dl");

               

                Show.EpisodeCounter = Convert.ToInt32(Bilgiler.QuerySelectorAll("dt")[1].Text().Split('/')[0].Trim().Replace(" ", ""));
                Show.StartedDay = Bilgiler.QuerySelectorAll("dt")[2].Text().ClearHtmlTags();
                Show.LastAddedDay= Bilgiler.QuerySelectorAll("dt")[3].Text().ClearHtmlTags(); ;
                Show.MinutePerEpisode = Bilgiler2.QuerySelectorAll("dt")[1].Text().ClearHtmlTags();
                Show.EndedDay = Bilgiler2.QuerySelectorAll("dt")[2].Text().ClearHtmlTags();
                Show.YearOfShow = Bilgiler2.QuerySelectorAll("dt")[3].Text().ClearHtmlTags();

                var TranslartorsHTML = Bilgiler2.ParentElement.NextElementSibling.QuerySelector("dl").QuerySelectorAll("dt>ul>li");

                foreach (var TranslartorHTML in TranslartorsHTML)
                {
                    Show.Translators.Add(TranslartorHTML.Text());
                }

                Show.Image = Document.QuerySelector(".animeDetail-video-player").QuerySelector(".poster").QuerySelector("img").Attributes["src"].Value;

                var IdParserRegex = new Regex(@"\/animes\/(.*?)\/");
                var IdParseTest = IdParserRegex.Match(Show.Image);

                if (IdParseTest.Success)
                {
                    Show.ID = Convert.ToInt32(IdParseTest.Groups[1].Value);
                }

                Show.Name = Document.QuerySelector(".playlist-title").QuerySelector("h1").Text();

                Regex SezonAndBolum = new Regex(@"(.*?) ([\d]+). sezon ([\d]+). bölüm izle");
                Regex OnlyBolum = new Regex(@"(.*?) ([\d]+). bölüm izle");
                Regex Onlyizle = new Regex(@"(.*?) izle");
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                var EpisodesHTML = Document.QuerySelectorAll(".episodeBtn");
                foreach (var EpisodeHTML in EpisodesHTML)
                {
                    var ShortEp = new ShortEpisode();
                    ShortEp.Name = EpisodeHTML.QuerySelector("p>span").Text();
                    ShortEp.EpID = EpisodeHTML.QuerySelector("p>meta").Attributes["content"].Value;

                    EpisodeHTML.QuerySelector("p>small>br").Remove();

                    ShortEp.PublisedDay = EpisodeHTML.QuerySelector("p>small").Text().ClearHtmlTags();
                    ShortEp.Image = EpisodeHTML.QuerySelector(".imgContainer>img").Attributes["src"].Value;
                    ShortEp.Uri = EpisodeHTML.Attributes["data-slug"].Value;

                    var TestSezonAndBolum = SezonAndBolum.Match(ShortEp.Name.ToLower());
                    if (TestSezonAndBolum.Success)
                    {
                        ShortEp.ShowName = ti.ToTitleCase(TestSezonAndBolum.Groups[1].Value);
                        ShortEp.SeasonID = TestSezonAndBolum.Groups[2].Value;
                    }
                    else
                    {
                        var TestBolum = OnlyBolum.Match(ShortEp.Name.ToLower());

                        if (TestBolum.Success)
                        {
                            ShortEp.ShowName = ti.ToTitleCase(TestBolum.Groups[1].Value);
                        }
                        else
                        {
                            var TestIzle = Onlyizle.Match(ShortEp.Name.ToLower());
                            if (TestIzle.Success)
                            {
                                ShortEp.ShowName = ti.ToTitleCase(TestIzle.Groups[1].Value);
                            }
                            else
                            {
                                ShortEp.ShowName = ShortEp.Name;
                            }
                        }
                    }

                    Show.Episodes.Add(ShortEp);
                }
                Show.Description = Document.QuerySelector("p[itemprop=description]").Text();

                Donut = Result.Success(Show);
            }

            return Donut;
        }

        async public Task<IResult<Episode>> GetEpisode(string Ep,string Fansub="")
        {
            IResult<Episode> Donut = null;

            
            var Req = new RestRequest("/"+Ep);
            var Res = this._HttpClient.Get(Req);

            if (Res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Donut = Result.Fail<Episode>(Res.StatusDescription);
            }
            else
            {
                var Episode = new Episode();

                var Document = await Res.Content.ConvertHTML();

                var EpID = Document.QuerySelector("#EpisodeId").Attributes["value"].Value;
                
                var FanSubsHTML = Document.QuerySelectorAll(".fansubSelector");

                foreach (var FanSubHTML in FanSubsHTML)
                {
                    Episode.FanSubs.Add(new Fansub()
                    {
                        ID = FanSubHTML.Attributes["data-fid"].Value,
                        Name = FanSubHTML.Text().Trim().ClearHtmlTags()
                    });
                }


                Fansub SelectedFanSub = null;
                if (Fansub == "")
                {
                    SelectedFanSub = Episode.FanSubs[0];
                }
                else
                {
                    var Bul = Episode.FanSubs.Find(x => x.ID == Fansub);
                    if (Bul != null)
                    {
                        SelectedFanSub = Bul;
                    }
                    else
                    {
                        SelectedFanSub = Episode.FanSubs[0];
                    }
                }


                var GetPlayersReq = new RestRequest("/api/fansubSources");
                GetPlayersReq.RequestFormat = DataFormat.Json;
                GetPlayersReq.AddJsonBody(new { EpisodeId = EpID, FansubId = SelectedFanSub.ID });

                var GetPlayersRes = this._HttpClient.Post(GetPlayersReq);
                var GetPlayersHTML = await GetPlayersRes.Content.ConvertHTML();

                var PlayersHTML = GetPlayersHTML.QuerySelectorAll(".sourceBtn");

                foreach (var PlayerHTML in PlayersHTML)
                {
                    var Player = new Player();
                    Player.ID = PlayerHTML.Attributes["data-id"].Value;

                    Player.Description = PlayerHTML.QuerySelector("p>small").Text().Trim().ClearHtmlTags();
                    PlayerHTML.QuerySelector("p>small").Remove();

                    Player.Name = PlayerHTML.QuerySelector("p").Text().Trim().ClearHtmlTags();


                    var GetPlayerSourceReq = new RestRequest("api/sourcePlayer/"+Player.ID);
                    var GetPlayerSourcesRes = this._HttpClient.Post(GetPlayerSourceReq);
                    var GetPlayerSourcesHTML = await JObject.Parse(GetPlayerSourcesRes.Content)["source"].ToString().ConvertHTML();

                    var Iframe = GetPlayerSourcesHTML.QuerySelector("iframe");

                    if (Iframe != null)
                    {
                        Player.Embed = Iframe.Attributes["src"].Value;
                    }
                    else
                    {
                        var A = GetPlayerSourcesHTML.QuerySelector("a");
                        if (A != null)
                        {
                            Player.Embed = A.Attributes["href"].Value;
                        }
                    }

                    if (Player.Embed != "")
                    {
                      
                        var KucultName = Player.Name.ToLower();
                        if (KucultName == "gplus" || KucultName=="yandex" || KucultName=="mp4upload" || KucultName=="vidmoly" || KucultName=="videobin" || KucultName=="vidia")
                        {
                            var GetDownlandAbleClient = new RestClient("https://player.tranimeizle.com/");
                            var GetDownlandAble = new RestRequest(Regex.Split(Player.Embed, "https://player.tranimeizle.com/")[1].ToString());
                            var GetDownloadableRes = GetDownlandAbleClient.Get(GetDownlandAble);

                            if (GetDownloadableRes.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var GetDownloadableContent = GetDownloadableRes.Content.ClearHtmlTags();

                                if(KucultName == "vidmoly")
                                {
                                    var IframeRegex = new Regex("<iframe src=\"(.*?)\".*></iframe>");
                                    var Parse = IframeRegex.Match(GetDownloadableContent);
                                    var Url = Parse.Groups[1].Value;

                                    var MolyClient = new RestClient("https://vidmoly.me/");
                                    var MolyReq = new RestRequest(Regex.Split(Url, "https://vidmoly.me/")[1].ToString());
                                    var MolyRes= MolyClient.Get(MolyReq);

                                    if (MolyRes.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        var MolyContent = MolyRes.Content.ClearHtmlTags();

                                        var SourcesRegex = new Regex(@"sources: (\[.*?\])");
                                        var ParseSources = SourcesRegex.Match(MolyContent);
                                        var Json = JsonConvert.DeserializeObject<List<Downloadable>>((ParseSources.Groups[1].Value).Replace("\t", "").Replace("\\", "").Trim());
                                        if (Json != null)
                                        {
                                            Player.Downloadables = Json;
                                        }
                                    }
                                }
                                else
                                {
                                    var SourcesRegex = new Regex(@"var sources = (\[.*?\])");
                                    var Parse = SourcesRegex.Match(GetDownloadableContent);
                                    var Json = JsonConvert.DeserializeObject<List<Downloadable>>((Parse.Groups[1].Value).Replace("\t", "").Replace("\\", "").Trim());
                                    if (Json != null)
                                    {
                                        Player.Downloadables = Json;
                                    }
                                }

                               

                            }

                        }else if (KucultName == "sibnet")
                        {
                            var GetDownlandAbleClient = new RestClient("https://video.sibnet.ru/");
                            GetDownlandAbleClient.FollowRedirects = false;
                            var GetDownlandAble = new RestRequest(Regex.Split(Player.Embed, "https://video.sibnet.ru/")[1].ToString());
                            var GetDownloadableRes = GetDownlandAbleClient.Get(GetDownlandAble);
                            if (GetDownloadableRes.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var GetDownloadableContent = GetDownloadableRes.Content.ClearHtmlTags();

                                var SourceRegex = new Regex(@"player.src\(\[{src: (.*?),.*,\]\)");
                                var Parse = SourceRegex.Match(GetDownloadableContent.ClearHtmlTags());

                                var PlayerRedict = Parse.Groups[1].Value.Replace("\"", "");

                                var GetMP4Req = new RestRequest(PlayerRedict);
                                GetMP4Req.AddHeader("referer", Player.Embed);
                                
                                var GetMP4Res = GetDownlandAbleClient.Get(GetMP4Req);

                                if (GetMP4Res.StatusCode == System.Net.HttpStatusCode.Redirect)
                                {
                                    Player.Downloadables.Add(new Downloadable()
                                    {
                                        Name = "default",
                                        Url = GetMP4Res.Headers.First(x => x.Name == "Location").Value.ToString()
                                    });
                                }
                            }
                        }
                    }

                    var OncekiHTML = Document.QuerySelector(".btn.btn-news.pull-left");
                    var SonrakiHTML = Document.QuerySelector(".btn.btn-news.pull-right");

                    if (OncekiHTML.Attributes["disabled"] == null)
                    {
                        Episode.Prev = OncekiHTML.Attributes["href"].Value;
                    }
                    if (SonrakiHTML.Attributes["disabled"] == null)
                    {
                        Episode.Next = SonrakiHTML.Attributes["href"].Value;
                    }


                    SelectedFanSub.Players.Add(Player);
                }

                Donut = Result.Success<Episode>(Episode);

            }


            return Donut;
        }

        async public Task<IResult<Episode>> PrepareDownloadEpisode(string Ep)
        {
            IResult<Episode> Donut = null;

            var Req = new RestRequest("/" + Ep);
            var Res = this._HttpClient.Get(Req);

            if (Res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Donut = Result.Fail<Episode>(Res.StatusDescription);
            }
            else
            {
                var Episode = new Episode();
                var Document = await Res.Content.ConvertHTML();
                var EpID = Document.QuerySelector("#EpisodeId").Attributes["value"].Value;

                var FanSubsHTML = Document.QuerySelectorAll(".fansubSelector");

                Parallel.ForEach(FanSubsHTML, new ParallelOptions { MaxDegreeOfParallelism = 5 }, async FanSubHTML => {
                    var Fansub = new Fansub();
                    Fansub.ID= FanSubHTML.Attributes["data-fid"].Value;
                    Fansub.Name = FanSubHTML.Text().Trim().ClearHtmlTags();

                    var GetPlayersReq = new RestRequest("/api/fansubSources");
                    GetPlayersReq.RequestFormat = DataFormat.Json;
                    GetPlayersReq.AddJsonBody(new { EpisodeId = EpID, FansubId = Fansub.ID });

                    var GetPlayersRes = this._HttpClient.Post(GetPlayersReq);
                    var GetPlayersHTML = await GetPlayersRes.Content.ConvertHTML();

                    var PlayersHTML = GetPlayersHTML.QuerySelectorAll(".sourceBtn");

                    Parallel.ForEach(PlayersHTML, new ParallelOptions { MaxDegreeOfParallelism = 5 },async PlayerHTML => {
                        var Player = new Player();

                        Player.ID = PlayerHTML.Attributes["data-id"].Value;
                        Player.Description = PlayerHTML.QuerySelector("p>small").Text().Trim().ClearHtmlTags();
                        PlayerHTML.QuerySelector("p>small").Remove();
                        Player.Name = PlayerHTML.QuerySelector("p").Text().Trim().ClearHtmlTags();

                        var GetPlayerSourceReq = new RestRequest("api/sourcePlayer/" + Player.ID);
                        var GetPlayerSourcesRes = this._HttpClient.Post(GetPlayerSourceReq);
                        var GetPlayerSourcesHTML = await JObject.Parse(GetPlayerSourcesRes.Content)["source"].ToString().ConvertHTML();

                        var Iframe = GetPlayerSourcesHTML.QuerySelector("iframe");

                        if (Iframe != null)
                        {
                            Player.Embed = Iframe.Attributes["src"].Value;
                        }
                        else
                        {
                            var A = GetPlayerSourcesHTML.QuerySelector("a");
                            if (A != null)
                            {
                                Player.Embed = A.Attributes["href"].Value;
                            }
                        }

                        if (Player.Embed != "")
                        {

                            var KucultName = Player.Name.ToLower();
                            if (KucultName == "gplus" || KucultName == "yandex" || KucultName == "mp4upload" || KucultName == "vidmoly" || KucultName == "videobin" || KucultName == "vidia")
                            {
                                var GetDownlandAbleClient = new RestClient("https://player.tranimeizle.com/");
                                var GetDownlandAble = new RestRequest(Regex.Split(Player.Embed, "https://player.tranimeizle.com/")[1].ToString());
                                GetDownlandAble.AddHeader("referer",Res.ResponseUri.ToString());
                                var GetDownloadableRes = GetDownlandAbleClient.Get(GetDownlandAble);

                                if (GetDownloadableRes.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    var GetDownloadableContent = GetDownloadableRes.Content.ClearHtmlTags();

                                    if (KucultName == "vidmoly")
                                    {
                                        var IframeRegex = new Regex("<iframe src=\"(.*?)\".*></iframe>");
                                        var Parse = IframeRegex.Match(GetDownloadableContent);
                                        var Url = Parse.Groups[1].Value;

                                        var MolyClient = new RestClient("https://vidmoly.me/");
                                        var MolyReq = new RestRequest(Regex.Split(Url, "https://vidmoly.me/")[1].ToString());
                                        var MolyRes = MolyClient.Get(MolyReq);

                                        if (MolyRes.StatusCode == System.Net.HttpStatusCode.OK)
                                        {
                                            var MolyContent = MolyRes.Content.ClearHtmlTags();

                                            var SourcesRegex = new Regex(@"sources: (\[.*?\])");
                                            var ParseSources = SourcesRegex.Match(MolyContent);
                                            var Json = JsonConvert.DeserializeObject<List<Downloadable>>((ParseSources.Groups[1].Value).Replace("\t", "").Replace("\\", "").Trim());
                                            if (Json != null)
                                            {
                                                Player.Downloadables = Json;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var SourcesRegex = new Regex(@"var sources = (\[.*?\])");
                                        var Parse = SourcesRegex.Match(GetDownloadableContent);
                                        var Json = JsonConvert.DeserializeObject<List<Downloadable>>((Parse.Groups[1].Value).Replace("\t", "").Replace("\\", "").Trim());
                                        if (Json != null)
                                        {
                                            Player.Downloadables = Json;
                                        }
                                    }



                                }

                            }
                            else if (KucultName == "sibnet")
                            {
                                var GetDownlandAbleClient = new RestClient("https://video.sibnet.ru/");
                                GetDownlandAbleClient.FollowRedirects = false;
                                var GetDownlandAble = new RestRequest(Regex.Split(Player.Embed, "https://video.sibnet.ru/")[1].ToString());
                                var GetDownloadableRes = GetDownlandAbleClient.Get(GetDownlandAble);
                                if (GetDownloadableRes.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    var GetDownloadableContent = GetDownloadableRes.Content.ClearHtmlTags();

                                    var SourceRegex = new Regex(@"player.src\(\[{src: (.*?),.*,\]\)");
                                    var Parse = SourceRegex.Match(GetDownloadableContent.ClearHtmlTags());

                                    var PlayerRedict = Parse.Groups[1].Value.Replace("\"", "");

                                    var GetMP4Req = new RestRequest(PlayerRedict);
                                    GetMP4Req.AddHeader("referer", Player.Embed);

                                    var GetMP4Res = GetDownlandAbleClient.Get(GetMP4Req);

                                    if (GetMP4Res.StatusCode == System.Net.HttpStatusCode.Redirect)
                                    {
                                        Player.Downloadables.Add(new Downloadable()
                                        {
                                            Name = "default",
                                            Url = GetMP4Res.Headers.First(x => x.Name == "Location").Value.ToString()
                                        });
                                    }
                                }
                            }

                        }
                        Fansub.Players.Add(Player);
                    });
                    Episode.FanSubs.Add(Fansub);
                });

                Donut = Result.Success<Episode>(Episode);
            }


            return Donut;
        }
    
        async public Task<IResult<ShowDownload>> PrepareDownloadShow(string ShowUri)
        {
            IResult<ShowDownload> Donut = null;
            var Req = new RestRequest("/anime/" + ShowUri);
            var Res = this._HttpClient.Get(Req);

            if (Res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Donut = Result.Fail<ShowDownload>(Res.StatusDescription);
            }
            else
            {
                var ShowDownload = new ShowDownload();

                var Document = await Res.Content.ConvertHTML();

                var EpisodesHTML = Document.QuerySelectorAll(".episodeBtn");

                Parallel.ForEach(EpisodesHTML, new ParallelOptions { MaxDegreeOfParallelism = 5 }, async EpisodeHTML => {
                    var Uri = EpisodeHTML.Attributes["data-slug"].Value;

                    ShowDownload.Episodes.Add((await this.PrepareDownloadEpisode(Uri)).Value);

                });

                Donut = Result.Success<ShowDownload>(ShowDownload);
            }


            return Donut;
        }
    }
}
