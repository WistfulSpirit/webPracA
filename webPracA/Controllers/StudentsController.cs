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
    public class StudentsController : Controller
    {
        private uniDBEntities db = new uniDBEntities();

        // GET: Students
        public ActionResult Index(int? gId)//gId - Url Group Num
        {
            IQueryable<Student> student = null;
            if (gId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            else
                student = db.Student.Include(s => s.Group).Where(s => s.GroupId == gId);
            TempData["gId"] = gId;
            ViewBag.GRNUM = db.Group.Where(g => g.Id == gId).ToList()[0].Number;
            return View(student.ToList());
        }

        public ActionResult MarkRecords(int? gId)
        {
            if(gId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var res = db.GetResultsAndAvg(gId).OrderBy(r=>r.StudentName);
            ViewBag.sGR = db.Group.Where(g => g.Id == gId).First().Number;
            return View(res.ToList());
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create(int? gId)
        {
            ViewBag.GroupId = new SelectList(db.Group, "Id", "Number");
            return View();
        }

        // POST: Students/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,GroupId")] Student student)
        {

            if (ModelState.IsValid)
            {
                db.Student.Add(student);
                foreach (var exP in db.ExamPlan.Where(ep => ep.GroupId == student.GroupId))
                {
                    ExamResult er = new ExamResult();
                    er.ExamPlanId = exP.Id;
                    er.StudentId = student.Id;
                    er.MarkVal = null;
                    db.ExamResult.Add(er);
                }
                db.SaveChanges();
                return RedirectToAction("Index", new { gId = student.GroupId });
            }

            ViewBag.GroupId = new SelectList(db.Group, "Id", "Number", student.GroupId);
            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupId = new SelectList(db.Group, "Id", "Number", student.GroupId);
            return View(student);
        }

        // POST: Students/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,GroupId")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { gId = student.GroupId });
            }
            ViewBag.GroupId = new SelectList(db.Group, "Id", "Number", student.GroupId);
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Student.Find(id);
            int tempNum = student.GroupId;
            foreach (var exR in db.ExamResult.Where(er => er.StudentId == id))
                db.ExamResult.Remove(exR);
            db.Student.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index", new { gId = tempNum });
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
