using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Alx.Models;
using System.Data.Linq.SqlClient;

namespace Alx.Controllers
{
    public class CategorieController : Controller
    {

        class Cate
        {
            public string Name { get; set; }
            public int Count { get; set; }

        }
        public JsonResult Categories(string name)
        {
            List<string> Val = new List<string>();
            List<Cate> list = new List<Cate>();
            using(var db = new EntitiesAlx())
            {
                
                if (name == "0")
                {
                    var select = (from m in db.Categories
                                  where m.ParentId == 0
                                  select m.Name);

                    Val = select.ToList<string>();
                }
                else
                {
                    var select = (from m in db.Categories
                                  join d in db.Categories on m.ParentId equals d.Id
                                  where d.Name.ToUpper() == name.ToUpper()
                                  select m.Name);
                    Val = select.ToList<string>();
                }
            }
            using(var db = new EntitiesAlx())
            {
                for (int i = 0; i < Val.Count; i++)
                {

                    name = Val[i];

                    var select = (from m in db.Categories
                                  join d in db.Categories on m.ParentId equals d.Id
                                  where d.Name.ToUpper() == name.ToUpper()
                                  select m.Id).Count<int>();
                    
                    list.Add(new Cate()
                    {
                        Name = Val[i],
                        Count = select
                    });
                }

            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Categorie/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Categorie/Details/5

        public JsonResult Details(int id)
        {
            List<Categories> list = new List<Categories>();

            using(var db = new EntitiesAlx())
            {
                IQueryable<Categories> category = db.Categories.Where(m => m.ParentId == id)
                    .Select(m => new Categories() 
                    {   Id = m.Id ,
                        Name =  m.Name,
                        Count = db.Categories.Where(n => n.ParentId == m.Id).Select(n => n.Id).Count<int>()
                    });

                list = category.ToList<Categories>();

            }

            return Json(list, JsonRequestBehavior.AllowGet); 
        }

        //
        // GET: /Categorie/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Categorie/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Categorie/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Categorie/Edit/5

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
        // GET: /Categorie/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Categorie/Delete/5

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
    }
}
