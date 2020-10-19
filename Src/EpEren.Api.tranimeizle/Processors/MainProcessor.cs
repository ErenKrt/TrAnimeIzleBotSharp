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
using System.Web;

namespace EpEren.Api.tranimeizle.Processors
{
    public class MainProcessor
    {
        private readonly RestClient _HttpClient;

        public MainProcessor(RestClient cl)
        {
            this._HttpClient = cl;
        }
        async public Task<IResult<List<LastComment>>> GetLastComments()
        {
            IResult<List<LastComment>> Donut = null;
            var Yorumlar = new List<LastComment>();
            var Req = new RestRequest("/");
            var Res = this._HttpClient.Get(Req);
            if (Res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Donut = Result.Fail<List<LastComment>>(Res.StatusDescription);
            }
            else
            {
                var Document = await Res.Content.ConvertHTML();
                var YorumlarHTML = Document.QuerySelectorAll(".most-viewed > .content > li");

                foreach (var YorumHTML in YorumlarHTML)
                {
                    LastComment LastComment = new LastComment();
                    LastComment.ID = Convert.ToInt32(YorumHTML.QuerySelectorAll("span")[0].Text());
                    LastComment.Episode = YorumHTML.QuerySelectorAll("span")[1].QuerySelector("a").Attributes["href"].Value;

                    var MainText = YorumHTML.QuerySelectorAll("span")[1].QuerySelector("a").Text();
                    var Bol = Regex.Split(MainText, " için;");

                    LastComment.User = Bol[1].Trim().Replace(" ", "");

                    YorumHTML.QuerySelectorAll("span")[1].QuerySelector("a").Remove();
                    YorumHTML.QuerySelectorAll("span")[1].QuerySelector("br").Remove();

                    LastComment.Comment = YorumHTML.QuerySelectorAll("span")[1].Text().ClearHtmlTags();

                    Yorumlar.Add(LastComment);
                }

                Donut = Result.Success(Yorumlar);
            }

            return Donut;
        }
        async public Task<IResult<Popular>> GetSearch(string Text,int Page = 1)
        {
            IResult<Popular> Donut = null;
            var Liste = new Popular();

            var Req = new RestRequest("arama/"+HttpUtility.UrlEncode(Text)+ "?page="+Page);
            var Res = this._HttpClient.Get(Req);
            if (Res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Donut = Result.Fail<Popular>(Res.StatusDescription);
            }
            else
            {
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                var Document = await Res.Content.ConvertHTML();

                var AnimelerHTMLS = Document.QuerySelectorAll(".flx-block");


                foreach (var AnimeHTML in AnimelerHTMLS)
                {
                    ShortShow ShortShow = new ShortShow();
                    ShortShow.Uri = AnimeHTML.Attributes["data-href"].Value;
                    ShortShow.Image = AnimeHTML.QuerySelector("a>img").Attributes["src"].Value;
                    ShortShow.Name = AnimeHTML.QuerySelector(".bar>h4").Text();
                    Regex Onlyizle = new Regex(@"(.*?) izle");

                    var TestIzle = Onlyizle.Match(ShortShow.Name.ToLower());
                    if (TestIzle.Success)
                    {
                        ShortShow.ShowName = ti.ToTitleCase(TestIzle.Groups[1].Value);
                    }
                    else
                    {
                        ShortShow.ShowName = ShortShow.Name;
                    }

                    Liste.Shows.Add(ShortShow);
                }
                var PaginationHtml = Document.QuerySelectorAll(".pagination")[0];

                Liste.Pagination.Current = Convert.ToInt32(PaginationHtml.QuerySelector(".active").Text());

                Liste.Pagination.Max = Convert.ToInt32(PaginationHtml.QuerySelectorAll("li")[PaginationHtml.QuerySelectorAll("li").Length - 2].Text());

                Donut = Result.Success(Liste);
            }

            return Donut;
        }
        async public Task<IResult<Popular>> GetPopularShows(int Page = 1)
        {
            IResult<Popular> Donut = null;
            var Liste = new Popular();

            var Req = new RestRequest("listeler/populer/sayfa-" + Page.ToString());
            var Res = this._HttpClient.Get(Req);
            if (Res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Donut = Result.Fail<Popular>(Res.StatusDescription);
            }
            else
            {
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                var Document = await Res.Content.ConvertHTML();

                var AnimelerHTMLS = Document.QuerySelectorAll(".flx-block");


                foreach (var AnimeHTML in AnimelerHTMLS)
                {
                    ShortShow ShortShow = new ShortShow();
                    ShortShow.Uri = AnimeHTML.Attributes["data-href"].Value;
                    ShortShow.Image = AnimeHTML.QuerySelector("a>img").Attributes["src"].Value;
                    ShortShow.Name = AnimeHTML.QuerySelector(".bar>h4").Text();
                    Regex Onlyizle = new Regex(@"(.*?) izle");

                    var TestIzle = Onlyizle.Match(ShortShow.Name.ToLower());
                    if (TestIzle.Success)
                    {
                        ShortShow.ShowName = ti.ToTitleCase(TestIzle.Groups[1].Value);
                    }
                    else
                    {
                        ShortShow.ShowName = ShortShow.Name;
                    }

                    Liste.Shows.Add(ShortShow);
                }
                var PaginationHtml = Document.QuerySelectorAll(".pagination")[0];

                Liste.Pagination.Current = Convert.ToInt32(PaginationHtml.QuerySelector(".active").Text());

                Liste.Pagination.Max = Convert.ToInt32(PaginationHtml.QuerySelectorAll("li")[PaginationHtml.QuerySelectorAll("li").Length - 2].Text());

                Donut = Result.Success(Liste);
            }

            return Donut;
        }
        async public Task<IResult<NewShow>> GetNewShows(int Page = 1) {
            IResult<NewShow> Donut = null;
            var Liste = new NewShow();

            var Req = new RestRequest("listeler/eklenen/sayfa-" + Page.ToString());
            var Res = this._HttpClient.Get(Req);
            if (Res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Donut = Result.Fail<NewShow>(Res.StatusDescription);
            }
            else
            {
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                Regex Onlyizle = new Regex(@"(.*?) izle");

                var Document = await Res.Content.ConvertHTML();

                var AnimelerHTMLS = Document.QuerySelectorAll(".flx-block");

                
                foreach (var AnimeHTML in AnimelerHTMLS)
                {
                    ShortShow ShortShow = new ShortShow();
                    ShortShow.Uri = AnimeHTML.Attributes["data-href"].Value;
                    ShortShow.Image = AnimeHTML.QuerySelector("a>img").Attributes["src"].Value;
                    ShortShow.Name = AnimeHTML.QuerySelector(".bar>h4").Text();
                    
                    var TestIzle = Onlyizle.Match(ShortShow.Name.ToLower());
                    if (TestIzle.Success)
                    {
                        ShortShow.ShowName = ti.ToTitleCase(TestIzle.Groups[1].Value);
                    }
                    else
                    {
                        ShortShow.ShowName = ShortShow.Name;
                    }

                    Liste.Shows.Add(ShortShow);
                }


                var PaginationHtml = Document.QuerySelectorAll(".pagination")[0];

                Liste.Pagination.Current = Convert.ToInt32(PaginationHtml.QuerySelector(".active").Text());

                Liste.Pagination.Max = Convert.ToInt32(PaginationHtml.QuerySelectorAll("li")[PaginationHtml.QuerySelectorAll("li").Length - 2].Text());

                Donut = Result.Success(Liste);
            }

            return Donut;

        }
        async public Task<IResult<NewEpisode>> GetNewEpisodes(int Page=1)
        {
            IResult<NewEpisode> Donut=null;

            var Liste = new NewEpisode();

            var Req = new RestRequest("listeler/yenibolum/sayfa-"+Page.ToString());
            var Res = this._HttpClient.Get(Req);
            if (Res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Donut = Result.Fail<NewEpisode>(Res.StatusDescription);
            }
            else
            {
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                var Document = await Res.Content.ConvertHTML();

                var BolumlerHTMLS = Document.QuerySelectorAll(".flx-block");

                Regex SezonAndBolum = new Regex(@"(.*?) ([\d]+). sezon ([\d]+). bölüm izle");
                Regex OnlyBolum = new Regex(@"(.*?) ([\d]+). bölüm izle");
                Regex Onlyizle = new Regex(@"(.*?) izle");


                foreach (var BolumHTML in BolumlerHTMLS)
                {
                    ShortEpisode ShortEpisode = new ShortEpisode();
                    var Chip = BolumHTML.QuerySelectorAll(".info-chip")[1].Text().ToLower().Trim().Replace(" ","");

                    var Bol = Chip.Split('/');

                    if (Chip.Contains("böl"))
                    {
                        ShortEpisode.EpID=Bol[0].Replace("böl", "").Trim();
                    }
                    else
                    {
                        ShortEpisode.EpID = Bol[0].Replace(" ","").Trim();
                    }

                    ShortEpisode.Name= BolumHTML.QuerySelector(".bar>h4").Text();

                   
                    var TestSezonAndBolum = SezonAndBolum.Match(ShortEpisode.Name.ToLower());
                    if (TestSezonAndBolum.Success)
                    {
                        ShortEpisode.ShowName = ti.ToTitleCase(TestSezonAndBolum.Groups[1].Value);
                        ShortEpisode.SeasonID = TestSezonAndBolum.Groups[2].Value;
                    }
                    else
                    {
                        var TestBolum = OnlyBolum.Match(ShortEpisode.Name.ToLower());

                        if (TestBolum.Success)
                        {
                            ShortEpisode.ShowName = ti.ToTitleCase(TestBolum.Groups[1].Value);
                        }
                        else
                        {
                            var TestIzle = Onlyizle.Match(ShortEpisode.Name.ToLower());
                            if (TestIzle.Success)
                            {
                                ShortEpisode.ShowName = ti.ToTitleCase(TestIzle.Groups[1].Value);
                            }
                            else
                            {
                                ShortEpisode.ShowName = ShortEpisode.Name;
                            }
                        }


                    }

                    
                    ShortEpisode.Uri = BolumHTML.Attributes["data-href"].Value;
                    ShortEpisode.Image=BolumHTML.QuerySelector("a>img").Attributes["src"].Value;

                    Liste.Episodes.Add(ShortEpisode);
                }

                var PaginationHtml = Document.QuerySelectorAll(".pagination")[0];

                Liste.Pagination.Current = Convert.ToInt32(PaginationHtml.QuerySelector(".active").Text());

                Liste.Pagination.Max = Convert.ToInt32(PaginationHtml.QuerySelectorAll("li")[PaginationHtml.QuerySelectorAll("li").Length - 2].Text());

                Donut = Result.Success(Liste);
            }

            return Donut;
        }
    }
}
