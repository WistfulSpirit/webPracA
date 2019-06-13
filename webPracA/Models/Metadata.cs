using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace webPracA.Models
{
    public class StudentMetadata
    {
        [StringLength(150)]
        [Display(Name = "ФИО")]
        public string Name;
    }
    public class ExamPlanMetadata
    {
        [DataType(DataType.Date)]
        [Display(Name = "Дата проведения")]
        public string ExamDate;
    }
    public class TeacherMetadata
    {
        [StringLength(150)]
        [Display(Name = "Имя преподавателя")]
        public string Name;
        [StringLength(50)]
        [Display(Name = "Логин")]
        public string Login;
        [StringLength(32)]
        [Display(Name = "Пароль")]
        public string Password;
    }
    public class GroupMetadata
    {
        [StringLength(50)]
        [Display(Name = "Номер группы")]
        public string Number;
    }
    public class LessonMetadata
    {
        [StringLength(50)]
        [Display(Name = "Название предмета")]
        public string Name;
    }
}