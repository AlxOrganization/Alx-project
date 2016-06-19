using Alx.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Linq.SqlClient;
using WebMatrix.WebData;
using PagedList;

namespace Alx.Controllers
{
    public class AnnouncementController : Controller
    {
        private SendEmail email = new SendEmail();
        public PartialViewResult Currency(int id)
        {
            List<Announcement> ann = new List<Announcement>();
            Session["OrderBy"] = null;
            using (var db = new EntitiesAlx())
            {
                
                IQueryable<Announcement> announcement;
                announcement = db.Announcements.Include("Images");

                if(id == 0)
                {
                    //Cautam doar acele anunturi cu pretul in RON
                    Session["Currency"] = "RON";
                    announcement = announcement.Where(m => m.Currency == 0);
                }
                else
                {
                    //Cautam doar acele anunturi cu pretul in euro
                    Session["Currency"] = "EURO";
                    announcement = announcement.Where(m => m.Currency == 1);
                }

                if (Session["TitleSearch"] != null)
                {
                    string search = Session["TitleSearch"].ToString();
                    announcement = announcement.Where(m => m.Title.Contains(search) || m.Description.Contains(search));
                }
                if (Session["CategorySearch"] != null && Session["CategorySearch"].ToString() != "-1")
                {
                    List<int> Parents = (List<int>)Session["CategorySearch"];
                    int category = Int32.Parse(Session["InputCategory"].ToString());
                    if (Parents.Count() > 0)
                    {
                        announcement = announcement.Where(m => Parents.Contains(m.CategoryId.Value));
                    }
                    else
                    {
                        announcement = announcement.Where(m => m.CategoryId == category);
                    }
                }
                if (Session["CountySearch"] != null && Session["CountySearch"].ToString() == "1")
                {
                    //trebuie sa cautam dupa judet
                    //se va cauta dupa toate id-urile de orase din judetul respectiv
                    string countyName = Session["CountySearch"].ToString();
                    if (countyName != "")
                    {
                        int? idCounty = (from m in db.Counties
                                         where m.CountyName.ToUpper() == countyName
                                         select m.Id).FirstOrDefault<int>();
                        IQueryable<int> IdCities = (from m in db.Cities
                                                    where m.County.Id == idCounty
                                                    select m.Id);
                        announcement = announcement.Where(m => IdCities.Contains<int>(m.Address.CityId.Value));

                        Session["Cities"] = new
                        {
                            Status = 1,
                            ID = idCounty.Value
                        };

                    }
                }
                if (Session["CitySearch"] != null)
                {
                    //trebuie sa cautam dupa acel id de oras
                    string CitiName = Session["CitySearch"].ToString();
                    int? idCounty = (from m in db.Cities
                                     where m.CityName.ToUpper() == CitiName
                                     select m.Id).FirstOrDefault<int>();
                    announcement = announcement.Where(m => m.Address.CityId == idCounty.Value);
                }

                Session["AllAnnouncementNo"] = announcement.Count();
                Session["FirmaAnnouncementNo"] = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIRMA").Select(m => m.Id).Count<Int32>();
                Session["PrivatAnnouncementNo"] = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIZICA").Select(m => m.Id).Count<Int32>();

                if (Session["ActiveAnnouncements"] != null && Session["ActiveAnnouncements"].ToString() != "TOATE")
                {

                    string Enterprenor = Session["ActiveAnnouncements"].ToString();
                    if (Enterprenor == "PRIVAT")
                    {
                        Enterprenor = "FIZICA";
                    }
                    announcement = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == Enterprenor);
                }
                ViewBag.PageActive = 1;
                ViewBag.PageSize = Int32.Parse(Math.Ceiling((Double)announcement.Count() / 9.00).ToString());                
                //Session["FirmaAnnouncements"] = ann.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIRMA").Select(m => m).ToList<Announcement>();
                //Session["PrivatAnnouncements"] = ann.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIZICA").Select(m => m).ToList<Announcement>();
                IPagedList<Announcement> ann1 = announcement.OrderByDescending(m => m.DateAnnouncement).ToPagedList<Announcement>(1, 9); ;

                ann = ann1.ToList<Announcement>();
            }
            return PartialView("Announcement", ann);
        }
        public PartialViewResult OrderBy(int id)
        {
            List<Announcement> ann = new List<Announcement>();
            

            using (var db = new EntitiesAlx())
            {
                //db.Configuration.LazyLoadingEnabled = true;
                IQueryable<Announcement> announcement;
                announcement = db.Announcements.Include("Images");
                if (Session["Currency"] != null)
                {
                    if (Session["Currency"].ToString() == "RON")
                    {
                        announcement = announcement.Where(m => m.Currency == 0);
                    }
                    else
                    {
                        announcement = announcement.Where(m => m.Currency == 1);
                    }
                }

                if (Session["TitleSearch"] != null)
                {
                    string search = Session["TitleSearch"].ToString();
                    announcement = announcement.Where(m => m.Title.Contains(search) || m.Description.Contains(search));
                }
                if (Session["CategorySearch"] != null && Session["CategorySearch"].ToString() != "-1")
                {
                    List<int> Parents = (List<int>)Session["CategorySearch"];
                    int category = Int32.Parse(Session["InputCategory"].ToString());
                    if (Parents.Count() > 0)
                    {
                        announcement = announcement.Where(m => Parents.Contains(m.CategoryId.Value));
                    }
                    else
                    {
                        announcement = announcement.Where(m => m.CategoryId == category);
                    }
                }
                if (Session["CountySearch"] != null && Session["CountySearch"].ToString() == "1")
                {
                    //trebuie sa cautam dupa judet
                    //se va cauta dupa toate id-urile de orase din judetul respectiv
                    string countyName = Session["CountySearch"].ToString();
                    if (countyName != "")
                    {
                        int? idCounty = (from m in db.Counties
                                         where m.CountyName.ToUpper() == countyName
                                         select m.Id).FirstOrDefault<int>();
                        IQueryable<int> IdCities = (from m in db.Cities
                                                    where m.County.Id == idCounty
                                                    select m.Id);
                        announcement = announcement.Where(m => IdCities.Contains<int>(m.Address.CityId.Value));

                        Session["Cities"] = new
                        {
                            Status = 1,
                            ID = idCounty.Value
                        };

                    }
                }
                if (Session["CitySearch"] != null)
                {
                    //trebuie sa cautam dupa acel id de oras
                    string CitiName = Session["CitySearch"].ToString();
                    int? idCounty = (from m in db.Cities
                                     where m.CityName.ToUpper() == CitiName
                                     select m.Id).FirstOrDefault<int>();
                    announcement = announcement.Where(m => m.Address.CityId == idCounty.Value);
                }

                if (Session["ActiveAnnouncements"] != null && Session["ActiveAnnouncements"].ToString() != "TOATE")
                {

                    string Enterprenor = Session["ActiveAnnouncements"].ToString();
                    if (Enterprenor == "PRIVAT")
                    {
                        Enterprenor = "FIZICA";
                    }
                    announcement = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == Enterprenor);
                }
                ViewBag.PageActive = 1;
                ViewBag.PageSize = Int32.Parse(Math.Ceiling((Double)announcement.Count() / 9.00).ToString());

                //Session["AllAnnouncementNo"] = announcement.Count();
                //Session["FirmaAnnouncementNo"] = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIRMA").Select(m => m.Id).Count<Int32>();
                //Session["PrivatAnnouncementNo"] = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIZICA").Select(m => m.Id).Count<Int32>();
                //Session["FirmaAnnouncements"] = ann.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIRMA").Select(m => m).ToList<Announcement>();
                //Session["PrivatAnnouncements"] = ann.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIZICA").Select(m => m).ToList<Announcement>();
                IPagedList<Announcement> ann1;
                switch(id)
                {
                    case 1://Suntem in cazul in care trebuie sa ordonam toate articolele dupa noi
                        ann1 = announcement.OrderByDescending(m => m.DateAnnouncement).ToPagedList<Announcement>(1, 9);
                        Session["OrderBy"] = "Noi";
                        break;
                    case 2://Suntem in cazul in care trebuie sa ordonam dupa anunturile ieftine
                        ann1 = announcement.OrderBy(m => m.Price).ToPagedList<Announcement>(1, 9);
                        Session["OrderBy"] = "Ieftine";
                        break;
                    case 3://Suntem in cazul in care trebuie sa ordonam dupa anunturile scumpe
                        ann1 = announcement.OrderByDescending(m => m.Price).ToPagedList<Announcement>(1, 9);
                        Session["OrderBy"] = "Scumpe";
                        break;
                    case 4: //Suntem in cazul in care trebuie sa ordonam dupa anunturile viziualizate
                        ann1 = announcement.OrderByDescending(m => m.Vizualization).ToPagedList<Announcement>(1, 9);
                        Session["OrderBy"] = "Populare";
                        break;
                    default:
                        ann1 = announcement.OrderByDescending(m => m.DateAnnouncement).ToPagedList<Announcement>(1, 9);
                        Session["OrderBy"] = null;
                        break;
                }
                 
                ann = ann1.ToList<Announcement>();
            }
            return PartialView("Announcement", ann);
        }
        public PartialViewResult Entrepeneur(int id)
        {
            
            ViewBag.PageSize = 0;
            ViewBag.PageActive = 1;
            Session["OrderBy"] = null;
            Session["Currency"] = "RON";
            #region removed
            //if(Session["Announcements"] != null && Session["FirmaAnnouncements"] != null && Session["PrivatAnnouncements"] != null)
            //{
            //    if (id == 1)
            //    {
            //        list = (List<Announcement>)Session["Announcements"];
            //        Session["ActiveAnnouncements"] = "TOATE";
            //    }
            //    if (id == 2)
            //    {
            //        list = (List<Announcement>)Session["PrivatAnnouncements"];
            //        Session["ActiveAnnouncements"] = "PRIVAT";
            //    }
            //    if (id == 3)
            //    {
            //        list = (List<Announcement>)Session["FirmaAnnouncements"];
            //        Session["ActiveAnnouncements"] = "FIRMA";
            //    }

            //    if (list.Count > 9)
            //    {
            //        //Daca exista mai multe anunturi decat 9, trebuie sa facem paginarea
            //        ViewBag.PageActive = 1;
            //        ViewBag.PageSize = Int32.Parse(Math.Ceiling((Double)list.Count / 9.00).ToString());

                    
            //        List<Announcement> list1 = new List<Announcement>();
            //        for (int i = 0; i < 9; i++)
            //        {
            //            list1.Add(list[i]);
            //        }

            //        return PartialView("Announcement", list1);

            //    }
            //    else
            //    {
            //        ViewBag.PageActive = 0;
            //        ViewBag.PageSize = 0;                   
            //    }
            //}
            #endregion 
            
            List<Announcement> ann = new List<Announcement>();
            using (var db = new EntitiesAlx())
            {
                IQueryable<Announcement> announcement;
                announcement = db.Announcements.Include("Images");

                if (Session["Currency"].ToString() == "RON")
                {
                    announcement = announcement.Where(m => m.Currency == 0);
                }
                else
                {
                    announcement = announcement.Where(m => m.Currency == 1);
                }

                if (Session["TitleSearch"] != null)
                {
                    string search = Session["TitleSearch"].ToString();
                    announcement = announcement.Where(m => m.Title.Contains(search) || m.Description.Contains(search));
                }
                if (Session["CategorySearch"] != null && Session["CategorySearch"].ToString() != "-1")
                {
                    List<int> Parents = (List<int>)Session["CategorySearch"];
                    int category = Int32.Parse(Session["InputCategory"].ToString());
                    if (Parents.Count() > 0)
                    {
                        announcement = announcement.Where(m => Parents.Contains(m.CategoryId.Value));
                    }
                    else
                    {
                        announcement = announcement.Where(m => m.CategoryId == category);
                    }
                }
                if (Session["CountySearch"] != null && Session["CountySearch"].ToString() == "1")
                {
                    //trebuie sa cautam dupa judet
                    //se va cauta dupa toate id-urile de orase din judetul respectiv
                    string countyName = Session["CountySearch"].ToString();
                    if (countyName != "")
                    {
                        int? idCounty = (from m in db.Counties
                                         where m.CountyName.ToUpper() == countyName
                                         select m.Id).FirstOrDefault<int>();
                        IQueryable<int> IdCities = (from m in db.Cities
                                                    where m.County.Id == idCounty
                                                    select m.Id);
                        announcement = announcement.Where(m => IdCities.Contains<int>(m.Address.CityId.Value));

                        Session["Cities"] = new
                        {
                            Status = 1,
                            ID = idCounty.Value
                        };

                    }
                }
                if (Session["CitySearch"] != null)
                {
                    //trebuie sa cautam dupa acel id de oras
                    string CitiName = Session["CitySearch"].ToString();
                    int? idCounty = (from m in db.Cities
                                     where m.CityName.ToUpper() == CitiName
                                     select m.Id).FirstOrDefault<int>();
                    announcement = announcement.Where(m => m.Address.CityId == idCounty.Value);
                }

                Session["AllAnnouncementNo"] = announcement.Count();
                Session["FirmaAnnouncementNo"] = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIRMA").Select(m => m.Id).Count<Int32>();
                Session["PrivatAnnouncementNo"] = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIZICA").Select(m => m.Id).Count<Int32>();

                if (id == 1)
                {
                    //announcement = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIRMA");
                    Session["ActiveAnnouncements"] = "TOATE";
                }
                if (id == 2)
                {
                    announcement = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIZICA");
                    Session["ActiveAnnouncements"] = "PRIVAT";
                }
                if (id == 3)
                {
                    announcement = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIRMA");
                    Session["ActiveAnnouncements"] = "FIRMA";
                }
                
                ViewBag.PageActive = 1;
                ViewBag.PageSize = Int32.Parse(Math.Ceiling((Double)announcement.Count() / 9.00).ToString());

                
                //Session["FirmaAnnouncements"] = ann.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIRMA").Select(m => m).ToList<Announcement>();
                //Session["PrivatAnnouncements"] = ann.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIZICA").Select(m => m).ToList<Announcement>();

                IPagedList<Announcement> ann1 = announcement.OrderBy(m => m.DateAnnouncement).ToPagedList<Announcement>(1, 9);
                ann = ann1.ToList<Announcement>();
            }

            return PartialView("Announcement", ann);
        }
        public PartialViewResult Paginating(int id)
        {
            List<Announcement> ann = new List<Announcement>();
            using (var db = new EntitiesAlx())
            {
                //db.Configuration.LazyLoadingEnabled = true;
                IQueryable<Announcement> announcement;
                announcement = db.Announcements.Include("Images");
                if(Session["Currency"] != null)
                {
                    if (Session["Currency"].ToString() == "RON")
                    {
                        announcement = announcement.Where(m => m.Currency == 0);
                    }
                    else
                    {
                        announcement = announcement.Where(m => m.Currency == 1);
                    }
                }
                if (Session["TitleSearch"] != null)
                {
                    string search = Session["TitleSearch"].ToString();
                    announcement = announcement.Where(m => m.Title.Contains(search) || m.Description.Contains(search));                    
                }
                if (Session["CategorySearch"] != null && Session["CategorySearch"].ToString() != "-1")
                {
                    List<int> Parents = (List<int>)Session["CategorySearch"];
                    int category = Int32.Parse(Session["InputCategory"].ToString());
                    if (Parents.Count() > 0)
                    {
                        announcement = announcement.Where(m => Parents.Contains(m.CategoryId.Value));
                    }
                    else
                    {
                        announcement = announcement.Where(m => m.CategoryId == category);
                    }
                }
                if (Session["CountySearch"] != null && Session["CountySearch"].ToString() == "1")
                {
                    //trebuie sa cautam dupa judet
                    //se va cauta dupa toate id-urile de orase din judetul respectiv
                    string countyName = Session["CountySearch"].ToString();
                    if (countyName != "")
                    {
                        int? idCounty = (from m in db.Counties
                                         where m.CountyName.ToUpper() == countyName
                                         select m.Id).FirstOrDefault<int>();
                        IQueryable<int> IdCities = (from m in db.Cities
                                                    where m.County.Id == idCounty
                                                    select m.Id);
                        announcement = announcement.Where(m => IdCities.Contains<int>(m.Address.CityId.Value));

                        Session["Cities"] = new
                        {
                            Status = 1,
                            ID = idCounty.Value
                        };

                    }
                }
                if (Session["CitySearch"] != null)
                {
                    //trebuie sa cautam dupa acel id de oras
                    string CitiName = Session["CitySearch"].ToString();
                    int? idCounty = (from m in db.Cities
                                     where m.CityName.ToUpper() == CitiName
                                     select m.Id).FirstOrDefault<int>();
                    announcement = announcement.Where(m => m.Address.CityId == idCounty.Value);
                }

                if (Session["ActiveAnnouncements"] != null && Session["ActiveAnnouncements"].ToString() != "TOATE")
                {

                    string Enterprenor = Session["ActiveAnnouncements"].ToString();
                    if(Enterprenor == "PRIVAT")
                    {
                        Enterprenor = "FIZICA";
                    }
                    announcement = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == Enterprenor);
                }
                ViewBag.PageActive = id;
                ViewBag.PageSize = Int32.Parse(Math.Ceiling((Double)announcement.Count() / 9.00).ToString());

                //Session["AllAnnouncementNo"] = announcement.Count();
                //Session["FirmaAnnouncementNo"] = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIRMA").Select(m => m.Id).Count<Int32>();
                //Session["PrivatAnnouncementNo"] = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIZICA").Select(m => m.Id).Count<Int32>();
                //Session["FirmaAnnouncements"] = ann.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIRMA").Select(m => m).ToList<Announcement>();
                //Session["PrivatAnnouncements"] = ann.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIZICA").Select(m => m).ToList<Announcement>();

                IPagedList<Announcement> ann1 = announcement.OrderByDescending(m => m.DateAnnouncement).ToPagedList<Announcement>(id, 9);
                if (Session["OrderBy"] != null)
                {
                    switch (Session["OrderBy"].ToString())
                    {
                        case "Noi"://Suntem in cazul in care trebuie sa ordonam toate articolele dupa noi
                            ann1 = announcement.OrderByDescending(m => m.DateAnnouncement).ToPagedList<Announcement>(id, 9);
                            Session["OrderBy"] = "Noi";
                            break;
                        case "Ieftine"://Suntem in cazul in care trebuie sa ordonam dupa anunturile ieftine
                            ann1 = announcement.OrderBy(m => m.Price).ToPagedList<Announcement>(id, 9);
                            Session["OrderBy"] = "Ieftine";
                            break;
                        case "Scumpe"://Suntem in cazul in care trebuie sa ordonam dupa anunturile scumpe
                            ann1 = announcement.OrderByDescending(m => m.Price).ToPagedList<Announcement>(id, 9);
                            Session["OrderBy"] = "Scumpe";
                            break;
                        case "Populare": //Suntem in cazul in care trebuie sa ordonam dupa anunturile cele mai vizualizate
                            Session["OrderBy"] = "Populare";
                            ann1 = announcement.OrderByDescending(m => m.Vizualization).ToPagedList<Announcement>(id, 9);
                            break;
                    }
                }

                ann = ann1.ToList<Announcement>();                
            }
            return PartialView("Announcement", ann);
        }
        public ActionResult SearchAnnouncement(int id)
        {

            IQueryable<Announcement> announcement;
            Announcement ann = new Announcement();
            using (var db = new EntitiesAlx())
            {
                announcement = db.Announcements.Include("Images").Include("Address");
                ann = announcement.Where(m => m.Id == id).SingleOrDefault<Announcement>();
                if (ann.Vizualization.HasValue)
                {
                    ann.Vizualization += 1;
                }
                else
                {
                    ann.Vizualization = 0;
                }
                if (Session["UserId"] != null)
                {
                    NumberAnnouncement numberAnnouncement = new NumberAnnouncement();
                    numberAnnouncement.UserId = Int32.Parse(Session["UserId"].ToString());
                    numberAnnouncement.AnnouncementId = id;
                    db.NumberAnnouncements.Add(numberAnnouncement);
                    
                }
                else
                {
                    if (Request.IsAuthenticated)
                    {
                        NumberAnnouncement numberAnnouncement = new NumberAnnouncement();
                        //WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                        numberAnnouncement.UserId = WebSecurity.GetUserId(User.Identity.Name);
                        Session["UserId"] = numberAnnouncement.UserId;
                        numberAnnouncement.AnnouncementId = id;
                        db.NumberAnnouncements.Add(numberAnnouncement);
                       
                    }
                    else
                    {
                        NumberAnnouncement numberAnnouncement = new NumberAnnouncement();
                        //WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                        //numberAnnouncement.UserId = WebSecurity.GetUserId(User.Identity.Name);
                        //Session["UserId"] = numberAnnouncement.UserId;
                        numberAnnouncement.AnnouncementId = id;
                        db.NumberAnnouncements.Add(numberAnnouncement);
                       
                    }
                }
                db.SaveChanges();
                return View(ann);
            }
        }
        public ActionResult Search(FormCollection collection)
        {

            Session["AllAnnouncementNo"] = 0;
            Session["FirmaAnnouncementNo"] = 0;
            Session["PrivatAnnouncementNo"] = 0;            
            Session["ActiveAnnouncements"] = "TOATE";
            Session["TitleSearch"] = null;
            Session["CategorySearch"] = null;
            Session["InputCategory"] = null;
            Session["CountySearch"] = null;
            Session["Cities"] = null;
            Session["CitySearch"] = null;
            Session["OrderBy"] = null;
            Session["Currency"] = "RON";

            int category = -1;
            try
            {
                category = int.Parse(Request.Form["MainCategory"].ToString());
            }
            catch
            {
            }
            string search = Request.Form["InputSearch"].ToString();
            string[] county = Request.Form["InputCounty"].ToString().Split(',');
            string OrderBy = Request.Form["OrderByPreference"].ToString();
            List<Announcement> ann = new List<Announcement>();                 
            using (var db = new EntitiesAlx())
            {
                //db.Configuration.LazyLoadingEnabled = true;
                IQueryable<Announcement> announcement;
                announcement = db.Announcements.Include("Images");
                if (Session["Currency"] != null)
                {
                    if (Session["Currency"].ToString() == "RON")
                    {
                        announcement = announcement.Where(m => m.Currency == 0);
                    }
                    else
                    {
                        announcement = announcement.Where(m => m.Currency == 1);
                    }
                }
                if(search != "" && search != null)
                {
                    announcement = announcement.Where(m => m.Title.Contains(search) || m.Description.Contains(search));
                    Session["TitleSearch"] = search;
                }
                if(category != -1)
                {
                    IQueryable<int> Parents = db.Categories.Where(m => m.ParentId == category).Select(m => m.Id);
                    Session["CategorySearch"] = Parents.ToList<int>();
                    Session["InputCategory"] = category;
                    if(Parents.Count() > 0)
                    {
                        announcement = announcement.Where(m => Parents.Contains(m.CategoryId.Value));
                    }
                    else
                    {
                        announcement = announcement.Where(m => m.CategoryId == category);
                    }
                }
                if(county.Count() > 0)
                {
                    if(county.Length == 1)
                    {
                        //trebuie sa cautam dupa judet
                        //se va cauta dupa toate id-urile de orase din judetul respectiv
                        string countyName = county[0].Trim().ToUpper();
                        Session["CountySearch"] = countyName;
                        if (countyName != "")
                        {
                            int? idCounty = (from m in db.Counties
                                             where m.CountyName.ToUpper() == countyName
                                             select m.Id).FirstOrDefault<int>();
                            IQueryable<int> IdCities = (from m in db.Cities
                                                  where m.County.Id == idCounty
                                                  select m.Id);
                            announcement = announcement.Where(m => IdCities.Contains<int>(m.Address.CityId.Value));

                            Session["Cities"] = new 
                            {
                                Status = 1,
                                ID = idCounty.Value
                            };

                        }
                    }
                    else
                    {
                        //trebuie sa cautam dupa acel id de oras
                        string CitiName = county[1].Trim().ToUpper();
                        Session["CitySearch"] = CitiName;
                        int? idCounty = (from m in db.Cities
                                         where m.CityName.ToUpper() == CitiName
                                         select m.Id).FirstOrDefault<int>();
                        announcement = announcement.Where(m => m.Address.CityId == idCounty.Value);
                    }
                }
                ViewBag.PageActive = 1;
                ViewBag.PageSize = Int32.Parse(Math.Ceiling((Double)announcement.Count() / 9.00).ToString());

                Session["AllAnnouncementNo"] = announcement.Count();
                Session["FirmaAnnouncementNo"] = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIRMA").Select(m => m.Id).Count<Int32>();
                Session["PrivatAnnouncementNo"] = announcement.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIZICA").Select(m => m.Id).Count<Int32>();
                //Session["FirmaAnnouncements"] = ann.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIRMA").Select(m => m).ToList<Announcement>();
                //Session["PrivatAnnouncements"] = ann.Where(m => m.Entrepeneur.Trim().ToUpper() == "FIZICA").Select(m => m).ToList<Announcement>();
                Session["ActiveAnnouncements"] = "TOATE";
                IPagedList<Announcement> ann1;
                if(OrderBy != "")
                {
                    if (OrderBy == "NOI")
                    {
                        ann1 = announcement.OrderByDescending(m => m.DateAnnouncement).ToPagedList<Announcement>(1, 9);
                        Session["OrderBy"] = "Noi";
                    }
                    else
                    {
                        ann1 = announcement.OrderByDescending(m => m.Vizualization).ToPagedList<Announcement>(1, 9);
                        Session["OrderBy"] = "Populare";
                    }
                }
                else
                {
                    ann1 = announcement.OrderByDescending(m => m.DateAnnouncement).ToPagedList<Announcement>(1, 9);
                }
 
                ann = ann1.ToList<Announcement>();
            }

            #region removed
            //using(var db = new EntitiesAlx())
            //{
            //    int? idCategory = null;
            //    int ChildCategory = 0;
            //    if(category.Length > 0 && category[0] != "")
            //    {
            //        string name = category[category.Length - 1].ToUpper().Trim();
            //        int? countCategory = (from m in db.Categories
            //                              where m.Name.ToUpper() == name
            //                              select m.ParentId).Count<int>();
            //        if (countCategory.HasValue == true && countCategory.Value == 1)
            //        {
            //            //Categoria este unica
            //            idCategory = (from m in db.Categories
            //                          where m.Name.ToUpper() == name
            //                          select m.Id).SingleOrDefault<int>();
            //            ChildCategory = (from m in db.Categories
            //                             where m.ParentId == idCategory
            //                             select m.Id).Count<int>();
            //        }
            //        else
            //        {
            //            //Categoria nu este unica
            //            string name1 = category[category.Length-2].Trim().ToUpper();
            //            idCategory = (from m in db.Categories
            //                          where m.Name.ToUpper() == name
            //                          && m.ParentId == (from n in db.Categories
            //                                            where n.Name.ToUpper() == name1
            //                                            select n.Id).FirstOrDefault<int>()
            //                          select m.Id
            //                              ).SingleOrDefault<int>();
            //            ChildCategory = (from m in db.Categories
            //                             where m.ParentId == idCategory
            //                             select m.Id).Count<int>();
            //        }
            //        ExistCategory = true;
            //    }
            //    List<int> IdCity = new List<int>();
            //    int? idCity = null;
            //    if(county.Length > 0)
            //    {
            //        if(county.Length == 1)
            //        {
            //            //trebuie sa cautam dupa judet
            //            //se va cauta dupa toate id-urile de orase din judetul respectiv
            //            string countyName = county[0].Trim().ToUpper();
            //            if (countyName != "")
            //            {
            //                int? idCounty = (from m in db.Counties
            //                                 where m.CountyName.ToUpper() == countyName
            //                                 select m.Id).FirstOrDefault<int>();
            //                List<int> IdCities = (from m in db.Cities
            //                                      where m.County.Id == idCounty
            //                                      select m.Id).ToList<int>();
            //                IdCity = IdCities;
            //                ExistCitie = true;
            //            }
            //        }
            //        else
            //        {
            //            string CitiName = county[1].Trim().ToUpper();
            //            int? idCounty = (from m in db.Cities
            //                             where m.CityName.ToUpper() == CitiName
            //                             select m.Id).FirstOrDefault<int>();
            //            idCity = idCounty.Value;
            //            //trebuie sa cautam dupa acel id de oras
            //            ExistCitie = true;
            //        }
                    
            //    }
            //    //var announcements = (from m in db.Announcements
            //    //                     where m.Title.Contains(search) ||
            //    //                     m.Description.Contains(search)
            //    //                     select m);
            //    //List<Announcement> announcements = db.Announcements.ToList<Announcement>();
            //    //var announcements = db.Announcements.Where(m => m.Title.Contains(search) || m.Description.Contains(search));
            //    var announcements = (from m in db.Announcements
            //                         select m);

            //    if(search != "")
            //    {
            //        //Exista un text anume dupa care se poate face seach
            //        //Trebuie sa construim selectul dupa toate crriteriile in parte

            //        if(ExistCategory == true && ExistCitie == true)
            //        {
            //            //Suntem in situatia cand cautam dupa toate criteriile
            //            if (IdCity.Count > 0)
            //            {
            //                //cautam in toate orsajele din judet
            //                //announcements.Where(m => IdCity.Contains(m.Address.CityId.Value) && m.Address.CityId.HasValue == true);
            //                if (idCategory.HasValue)
            //                {
            //                    if (ChildCategory == 0)
            //                    {
            //                        //Nu exista alte subcategorii
            //                        //announcements.Where(m => m.Category.Id == idCategory.Value);
            //                        announcements = (from m in db.Announcements
            //                                         where (m.Title.Contains(search) || m.Description.Contains(search))
            //                                         && IdCity.Contains(m.Address.CityId.Value) && m.Address.CityId.HasValue == true
            //                                         && m.CategoryId.Value == idCategory.Value
            //                                         select m);
            //                    }
            //                    else
            //                    {
            //                        //trebuie sa cautam in toate subcategoriile acelei categorii
            //                        List<int> idCategories = (from m in db.Categories
            //                                                  where m.ParentId == idCategory.Value
            //                                                  select m.Id).ToList<int>();
            //                        //announcements.Where(m => idCategories.Contains(m.Category.Id));
            //                        announcements = (from m in db.Announcements
            //                                         where (m.Title.Contains(search) || m.Description.Contains(search))
            //                                         && IdCity.Contains(m.Address.CityId.Value) && m.Address.CityId.HasValue == true
            //                                         && idCategories.Contains(m.CategoryId.Value)
            //                                         select m);
            //                    }
            //                }
                            
            //            }
            //            if (idCity.HasValue)
            //            {
            //                //acutam dupa un singur oras
            //                //announcements.Where(m => m.Address.CityId.Value == idCity && m.Address.CityId.HasValue == true);
            //                if (idCategory.HasValue)
            //                {
            //                    if (ChildCategory == 0)
            //                    {
            //                        //Nu exista alte subcategorii
            //                        //announcements.Where(m => m.Category.Id == idCategory.Value);
            //                        announcements = (from m in db.Announcements
            //                                         where (m.Title.Contains(search) || m.Description.Contains(search))
            //                                         && m.Address.CityId.Value == idCity && m.Address.CityId.HasValue == true
            //                                         && m.CategoryId.Value == idCategory.Value
            //                                         select m);
            //                    }
            //                    else
            //                    {
            //                        //trebuie sa cautam in toate subcategoriile acelei categorii
            //                        List<int> idCategories = (from m in db.Categories
            //                                                  where m.ParentId == idCategory.Value
            //                                                  select m.Id).ToList<int>();
            //                        //announcements.Where(m => idCategories.Contains(m.Category.Id));
            //                        announcements = (from m in db.Announcements
            //                                         where (m.Title.Contains(search) || m.Description.Contains(search))
            //                                         && m.Address.CityId.Value == idCity && m.Address.CityId.HasValue == true
            //                                         && idCategories.Contains(m.CategoryId.Value)
            //                                         select m);
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            //suntem in situatia in care nu mergem pe toate categorile 
            //            if(ExistCategory == true)
            //            {
            //                //Suntem in situatia in care doar mergem pentru categorie
            //                if (idCategory.HasValue)
            //                {
            //                    if (ChildCategory == 0)
            //                    {
            //                        //Nu exista alte subcategorii
            //                        //announcements.Where(m => m.Category.Id == idCategory.Value);
            //                        announcements = (from m in db.Announcements
            //                                         where (m.Title.Contains(search) || m.Description.Contains(search))
            //                                         && m.CategoryId.Value == idCategory.Value
            //                                         select m);
            //                    }
            //                    else
            //                    {
            //                        //trebuie sa cautam in toate subcategoriile acelei categorii
            //                        List<int> idCategories = (from m in db.Categories
            //                                                  where m.ParentId == idCategory.Value
            //                                                  select m.Id).ToList<int>();
            //                        //announcements.Where(m => idCategories.Contains(m.Category.Id));
            //                        announcements = (from m in db.Announcements
            //                                         where (m.Title.Contains(search) || m.Description.Contains(search))                                                     
            //                                         && idCategories.Contains(m.CategoryId.Value)
            //                                         select m);
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                if(ExistCitie == true)
            //                {
            //                    //Suntem in situatia in care trebuie sa cautam doar dupa oras
            //                    if (IdCity.Count > 0)
            //                    {
            //                        //announcements.Where(m => IdCity.Contains(m.Address.CityId.Value) && m.Address.CityId.HasValue == true);
            //                        announcements = (from m in db.Announcements
            //                                         where (m.Title.Contains(search) || m.Description.Contains(search))
            //                                         && IdCity.Contains(m.Address.CityId.Value) && m.Address.CityId.HasValue == true                                                     
            //                                         select m);

            //                    }
            //                    if (idCity.HasValue)
            //                    {
            //                        //announcements.Where(m => m.Address.CityId.Value == idCity && m.Address.CityId.HasValue == true);
            //                        announcements = (from m in db.Announcements
            //                                         where (m.Title.Contains(search) || m.Description.Contains(search))
            //                                         && m.Address.CityId.Value == idCity && m.Address.CityId.HasValue == true                                                    
            //                                         select m);
            //                    }
            //                }
            //                else
            //                {
            //                    //Suntem in situatia in care cautam doar dupa titlu sau descriere
            //                    announcements = (from m in db.Announcements
            //                                     where (m.Title.Contains(search) || m.Description.Contains(search))                                                 
            //                                     select m);
            //                }
            //            }
            //        }

            //    }
            //    else
            //    {
            //        //Suntem in sutiatia cand nu trebuie sa cautam dupa titlu sau descriere
            //        if (ExistCategory == true && ExistCitie == true)
            //        {
            //            //Suntem in situatia cand cautam dupa toate criteriile
            //            if (IdCity.Count > 0)
            //            {
            //                announcements.Where(m => IdCity.Contains(m.Address.CityId.Value) && m.Address.CityId.HasValue == true);
            //                if (idCategory.HasValue)
            //                {
            //                    if (ChildCategory == 0)
            //                    {
            //                        //Nu exista alte subcategorii
            //                        //announcements.Where(m => m.Category.Id == idCategory.Value);
            //                        announcements = (from m in db.Announcements
            //                                         where IdCity.Contains(m.Address.CityId.Value) && m.Address.CityId.HasValue == true
            //                                         && m.CategoryId.Value == idCategory.Value
            //                                         select m);
            //                    }
            //                    else
            //                    {
            //                        //trebuie sa cautam in toate subcategoriile acelei categorii
            //                        List<int> idCategories = (from m in db.Categories
            //                                                  where m.ParentId == idCategory.Value
            //                                                  select m.Id).ToList<int>();
            //                        //announcements.Where(m => idCategories.Contains(m.Category.Id));
            //                        announcements = (from m in db.Announcements
            //                                         where  IdCity.Contains(m.Address.CityId.Value) && m.Address.CityId.HasValue == true
            //                                         && idCategories.Contains(m.CategoryId.Value)
            //                                         select m);
            //                    }
            //                }

            //            }
            //            if (idCity.HasValue)
            //            {
            //                announcements.Where(m => m.Address.CityId.Value == idCity && m.Address.CityId.HasValue == true);
            //                if (idCategory.HasValue)
            //                {
            //                    if (ChildCategory == 0)
            //                    {
            //                        //Nu exista alte subcategorii
            //                        //announcements.Where(m => m.Category.Id == idCategory.Value);
            //                        announcements = (from m in db.Announcements
            //                                         where m.Address.CityId.Value == idCity && m.Address.CityId.HasValue == true
            //                                         && m.CategoryId.Value == idCategory.Value
            //                                         select m);
            //                    }
            //                    else
            //                    {
            //                        //trebuie sa cautam in toate subcategoriile acelei categorii
            //                        List<int> idCategories = (from m in db.Categories
            //                                                  where m.ParentId == idCategory.Value
            //                                                  select m.Id).ToList<int>();
            //                        //announcements.Where(m => idCategories.Contains(m.Category.Id));
            //                        announcements = (from m in db.Announcements
            //                                         where m.Address.CityId.Value == idCity && m.Address.CityId.HasValue == true
            //                                         && idCategories.Contains(m.CategoryId.Value)
            //                                         select m);
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            //suntem in situatia in care nu mergem pe toate categorile 
            //            if (ExistCategory == true)
            //            {
            //                //Suntem in situatia in care doar mergem pentru categorie
            //                if (idCategory.HasValue)
            //                {
            //                    if (ChildCategory == 0)
            //                    {
            //                        //Nu exista alte subcategorii
            //                        //announcements.Where(m => m.Category.Id == idCategory.Value);
            //                        announcements = (from m in db.Announcements
            //                                         where m.CategoryId.Value == idCategory.Value
            //                                         select m);
            //                    }
            //                    else
            //                    {
            //                        //trebuie sa cautam in toate subcategoriile acelei categorii
            //                        List<int> idCategories = (from m in db.Categories
            //                                                  where m.ParentId == idCategory.Value
            //                                                  select m.Id).ToList<int>();
            //                        //announcements.Where(m => idCategories.Contains(m.Category.Id));
            //                        announcements = (from m in db.Announcements
            //                                         where idCategories.Contains(m.CategoryId.Value)
            //                                         select m);
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                if (ExistCitie == true)
            //                {
            //                    //Suntem in situatia in care trebuie sa cautam doar dupa oras
            //                    if (IdCity.Count > 0)
            //                    {
            //                        //announcements.Where(m => IdCity.Contains(m.Address.CityId.Value) && m.Address.CityId.HasValue == true);
            //                        announcements = (from m in db.Announcements
            //                                         where IdCity.Contains(m.Address.CityId.Value) && m.Address.CityId.HasValue == true
            //                                         select m);

            //                    }
            //                    if (idCity.HasValue)
            //                    {
            //                        //announcements.Where(m => m.Address.CityId.Value == idCity && m.Address.CityId.HasValue == true);
            //                        announcements = (from m in db.Announcements
            //                                         where m.Address.CityId.Value == idCity && m.Address.CityId.HasValue == true
            //                                         select m);
            //                    }
            //                }
            //                else
            //                {
                                
            //                }
            //            }
            //        }
            //    }
            //    foreach(var item in announcements)
            //    {
            //        Announcement da = new Announcement();
            //        List<Image> Img = new List<Image>();
            //        foreach(var img in item.Images)
            //        {
            //            Img.Add(img);
            //        }
            //        da = item;
            //        da.Images = Img;
            //        ann.Add(da);                    
            //    }

            //}

            #endregion

            return View(ann);
                 
            
        }
        public ActionResult Index()
        {
            return View();
        }
        //
        // GET: /Announcement/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        //
        // GET: /Announcement/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }
        //
        // POST: /Announcement/Create
        [HttpPost]
        public ActionResult Create(Announcement announcement)
        {
            //Adaugat 03.03.2015

            string[] category = Request.Form["inputCategori"].ToString().Split('>');
            
            //End Adaugare 03.03.2015
            using (var db = new EntitiesAlx())
            {
                var citi = Request.Form["City.CityName"].ToString();
                string citiName = citi.Split(',')[0].ToUpper();
                IQueryable<Int32> IdCity = from m in db.Cities
                                           where m.CityName.ToUpper().Contains(citiName)
                                            select m.Id;
                List<Int32> a = IdCity.ToList<Int32>();
                foreach (var i in IdCity)
                {
                    announcement.Address.CityId = Int32.Parse(i.ToString());
                    break;
                }

                //Adaugat 03.03.2015
                int? idCategory = null;
                if (category.Length > 0 && category[0] != "")
                {
                    string name = category[category.Length - 1].ToUpper().Trim();
                    int? countCategory = (from m in db.Categories
                                          where m.Name.ToUpper() == name
                                          select m.ParentId).Count<int>();
                    if (countCategory.HasValue == true && countCategory.Value == 1)
                    {
                        //Categoria este unica
                        idCategory = (from m in db.Categories
                                      where m.Name.ToUpper() == name
                                      select m.Id).SingleOrDefault<int>();                        
                    }
                    else
                    {
                        //Categoria nu este unica
                        string name1 = category[category.Length - 2].Trim().ToUpper();
                        idCategory = (from m in db.Categories
                                      where m.Name.ToUpper() == name
                                      && m.ParentId == (from n in db.Categories
                                                        where n.Name.ToUpper() == name1
                                                        select n.Id).FirstOrDefault<int>()
                                      select m.Id
                                          ).SingleOrDefault<int>();                        
                    }
                }

                if (idCategory.HasValue)
                {                    
                    announcement.CategoryId = idCategory.Value;
                }

                //End Adaugare 03.03.2015
                //announcement.Address.CityId = IdCity;

                for (int i = 0; i < Request.Files.Count && i < 8;i++ )
                {

                    HttpPostedFileBase hpf = Request.Files[i] as HttpPostedFileBase;
                    if (hpf.ContentLength != 0)
                    {
                        byte[] array = new byte[hpf.ContentLength];

                        hpf.InputStream.Read(array, 0, hpf.ContentLength);
                        Alx.Models.Image img = new Alx.Models.Image();
                        img.Name = hpf.FileName;
                        img.PhysicalImage = array;
                        announcement.Images.Add(img);
                    }
                    else
                    {
                        return View("ImportFailed");
                    }
                }

                if (Session["UserId"] != null)
                {
                    announcement.UserId = Int32.Parse(Session["UserId"].ToString());
                }
                else
                {
                    if(Request.IsAuthenticated)
                    {
                        announcement.UserId = WebSecurity.GetUserId(User.Identity.Name);
                        Session["UserId"] = announcement.UserId;
                    }
                }

                announcement.DateAnnouncement = System.DateTime.Now;

                db.Announcements.Add(announcement);
                db.SaveChanges();
            }
            try
            {
                // TODO: Add insert logic here
                return RedirectToAction("Index","Home");
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public void SendEmail(FormCollection collection)
        {
            Int32 UserId = 0;
            string NrAnunt = collection["IdAnnouncement"].ToString();
            string title = collection["TitleAnnouncement"].ToString();
            string UserEmail = "alexandru.dumitraiche@gmail.com";
            string message = collection["MessageEmail"].ToString();
            try
            {
                UserId = Int32.Parse(collection["UserId"].ToString().Trim());
            }
            catch
            {
            }
            
            using (var db = new UsersContext())
            {
                IQueryable<string> userProfile = db.UserProfiles.Where(m => m.UserId == UserId).Select(m => m.UserName);
                UserEmail = userProfile.SingleOrDefault<string>();
            }
            if (UserEmail == null || UserEmail == "")
            {
                UserEmail = "alexandru.dumitraiche@gmail.com";
            }
            using(var db = new EntitiesAlx())
            {
                
                string subject = "Anuntul cu numarul: "+NrAnunt+" cu titlul: "+title;
                string body = message.Replace("\n","<br />").Replace("\r","");
                
                //email.SendEmailPrivateGmail(UserEmail, "bby94z@gmail.com", subject, body);

                Message Message = new Message();
                if(Request.IsAuthenticated)
                {
                    Message.FromUserId = WebSecurity.GetUserId(User.Identity.Name);
                    Message.FromUserName = User.Identity.Name;
                }
                else
                {
                    Message.FromUserId = 0;
                    Message.FromUserName = collection["FromEmail"].ToString();
                }

                email.SendEmailPrivateGmail(UserEmail, Message.FromUserName, subject, body);
                Message.ToUserId = UserId;
                Message.ToUserName = UserEmail;
                Message.Body = body;
                Message.Subject = subject;
                Message.DateCreate = System.DateTime.Now;
                try
                {
                    Message.AnnouncementId = Int32.Parse(NrAnunt.Trim());
                }
                catch
                {

                }
                db.Messages.Add(Message);
                db.SaveChanges();
            }
        }
        //
        // GET: /Announcement/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }
        //
        // POST: /Announcement/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        //
        // GET: /Announcement/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }
        //
        // POST: /Announcement/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public JsonResult Address()
        {
            List<string> adr = new List<string>();
            using(var db = new EntitiesAlx())
            {
                var select = from counties in db.Counties
                             join cities in db.Cities on counties.Id equals cities.County.Id
                             select new { counties.CountyName, cities.CityName };
                foreach(var item in select)
                {
                    adr.Add(item.CityName + ", " + item.CountyName); 
                }
            }

            return Json(adr, JsonRequestBehavior.AllowGet);
        }
    }
}
