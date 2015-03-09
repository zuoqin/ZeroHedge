using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Text;
using System.Collections;
using System.Web.Caching;

using System.IO;

namespace ZeroHedge.Controllers
{
    public class HomeController : Controller
    {
        //
        public List<Story> articles;
        public List<PageDef> Pages;
        Hashtable PagesIDs = new Hashtable();
        Hashtable StoryIDs = new Hashtable();
        public ActionResult Index(int id = -1)
        {
            PageDef thePageDef = null;

            //return View();
            StoryIDs.Clear();
            //PagesIDs.Clear();
            articles = new List<Story>();
            Pages = (List<PageDef>)ControllerContext.HttpContext.Cache.Get("Pages");
            if (Pages == null || Pages.Count == 0)
            {
                Pages = new List<PageDef>();
            }
            else
            {
                PagesIDs = (Hashtable)ControllerContext.HttpContext.Cache.Get("Pages_IDs");
            }


            //id = (int)ControllerContext.HttpContext.Cache.Get("CurrentPage_ID");

            string URL = "";
            if (id == -1)
            {
#if DEBUG
                //URL = "http://www.zerohedge.com"; // "http://localhost/zh2.html";// 
                URL = "http://localhost/zh2.html";// 
#else
                URL = "http://www.zerohedge.com"; // "http://localhost/zh2.html";// 
#endif

            }
            else
            {
                thePageDef = (PageDef)PagesIDs[id];
#if DEBUG
                //URL = "http://www.zerohedge.com" + thePageDef.Reference; //"http://localhost/zh2.html";// 
                URL = "http://localhost/zh2.html";// 
#else
                URL = "http://www.zerohedge.com" + thePageDef.Reference; //"http://localhost/zh2.html";// 
#endif

            }
            Pages.Clear();
            PagesIDs.Clear();
            string str1 = URL;
            //str1 = "http://www.zerohedge.com";//
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();

            // Download the markup from 
            //byte[] myDataBuffer = myWebClient.DownloadData(str1);

            // Convert the downloaded data into a string
            string markup = DownloadURL(str1); //Encoding.ASCII.GetString(myDataBuffer);
            int ind5 = 0;
            for (int j = 0; j < 140; j++)
            {
                int ind1 = markup.IndexOf("<div class=\"picture\">", ind5);
                if (ind1 < 0)
                {
                    break;
                }

                int ind2 = markup.IndexOf("<h2 class=\"title\"><a href=", ind1);
                ind2 += 27;
                int ind3 = markup.IndexOf("\">", ind2);
                string ref1 = markup.Substring(ind2, ind3 - ind2);
                ind3 += 2;
                int ind4 = markup.IndexOf("</a>", ind3);
                string title = markup.Substring(ind3, ind4 - ind3);


                ind2 = markup.IndexOf(" on ", ind4);
                ind2 += 4;
                ind3 = markup.IndexOf("</span>", ind2);
                string published = markup.Substring(ind2, ind3 - ind2);

                ind5 = markup.IndexOf("<p>", ind4);
                int ind6 = 0;
                if (markup.Substring(ind5 + 3, 3).CompareTo("<a ") == 0)
                {
                    ind5 = markup.IndexOf("<p>", ind5 + 5);
                    ind6 = ind5 + 3;
                    ind5 = markup.IndexOf("</p>", ind6);
                }
                else if (markup.IndexOf("<img", ind5) < markup.IndexOf("</p>", ind5))
                {
                    ind6 = markup.IndexOf("/>", ind5);
                    ind6 += 2;
                    ind5 = markup.IndexOf("<div class=\"clear-block\"></div>", ind6);

                }
                else
                {
                    //ind6 = markup.IndexOf("</p>", ind5);
                    //ind6 += 4;
                    ind6 = ind5;
                    ind5 = markup.IndexOf("<div class=\"clear-block\"></div>", ind6);
                }
                string body = markup.Substring(ind6, ind5 - ind6);
                if (body.Substring(0, 5).CompareTo("</em>") == 0)
                {
                    body = body.Substring(5);
                }
                if (body.IndexOf("<p>") < 0 || body.IndexOf("</p>") < body.IndexOf("<p>"))
                {
                    body = "<p>" + body;
                }
                var article = new Story()
                {
                    Title = title,
                    Body = body,
#if DEBUG
                    //Reference = ref1, // "http://localhost/zh3.html", //
                    Reference = "http://localhost/zh3.html", //ref1, // 
#else
                    Reference = ref1, // "http://localhost/zh3.html", //
#endif

                    Published = published,
                    ID = j
                };

                articles.Add(article);
                StoryIDs.Add(j, article);


            }

            ViewBag.PageData = articles;
            ViewBag.Header1 = "<div class=\"panel panel-primary\"><div class=\"panel-heading\"><h3 class=\"panel-title\">";
            ViewBag.Header2 = "</h3></div><div class=\"panel-body\">";


            //string ff = ViewBag.Header1 + "jhkhkhj" + ViewBag.Header2 + "The body,,,,,," +  "</div></div>";
            ControllerContext.HttpContext.Cache.Insert("Page1_Articles", articles, null, DateTime.MaxValue, TimeSpan.Zero);
            ControllerContext.HttpContext.Cache.Insert("Page1_IDs", StoryIDs, null, DateTime.MaxValue, TimeSpan.Zero);

            ind5 = 0;
            for (int j = 0; j < 140; j++)
            {
                string ref1 = "";
                string sNum = "";
                int ind1 = markup.IndexOf("<li class=\"pager-item\">", ind5);
                int ind2 = markup.IndexOf("<li class=\"pager-current\"", ind5);
                if ( (ind2 > 0) && (ind2 < ind1) )
                {
                    ind1 = markup.IndexOf("<li class=\"pager-current\"", ind5);
                    ind1 += 26;
                    ind5 = markup.IndexOf("</li", ind1);
                    sNum = markup.Substring(ind1, ind5 - ind1);
                    int dNum = Convert.ToInt32(sNum);
                    dNum -= 1;
                    sNum = dNum.ToString();
                }
                else
                {
                    if (ind1 < 0)
                    {
                        break;
                    }
                    ind2 = markup.IndexOf("<a href=\"", ind1);
                    ind2 += 9;
                    ind5 = markup.IndexOf("\"", ind2);
                    if (markup.IndexOf("&", ind2 + 13) < ind5)
                    {
                        ind5 = markup.IndexOf("&", ind2 + 13);
                    }
                    
                    ref1 = markup.Substring(ind2, ind5 - ind2);
                    int ind3 = markup.IndexOf("page=", ind2);
                    if (markup.IndexOf("page=", ind2) < markup.IndexOf("title=", ind2))
                    {
                        ind3 += 5;

                        if (ind5 > ind3)
                        {

                        }
                        else
                        {
                            ind5 = markup.IndexOf("title=", ind2);
                            ind3 = markup.IndexOf(">", ind5);
                            ind3 += 1;
                            ind5 = markup.IndexOf("<", ind5);

                        }
                        sNum = markup.Substring(ind3, ind5 - ind3);
                        //ref1 = "/articles?page=" + (j + 1);
                    }
                    else
                    {
                        sNum = "0";
                    }
                }
                int dNum1 = Convert.ToInt32(sNum);
                var pageDef = new PageDef()
                {
                    Name = dNum1 == 0 ? "Home" : "Page " + sNum,
#if DEBUG
                    Reference = "http://localhost/zh3.html",//
#else
                    Reference = ref1,// "http://localhost/zh3.html",//
#endif

                    ID = j
                };
                if (dNum1 == 0)
                {
                }
                else
                {
                    Pages.Add(pageDef);
                    PagesIDs.Add(j, pageDef);
                }
            }
            ViewBag.Pages = Pages;
            ViewBag.Pages_IDs = PagesIDs;
            if (id == -1)
            {
                ViewBag.CurrentPage = "Home";
            }
            else
            {
                ViewBag.CurrentPage = thePageDef.Name;
            }
            ControllerContext.HttpContext.Cache.Insert("Pages_IDs", PagesIDs, null, DateTime.MaxValue, TimeSpan.Zero);
            ControllerContext.HttpContext.Cache.Insert("Pages", Pages, null, DateTime.MaxValue, TimeSpan.Zero);
            return View(articles);
        }

