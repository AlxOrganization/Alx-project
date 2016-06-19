using Alx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Alx.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            Dictionary<int, List<AnnouncementModel>> Dictionary = new Dictionary<int, List<AnnouncementModel>>();
            
            List<Announcement> ann = new List<Announcement>();
            using (var db = new EntitiesAlx())
            {
                db.Configuration.LazyLoadingEnabled = true;
                IQueryable<Announcement> announcement = db.Announcements.Include("Images");
                if (Request.IsAuthenticated)
                {
                    //Daca userul este autentificat luam categoria la cele mai vizualizat anunt
                    //Request.LogonUserIdentity.User
                    var NoAnnouncement = db.NumberAnnouncements.Where(m => m.UserId == 1).GroupBy(m => m.AnnouncementId).Select(g => new { AnnouncementId = g.Key, Count = g.Count() })
                        .Where(g => g.Count == db.NumberAnnouncements.Where(m => m.UserId == 1).GroupBy(m => m.AnnouncementId).Select(gg => new { AnnouncementId = gg.Key, Count = gg.Count() }).Max(m=>m.Count)).Select(m=>m.AnnouncementId).FirstOrDefault();
                    //NoAnnouncement = NoAnnouncement.GroupBy(m => m.AnnouncementId).Select(g => new { AnnouncementId = g.Key, Max = g.Max(g.Count())});
                    if(NoAnnouncement.HasValue == true)
                    {
                        Int32? IdCategory = db.Announcements.Where(m=>m.Id == NoAnnouncement.Value).Select(m=>m.CategoryId).FirstOrDefault();
                        announcement = db.Announcements.Where(m => m.CategoryId == IdCategory.Value);
                    }
                    else
                    {
                        //Este logat pentru prima oara sau nu a vizualizat pana acum un anunt.
                        var NoAnnouncement1 = db.NumberAnnouncements.GroupBy(m => m.AnnouncementId).Select(g => new { AnnouncementId = g.Key, Count = g.Count() })
                        .Where(g => g.Count == db.NumberAnnouncements.GroupBy(m => m.AnnouncementId).Select(gg => new { AnnouncementId = gg.Key, Count = gg.Count() }).Max(m => m.Count)).Select(m => m.AnnouncementId).FirstOrDefault();

                        if (NoAnnouncement1.HasValue == true)
                        {
                            Int32? IdCategory = db.Announcements.Where(m => m.Id == NoAnnouncement1.Value).Select(m => m.CategoryId).FirstOrDefault();
                            announcement = db.Announcements.Where(m => m.CategoryId == IdCategory.Value);
                        }
                    }
                }
                else
                {
                    var NoAnnouncement1 = db.NumberAnnouncements.GroupBy(m => m.AnnouncementId).Select(g => new { AnnouncementId = g.Key, Count = g.Count() })
                        .Where(g => g.Count == db.NumberAnnouncements.GroupBy(m => m.AnnouncementId).Select(gg => new { AnnouncementId = gg.Key, Count = gg.Count() }).Max(m => m.Count)).Select(m => m.AnnouncementId).FirstOrDefault();

                    if (NoAnnouncement1.HasValue == true)
                    {
                        Int32? IdCategory = db.Announcements.Where(m => m.Id == NoAnnouncement1.Value).Select(m => m.CategoryId).FirstOrDefault();
                        announcement = db.Announcements.Where(m => m.CategoryId == IdCategory.Value);
                    }
                }
                ann = announcement.Select(m => m).Take<Announcement>(4).ToList<Announcement>();
                List<AnnouncementModel> model = new List<AnnouncementModel>();

                foreach (Announcement item in ann)
                {
                    AnnouncementModel model1 = new AnnouncementModel();
                    model1.Id = item.Id;
                    if (item.Images != null && item.Images.Count > 0)
                    {
                        model1.ImageString = new string[1];
                        model1.ImageString[0] = "data:image/jpeg;base64," + Convert.ToBase64String(item.Images.First().PhysicalImage);
                    }
                    model1.Category = db.Categories.Where(m => m.Id == item.CategoryId.Value).Select(m => m.Name).FirstOrDefault<string>();
                    model1.Title = item.Title;
                    if (item.CategoryId.HasValue)
                        model1.CategoryId = item.CategoryId.Value;
                    else
                        model1.CategoryId = 0;
                    model.Add(model1);
                }

                List<Announcement> ann1 = db.Announcements.Select(m => m).OrderByDescending(m => m.DateAnnouncement).Take<Announcement>(4).ToList<Announcement>();
                List<AnnouncementModel> model2 = new List<AnnouncementModel>();
                foreach(Announcement item in ann1)
                {
                    AnnouncementModel model11 = new AnnouncementModel();
                    model11.Id = item.Id;
                    if (item.Images != null && item.Images.Count > 0)
                    {
                        model11.ImageString = new string[1];
                        model11.ImageString[0] = "data:image/jpeg;base64," + Convert.ToBase64String(item.Images.First().PhysicalImage);
                    }
                    model11.Category = db.Categories.Where(m => m.Id == item.CategoryId.Value).Select(m => m.Name).FirstOrDefault<string>();
                    model11.Title = item.Title;
                    if(item.CategoryId.HasValue)                  
                        model11.CategoryId = item.CategoryId.Value;
                    else
                        model11.CategoryId = 0;
                    model2.Add(model11);
                }
                List<Announcement> list = db.Announcements.
                    OrderByDescending(m => m.Vizualization).
                    Select(m => m).Take<Announcement>(4).ToList<Announcement>();

                List<AnnouncementModel> model3 = new List<AnnouncementModel>();

                foreach(Announcement item in list)
                {
                    AnnouncementModel model11 = new AnnouncementModel();
                    model11.Id = item.Id;
                    if (item.Images != null && item.Images.Count > 0)
                    {
                        model11.ImageString = new string[1];
                        model11.ImageString[0] = "data:image/jpeg;base64," + Convert.ToBase64String(item.Images.First().PhysicalImage);
                    }
                    model11.Category = db.Categories.Where(m => m.Id == item.CategoryId.Value).Select(m => m.Name).FirstOrDefault<string>();
                    model11.Title = item.Title;
                    if (item.CategoryId.HasValue)
                        model11.CategoryId = item.CategoryId.Value;
                    else
                        model11.CategoryId = 0;
                    model3.Add(model11);
                }
                Dictionary.Add(1, model);
                Dictionary.Add(2, model2);
                Dictionary.Add(3, model3);
            }

            return View(Dictionary);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
