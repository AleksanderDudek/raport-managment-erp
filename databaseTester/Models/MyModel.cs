namespace WebApplication1.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using SessionStates;
    using Klasy_Raport;

    public partial class MyModel : DbContext
    {
        //klasa posredniczaca, pozwala pobierac z bazy i do neij wysylac 
        public MyModel()
            : base("name=MyModel")
        {
        }
        //wbudowane w baze
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }

        //dostep do tabeli, ktora przypisuje user'owi jego sesje, aby mozna ja bylo odtworzyc
        public virtual DbSet<SessionUserCodeFirst> SessionUserCodeFirst { get; set; }
        //raporty przyporzadkowane do uzytkownikow i zawierajace wskazannia na konkretne zlomy
        public virtual DbSet<Raport> Raport { get; set; }
        //wszystkie wpisy ilosciowe 
        public virtual DbSet<ThrashType> ThrashType { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserRoles)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);
        }

        public System.Data.Entity.DbSet<WebApplication1.Models.TrashViewModel> TrashViewModels { get; set; }

        //public System.Data.Entity.DbSet<WebApplication1.Models.MyViewModel> MyViewModels { get; set; }
    }
}
