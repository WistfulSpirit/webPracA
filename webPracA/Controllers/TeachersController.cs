using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using webPracA.Models;

namespace webPracA.Controllers
{
    public class TeachersController : Controller
    {
        private uniDBEntities db = new uniDBEntities();

        // GET: Teachers
        public ActionResult Index()
        {
            return View(db.Teacher.ToList());
        }
/*        public ActionResult Login()
        {
            return View();
        }

        //could be done with ASP.NET Identity || Membership API || OWIN или я всё путаю, но в общем можно сделать намного лучше и красивее 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login u)
        {
            if (ModelState.IsValid)
            {
                var v = db.Teacher.Where(t => t.Login.Equals(u.UserLogin) && t.Password.Equals(u.Password)).FirstOrDefault();
                if (v != null)
                {
                    Session["LoggedTeacherId"] = v.Id;
                    Session["LoggedTeacherName"] = v.Name;
                    return RedirectToAction("AfterLogin");
                }
                else if (u.UserLogin.Equals("admin") && u.Password.Equals("admin"))
                {
                    Session["LoggedTeacherId"] = u.UserLogin;
                    Session["LoggedTeacherName"] = u.Password;
                    return RedirectToAction("AfterLogin");
                }
                else
                {
                    ModelState.AddModelError("", "Неверный логин или пароль!");
                }
            }
            return View(u);
        }

        public ActionResult AfterLogin()
        {
            if (Session["LoggedTeacherId"].Equals("admin"))
            {
                return RedirectToRoute(new { controller = "ExamResults", action = "Index" });
            }
            if (Session["LoggedTeacherId"] != null)
            {
                return RedirectToAction("ExamResults", "Index", new { id = Session["LoggedTeacherId"] });
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Logout()
        {

            if (Session["LoggedTeacherId"] != null)
            {
                Session["LoggedTeacherId"] = null;
            }
            if (Session["LoggedTeacherName"] != null)
            {
                Session["LoggedTeacherName"] = null;
            }
            return RedirectToAction("Login");
        }
        */
        // GET: Teachers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teacher.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // GET: Teachers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Login,Password")] Teacher teacher, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        teacher.Image = reader.ReadBytes(upload.ContentLength);
                    }
                }
                db.Teacher.Add(teacher);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(teacher);
        }


        // GET: Teachers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teacher.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Login,Password")] Teacher teacher, HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(teacher).State = EntityState.Modified;
                    if (upload != null && upload.ContentLength > 0)
                    {
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            teacher.Image = reader.ReadBytes(upload.ContentLength);
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(teacher).Property(m => m.Image).IsModified = false;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

                return View(teacher);
            }
            catch (Exception e) { }

            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teacher.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Teacher teacher = db.Teacher.Find(id);
                db.Teacher.Remove(teacher);
                db.SaveChanges();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Нельзя удалить");
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
