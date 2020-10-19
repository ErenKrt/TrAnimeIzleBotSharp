using EpEren.Api.tranimeizle.Classes;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace EpEren.Api.tranimeizle.Helpers
{
    
    public class SessionHelper
    {
        private readonly RestClient _HttpClient;
        private List<Cookie> Cookies = new List<Cookie>();

        public SessionHelper(RestClient _Cl)
        {
            this._HttpClient = _Cl;

            this.ResetUserAgent();
        }
        public bool TestConnection()
        {
            var Donut = false;

            var Res= this._HttpClient.Get(new RestRequest("/"));

            if (Res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Donut = true;
            }
          

            return Donut;
        }
        public void ResetUserAgent()
        {
            this._HttpClient.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36 OPR/71.0.3770.198";
        }
        public void SetUserAgent(string UserAgent)
        {
            this._HttpClient.UserAgent = UserAgent;
        }
        public void SetCookies(List<Cookie> cookies = null)
        {
            if (cookies != null)
            {
                this.Cookies = cookies;
                UpdateCookies();
            }

        }
        public void AddCookies(List<Cookie> cookies=null)
        {
            if (cookies != null)
            {
                foreach (var scookie in cookies)
                {
                    this.Cookies.Add(scookie);
                }
                UpdateCookies();
            }
            
        }
        public void AddCookie(string name,string val)
        {
            Cookies.Add(new Cookie() { 
                Name=name,
                Value=val
            });

            UpdateCookies();
        }
        public void ResetCookies()
        {
          this.Cookies = new List<Cookie>();

          UpdateCookies();

        }
        public void RemoveCookie(string name)
        {
            var Bul = this.Cookies.FindIndex(x => x.Name.ToLower() == name.ToLower());
            if (Bul != -1)
            {
                this.Cookies.RemoveAt(Bul);

                UpdateCookies();
            }
        }
        private void UpdateCookies()
        {
            var CookCont = new System.Net.CookieContainer();

            foreach (var SCookie in this.Cookies)
            {
                CookCont.Add(new System.Net.Cookie()
                {
                    Domain= ".tranimeizle.net",
                    Name = SCookie.Name,
                    Value = SCookie.Value
                });
            }

            this._HttpClient.CookieContainer = CookCont;
        }
    }
}
