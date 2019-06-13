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
    [Authorize]
    public class ExamPlansController : Controller
    {
        private uniDBEntities db = new uniDBEntities();

        // GET: ExamPlans
        public ActionResult Index(string searchString)
        {

            var examPlan = db.ExamPlan.Include(e => e.Group).Include(e => e.Lesson).Include(e => e.Teacher);
            if (User.Identity.Name != "admin")
            {
                int TT = Convert.ToInt32(User.Identity.Name);
                examPlan = examPlan.Where(e => e.TeacherId == TT);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                examPlan = examPlan.Where(s => s.Group.Number.Contains(searchString));
            }
            examPlan = examPlan.OrderBy(ep => ep.Group.Number);
            return View(examPlan.ToList());
        }

        // GET: ExamPlans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamPlan examPlan = db.ExamPlan.Find(id);
            if (examPlan == null)
            {
                return HttpNotFound();
            }
            return View(examPlan);
        }

        // GET: ExamPlans/Create
        public ActionResult Create()
        {
            ViewBag.GroupId = new SelectList(db.Group, "Id", "Number");
            ViewBag.LessonId = new SelectList(db.Lesson, "Id", "Name");
            ViewBag.TeacherId = new SelectList(db.Teacher, "Id", "Name");
            return View();
        }

        // POST: ExamPlans/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LessonId,GroupId,TeacherId,ExamDate")] ExamPlan examPlan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.ExamPlan.Add(examPlan);
                    db.SaveChanges();
                    UpdateResults(examPlan.Id, examPlan.GroupId);
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ViewBag.LessonId = new SelectList(db.Lesson, "Id", "Name", examPlan.LessonId);
                ViewBag.GroupId = new SelectList(db.Group, "Id", "Number", examPlan.GroupId);
                ViewBag.TeacherId = new SelectList(db.Teacher, "Id", "Name", examPlan.TeacherId);
                ModelState.AddModelError("", "Такой экзамен уже существует!");
                return View(examPlan);
            }
            return View(examPlan);
        }

        private void UpdateResults(int explnId, int groupId)
        {
            foreach (var student in db.Student.Where(s => s.GroupId == groupId))
            {
                ExamResult exr = new ExamResult();
                exr.ExamPlanId = explnId;
                exr.StudentId = student.Id;
                exr.MarkVal = null;
                db.ExamResult.Add(exr);
            }
            db.SaveChanges();
        }

        // GET: ExamPlans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamPlan examPlan = db.ExamPlan.Find(id);
            if (examPlan == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupId = new SelectList(db.Group, "Id", "Number", examPlan.GroupId);
            ViewBag.LessonId = new SelectList(db.Lesson, "Id", "Name", examPlan.LessonId);
            ViewBag.TeacherId = new SelectList(db.Teacher, "Id", "Name", examPlan.TeacherId);
            return View(examPlan);
        }

        // POST: ExamPlans/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LessonId,GroupId,TeacherId,ExamDate")] ExamPlan examPlan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(examPlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupId = new SelectList(db.Group, "Id", "Number", examPlan.GroupId);
            ViewBag.LessonId = new SelectList(db.Lesson, "Id", "Name", examPlan.LessonId);
            ViewBag.TeacherId = new SelectList(db.Teacher, "Id", "Name", examPlan.TeacherId);
            return View(examPlan);
        }

        // GET: ExamPlans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamPlan examPlan = db.ExamPlan.Find(id);
            if (examPlan == null)
            {
                return HttpNotFound();
            }
            return View(examPlan);
        }

        // POST: ExamPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExamPlan examPlan = db.ExamPlan.Find(id);
            foreach (var res in db.ExamResult.Where(e => e.ExamPlanId == id))
                db.ExamResult.Remove(res);
            db.ExamPlan.Remove(examPlan);
            db.SaveChanges();
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
