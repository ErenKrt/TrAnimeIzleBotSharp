# TrAnimeIzleBotSharp / TrAnimeİzle Botu / tranimeizle botu / Anime Botu | .Net
Botun çalışma prensibi bir kullanıcı gibi  **tranimeizle.net** adresine girerek **HtmlParser** mantığında alanları (**div**/**section**/**vb..**) parçalıyor ve size özel **object**ler olarak döndürüyor

---
- [Bilgilendirme](#bilgilendirme)
- [Kurulum](#kurulum)
---


# Bilgilendirme
> Multi Platforms desteği

Sürümleri kaynak kodlarını indirdikten sonra build ederek değişebilirsiniz.
# Kurulum
> [Release](https://github.com/ErenKrt/TrAnimeIzleBotSharp/releases) kısmından güncel **dll**leri indirip projenize ekleyerek kullanabilirsiniz.

## Kod Kullanımları
```csharp
var TrAnime = new TrAnimeIzle();

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

TrAnime.SessionHelper.SetCookies(Cookies);
TrAnime.SessionHelper.SetUserAgent("{YOUR USERAGENT}");

var AreWeOkay = TrAnime.SessionHelper.TestConnection();
```
---
Geliştirci: &copy; [ErenKrt](https://www.instagram.com/ep.eren/)
