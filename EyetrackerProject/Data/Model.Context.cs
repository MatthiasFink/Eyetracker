﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EyetrackerEntities : DbContext
    {
        public EyetrackerEntities()
            : base("name=EyetrackerEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Answer> Answer { get; set; }
        public virtual DbSet<Candidate> Candidate { get; set; }
        public virtual DbSet<Choice> Choice { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<Questionnaire> Questionnaire { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<Slide> Slide { get; set; }
        public virtual DbSet<Slide_Answer> Slide_Answer { get; set; }
        public virtual DbSet<Slide_Choice> Slide_Choice { get; set; }
        public virtual DbSet<Test> Test { get; set; }
        public virtual DbSet<Test_Definition> Test_Definition { get; set; }
        public virtual DbSet<Tracking> Tracking { get; set; }
    }
}