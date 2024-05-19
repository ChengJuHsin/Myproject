using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HomeWork325.Models;

namespace HomeWork325.Controllers
{
    public class C325Controller : Controller
    {
        C11010325Entities mydb = new C11010325Entities();
        // GET: C325
        public ActionResult Index()
        {
            return View(mydb.product.ToList());
        }
        public ActionResult orderlist()
        {
            return View(mydb.OrderList.ToList());
        }
        public ActionResult customlist()
        {
            return View(mydb.custom.ToList());
        }
        public ActionResult NewRecord() //create
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewRecord(custom p)
        {
            if (ModelState.IsValid) //model是有效
            {
                p.cCard = 1;
                mydb.custom.Add(p); //將p載入資料庫
                mydb.SaveChanges(); //存檔
                return RedirectToAction("index"); //回到index
            }
            return View();
        }
        public ActionResult Edit(int? id)
        {
        
            var p = mydb.custom.Find(id); //找id，移除remove


            return View(p);
        }
        [HttpPost]
        public ActionResult Edit(custom c)
        {
            if (!ModelState.IsValid)
            {
                return View(c);
            }
            if (ModelState.IsValid) //model是有效
            {
                var temp = mydb.custom
                    .Where(m => m.Id == c.Id)
                    .FirstOrDefault();
                temp.cName = c.cName;
                temp.cAccount = c.cAccount;
                temp.cCard = c.cCard;
                temp.cPasswd = c.cPasswd;
                mydb.SaveChanges();
                return RedirectToAction("customlist"); //回到index
            }
            return View(c);
        }
        public ActionResult Delete(int id)
        {
            var temp = mydb.custom
                .Where(m => m.Id == id)
                .FirstOrDefault();
            mydb.custom.Remove(temp);
            mydb.SaveChanges();
            return RedirectToAction("customlist");
        }
        public ActionResult orderlistAllName()
        {

            return View(mydb.custom.ToList());
        }
        [HttpPost]
        public ActionResult oderlistB(string select)
        {
            ViewBag.select = select;
            return View(mydb.OrderList.ToList());
        }
    }
}