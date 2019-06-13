using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using webPracA.Models;
using Excel = Microsoft.Office.Interop.Excel;
/*
 CREATE PROCEDURE [dbo].[GetExamResults]
	@lId int,
	@gNum nvarchar(50)
AS  
	SELECT  Lesson.[Name] as LessonName, Teacher.[Name] as TeacherName, ExamPlan.GroupId, Student.[Name] as StudentName, ExamResult.MarkVal, ExamPlan.ExamDate FROM ExamPlan
	INNER JOIN ExamResult ON ExamResult.ExamPlanId = ExamPlan.Id
	INNER JOIN Student ON Student.GroupId = ExamPlan.GroupId
	INNER JOIN Lesson ON Lesson.Id = ExamPlan.LessonId
	INNER JOIN Teacher ON Teacher.Id = ExamPlan.TeacherId
	WHERE ExamPlan.LessonId = @lId AND ExamPlan.GroupId = @gNum
	ORDER BY Student.[Name]
RETURN 0
     */

namespace webPracA.Controllers
{
    public class ExamResultsController : Controller
    {
        private uniDBEntities db = new uniDBEntities();

        public ActionResult Export(int? gId = null, int? lId = null)
        {
            /*
             Lesson.[Name] as LessonName,
             Teacher.[Name] as TeacherName,
             [Group].Number as GroupNum,
             Student.[Name] as StudentName,
             ExamResult.MarkVal, 
			 Mark.[Name] as MarkName,
             ExamPlan.ExamDate,
             */

            if (gId == null || lId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var examResult = db.GetExamResults(gId, lId).OrderBy(g => g.StudentName).ToList();
            Application exApp = new Application();
            exApp.Visible = true;
            exApp.Workbooks.Add();
            Worksheet workSheet = (Worksheet)exApp.ActiveSheet;

            Range range = workSheet.get_Range("A1", "G1").Cells;
            range.Merge(Type.Missing);
            workSheet.Cells[1, 1] = "Преподаватель - " + examResult[0].TeacherName;

            range = workSheet.get_Range("A2", "G2").Cells;
            range.Merge(Type.Missing);
            workSheet.Cells[2, 1] = "Название предмета - " + examResult[0].LessonName;

            workSheet.Cells[3, 1] = "Имя студента";
            workSheet.Cells[3, 2] = "Оценка";
            int rowExcel = 4;
            for (int i = 0; i < examResult.Count(); i++)
            {
                workSheet.Cells[rowExcel, "A"] = examResult[i].StudentName;
                workSheet.Cells[rowExcel, "B"] = String.Format("{0}({1})", examResult[i].MarkVal, examResult[i].MarkName);
                ++rowExcel;
            }
            workSheet.SaveAs("MyExcel.xls");
            exApp.Quit();
            return RedirectToAction("Index", "ExamPlans");
        }

        // GET: ExamResults
        public ActionResult Index(int? gId = null, int? lId = null, string sortOrder = null)
        {
            if (User.Identity.Name == "admin")
                SetLists(gId, lId);
            else
                SetLists(Convert.ToInt32(User.Identity.Name), gId, lId);
            int gn = (int)((SelectList)ViewBag.GroupList).SelectedValue;//groupId
            int li = (int)((SelectList)ViewBag.LessonList).SelectedValue;//lessonId

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.MarkSortParm = sortOrder == "Mark" ? "mark_desc" : "Mark";

            var examResult = db.GetExamResults(li, gn).OrderBy(g => g.StudentName);
            switch (sortOrder)
            {
                case "name_desc":
                    examResult = examResult.OrderByDescending(s => s.StudentName);
                    break;
                case "Mark":
                    examResult = examResult.OrderBy(s => s.MarkVal);
                    break;
                case "mark_desc":
                    examResult = examResult.OrderByDescending(s => s.MarkVal);
                    break;
                default:
                    examResult = examResult.OrderBy(s => s.StudentName);
                    break;
            }


            ViewData["LN"] = db.Lesson.Where(l => l.Id == lId).ToList()[0].Name;
            ViewData["TN"] = db.ExamPlan.Include(e => e.Teacher).Where(ep => ep.GroupId == gId && ep.LessonId == lId).ToList()[0].Teacher.Name;
            //System.Diagnostics.Debug.WriteLine("-----HERE COMES THE GROUPLIST-----");
            return View(examResult.ToList());
        }

        #region teacherViews
        private void SetLists(int tId, int? gId, int? lId)
        {
            var query = from expln in db.ExamPlan
                        join lesson in db.Lesson on expln.LessonId equals lesson.Id
                        join grp in db.Group on expln.GroupId equals grp.Id
                        where expln.TeacherId == tId
                        select new { Id = lesson.Id, Name = lesson.Name, grpId = grp.Id, Number = grp.Number };
            /*SELECT Lesson.Id as Id, Lesson.[Name] as Name, [Group].Number as Number
              FROM ExamPlan
              JOIN Lesson on ExamPlan.LessonId = Lesson.Id
              JOIN[Group] on ExamPlan.GroupId = [Group].Number
              where ExamPlan.TeacherId = 5
            */
            lId = lId ?? query.ToList()[0].Id;
            gId = gId ?? query.ToList()[0].Id;

            //foreach (var dick in query.Distinct().ToList())
            //    System.Diagnostics.Debug.WriteLine("this is dick --- " + dick);
            ViewBag.LessonList = new SelectList(query.OrderBy(e => e.Name).Distinct(), "Id", "Name", lId);
            ViewBag.GroupList = new SelectList(query.GroupBy(q => q.Number).Select(q => q.FirstOrDefault()), "grpId", "Number", gId);
            //ViewBag.LessonList = new SelectList(db.Lesson.Where(l => db.ExamPlan.Select(e => e.LessonId).Contains(l.Id)), "Id", "Name", lId);
        }

        //ERROR 500 
        public ActionResult GetGroupList(int tId, int lId)
        {
            var query = from expln in db.ExamPlan
                        where expln.TeacherId == tId && expln.LessonId == lId
                        orderby expln.GroupId
                        select new { Id = expln.GroupId, Number = expln.Group.Number };

            ViewBag.GroupList = new SelectList(query, "Id", "Number", query.ToList()[0].Number);
            return PartialView();
        }

        public ActionResult ShowExamListT(int tId, int? GroupList, int? LessonList, string sortOrder = null)
        {
            var examResult = db.GetExamResults(LessonList, GroupList).Where(g => g.RTeacherId == tId);
            ViewData["LN"] = db.Lesson.Where(l => l.Id == LessonList).ToList()[0].Name;
            ViewData["TN"] = db.ExamPlan.Include(e => e.Teacher).Where(ep => ep.GroupId == GroupList && ep.LessonId == LessonList).ToList()[0].Teacher.Name;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.MarkSortParm = sortOrder == "Mark" ? "mark_desc" : "Mark";
            ViewBag.GL = GroupList;
            ViewBag.LL = LessonList;
            return PartialView(examResult.ToList());
        }
        #endregion

        #region adminViews
        private void SetLists(int? gId, int? lId)
        {
            var query = from lesson in db.Lesson
                        join expln in db.ExamPlan on lesson.Id equals expln.LessonId
                        where expln.GroupId == gId
                        orderby lesson.Name
                        select new { Id = lesson.Id, Name = lesson.Name };
            gId = gId ?? db.Group.Select(g => g.Id).ToList()[0];
            lId = lId ?? query.ToList()[0].Id;
            ViewData["LN"] = db.Lesson.Where(l => l.Id == lId).ToList()[0].Name;
            ViewData["TN"] = db.ExamPlan.Include(e => e.Teacher).Where(ep => ep.GroupId == gId && ep.LessonId == lId).ToList()[0].Teacher.Name;
            ViewBag.GroupList = new SelectList(db.Group.Where(g => db.ExamPlan.Select(e => e.GroupId).Contains(g.Id)).OrderBy(e => e.Number), "Id", "Number", gId);
            ViewBag.LessonList = new SelectList(query, "Id", "Name", lId);
            //ViewBag.LessonList = new SelectList(db.Lesson.Where(l => db.ExamPlan.Select(e => e.LessonId).Contains(l.Id)), "Id", "Name", lId);
        }

        public ActionResult GetLessonList(int? gId)
        {
            var query = from lesson in db.Lesson
                        join expln in db.ExamPlan on lesson.Id equals expln.LessonId
                        where expln.GroupId == gId
                        orderby lesson.Name
                        select new { Id = lesson.Id, Name = lesson.Name };
            ViewBag.LessonList = new SelectList(query, "Id", "Name", 1);
            return PartialView();
        }

        public ActionResult ShowExamList(int? GroupList, int? LessonList, string sortOrder = null)
        {
            ViewData["LN"] = db.Lesson.Where(l => l.Id == LessonList).ToList()[0].Name;
            ViewData["TN"] = db.ExamPlan.Include(e => e.Teacher).Where(ep => ep.GroupId == GroupList && ep.LessonId == LessonList).ToList()[0].Teacher.Name;
            var examResult = db.GetExamResults(LessonList, GroupList).OrderBy(g => g.StudentName);
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.MarkSortParm = sortOrder == "Mark" ? "mark_desc" : "Mark";
            ViewBag.GL = GroupList;
            ViewBag.LL = LessonList;

            return PartialView(examResult.ToList());
        }
        #endregion

        // GET: ExamResults/Details/5
        public ActionResult Details(int? pId, int? sId)
        {
            if (pId == null || sId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamResult examResult = db.ExamResult.Find(pId, sId);
            if (examResult == null)
            {
                return HttpNotFound();
            }
            return View(examResult);
        }

        // GET: ExamResults/Edit/5
        public ActionResult Edit(int? pId, int? sId)
        {
            if (pId == null || sId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamResult examResult = db.ExamResult.Find(pId, sId);
            if (examResult == null)
            {
                return HttpNotFound();
            }
            ViewBag.ExamPlanId = new SelectList(db.ExamPlan, "Id", "GroupId", examResult.ExamPlanId);
            ViewBag.MarkVal = new SelectList(db.Mark, "Value", "Name", examResult.MarkVal);
            ViewBag.StudentId = new SelectList(db.Student, "Id", "Name", examResult.StudentId);
            return View(examResult);
        }

        // POST: ExamResults/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExamPlanId,StudentId,MarkVal")] ExamResult examResult)
        {
            if (ModelState.IsValid)
            {
                var tp = db.ExamPlan.Where(ep => ep.Id == examResult.ExamPlanId).ToList()[0];
                db.Entry(examResult).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { gId = tp.GroupId, lId = tp.LessonId });
            }
            ViewBag.ExamPlanId = new SelectList(db.ExamPlan, "Id", "GroupId", examResult.ExamPlanId);
            ViewBag.MarkVal = new SelectList(db.Mark, "Value", "Name", examResult.MarkVal);
            ViewBag.StudentId = new SelectList(db.Student, "Id", "Name", examResult.StudentId);
            return View(examResult);
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
