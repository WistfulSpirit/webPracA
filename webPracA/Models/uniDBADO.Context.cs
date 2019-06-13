﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace webPracA.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class uniDBEntities : DbContext
    {
        public uniDBEntities()
            : base("name=uniDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ExamPlan> ExamPlan { get; set; }
        public virtual DbSet<ExamResult> ExamResult { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<Lesson> Lesson { get; set; }
        public virtual DbSet<Mark> Mark { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Teacher> Teacher { get; set; }
    
        public virtual ObjectResult<GetExamResults_Result> GetExamResults(Nullable<int> lId, Nullable<int> gId)
        {
            var lIdParameter = lId.HasValue ?
                new ObjectParameter("lId", lId) :
                new ObjectParameter("lId", typeof(int));
    
            var gIdParameter = gId.HasValue ?
                new ObjectParameter("gId", gId) :
                new ObjectParameter("gId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetExamResults_Result>("GetExamResults", lIdParameter, gIdParameter);
        }
    
        public virtual ObjectResult<LoginByUP_Result> LoginByUP(string username, string password)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LoginByUP_Result>("LoginByUP", usernameParameter, passwordParameter);
        }
    
        public virtual ObjectResult<GetResultsAndAvg_Result> GetResultsAndAvg(Nullable<int> gId)
        {
            var gIdParameter = gId.HasValue ?
                new ObjectParameter("gId", gId) :
                new ObjectParameter("gId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetResultsAndAvg_Result>("GetResultsAndAvg", gIdParameter);
        }
    }
}
