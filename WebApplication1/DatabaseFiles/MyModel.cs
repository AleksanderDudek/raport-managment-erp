namespace WebApplication1.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    //using SessionStates;
    using Klasy_Raport;
    using DatabaseFiles;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public partial class MyModel : DbContext
    {
        //klasa posredniczaca, pozwala pobierac z bazy i do neij wysylac 
        public MyModel()  : base("TomAppDB")
        {
        }
        //wbudowane w baze
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }

        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }

        //dostep do tabeli, ktora przypisuje user'owi jego sesje, aby mozna ja bylo odtworzyc
        //public virtual DbSet<SessionUserCodeFirst> SessionUserCodeFirst { get; set; }
        //raporty przyporzadkowane do uzytkownikow i zawierajace wskazannia na konkretne zlomy
        public virtual DbSet<Raport> Raport { get; set; }
        //wszystkie wpisy ilosciowe 
        public virtual DbSet<ThrashType> ThrashType { get; set; }

        public virtual DbSet<UserFrontInfo> UserFrontInfo { get; set; }

        public virtual DbSet<TrashPlaces> TrashPlaces { get; set; }

        public virtual DbSet<UserPlace> UserPlace { get; set; }

        public virtual DbSet<SendRecordType> SendRecordType { get; set; }

        public virtual DbSet<IteratorTable> IteratorTable { get; set; }
        public virtual DbSet<TrashClassTable> TrashClassTable { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserRoles)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);
            

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();


        }

        public System.Data.Entity.DbSet<WebApplication1.Models.TrashViewModel> TrashViewModels { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.SendSend> SendSends { get; set; }

        //public System.Data.Entity.DbSet<WebApplication1.Models.TrashViewModel> TrashViewModels { get; set; }

        //public System.Data.Entity.DbSet<WebApplication1.Models.MyViewModel> MyViewModels { get; set; }
    }
}