        public ActionResult Story(int id)
        {

            List<Story> articles = (List<Story>)ControllerContext.HttpContext.Cache.Get("Page1_Articles");
            Hashtable StoryIDs = (Hashtable)ControllerContext.HttpContext.Cache.Get("Page1_IDs");
            Story theStory = (Story)StoryIDs[id];
            ViewBag.Story = theStory;
            string result = DownLoadStory(theStory);
            theStory.Body = result;
            return View(theStory);
        }

        public string DownloadURL(string uri)
        {
            string s = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            // Set some reasonable limits on resources used by this request
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Method = "GET";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers["Accept-Encoding"] = "gzip,deflate,sdch";
            request.Headers["Accept-Language"] = "ru,en-US;q=0.8,en;q=0.6,zh-CN;q=0.4,zh;q=0.2";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.125 Safari/537.36";
            //request.Connection = "Keep-alive";
            // Set credentials to use for this request.
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();


            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            s = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
            return s;
        }

        
        string DownLoadStory(Story theStory)
        {
            //WebClient myWebClient = new WebClient();

            // Download the markup from 
            //byte[] myDataBuffer = myWebClient.DownloadData(theStory.Reference);

            // Convert the downloaded data into a string
            string markup = DownloadURL(theStory.Reference); //Encoding.ASCII.GetString(myDataBuffer);


            int ind1 = markup.IndexOf("<h1 class=\"title\">" + theStory.Title, 0);
#if DEBUG
            ind1 = markup.IndexOf("<h1 class=\"title\">" + "Is Food Inflation Coming Back", 0); 
#endif

            ind1 = markup.IndexOf("<div class=\"content\">", ind1);
            ind1 += 21;
            int ind2 = markup.IndexOf("<div class=\"fivestar-static-form-item\">", ind1);
            string body = markup.Substring(ind1, ind2 - ind1);
            return body;

        }
    }
}
