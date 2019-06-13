using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace webPracA.Models
{
    [MetadataType(typeof(StudentMetadata))]
    public partial class Student
    {
    }
    [MetadataType(typeof(ExamPlanMetadata))]
    public partial class ExamPlan
    {
    }
    [MetadataType(typeof(TeacherMetadata))]
    public partial class Teacher
    {
    }
    [MetadataType(typeof(GroupMetadata))]
    public partial class Group
    {
    }
    [MetadataType(typeof(LessonMetadata))]
    public partial class Lesson
    {
    }
}