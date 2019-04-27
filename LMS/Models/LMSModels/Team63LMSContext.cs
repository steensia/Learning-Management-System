using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team63LMSContext : DbContext
    {
        public Team63LMSContext()
        {
        }

        public Team63LMSContext(DbContextOptions<Team63LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrators> Administrators { get; set; }
        public virtual DbSet<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual DbSet<Assignments> Assignments { get; set; }
        public virtual DbSet<Classes> Classes { get; set; }
        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Enrolled> Enrolled { get; set; }
        public virtual DbSet<Professors> Professors { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Submission> Submission { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=atr.eng.utah.edu;User Id=u1031024;Password=aero123;Database=Team63LMS");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrators>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FName)
                    .HasColumnName("fName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LName)
                    .HasColumnName("lName")
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<AssignmentCategories>(entity =>
            {
                entity.HasKey(e => e.AssignCatId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId)
                    .HasName("ClassID");

                entity.HasIndex(e => new { e.Name, e.ClassId })
                    .HasName("Name")
                    .IsUnique();

                entity.Property(e => e.AssignCatId).HasColumnName("assignCatID");

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.Name).HasColumnType("varchar(100)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("AssignmentCategories_ibfk_1");
            });

            modelBuilder.Entity<Assignments>(entity =>
            {
                entity.HasKey(e => e.AId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.AssignCatId)
                    .HasName("assignCatID");

                entity.Property(e => e.AId).HasColumnName("aID");

                entity.Property(e => e.AssignCatId).HasColumnName("assignCatID");

                entity.Property(e => e.Contents).HasColumnType("varchar(8192)");

                entity.Property(e => e.Due).HasColumnType("datetime");

                entity.Property(e => e.Name).HasColumnType("varchar(100)");

                entity.HasOne(d => d.AssignCat)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.AssignCatId)
                    .HasConstraintName("Assignments_ibfk_1");
            });

            modelBuilder.Entity<Classes>(entity =>
            {
                entity.HasKey(e => e.ClassId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("CategoryID");

                entity.HasIndex(e => e.TaughtBy)
                    .HasName("TaughtBy");

                entity.HasIndex(e => new { e.Semester, e.CategoryId })
                    .HasName("Semester")
                    .IsUnique();

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.End).HasColumnType("time");

                entity.Property(e => e.Loc).HasColumnType("varchar(100)");

                entity.Property(e => e.Semester).HasColumnType("varchar(11)");

                entity.Property(e => e.Start).HasColumnType("time");

                entity.Property(e => e.TaughtBy)
                    .IsRequired()
                    .HasColumnType("char(8)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("Classes_ibfk_2");

                entity.HasOne(d => d.TaughtByNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.TaughtBy)
                    .HasConstraintName("Classes_ibfk_1");
            });

            modelBuilder.Entity<Courses>(entity =>
            {
                entity.HasKey(e => e.CatalogId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Listing)
                    .HasName("Listing");

                entity.Property(e => e.CatalogId).HasColumnName("CatalogID");

                entity.Property(e => e.Listing)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.Name).HasColumnType("varchar(100)");

                entity.HasOne(d => d.ListingNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.Listing)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Departments>(entity =>
            {
                entity.HasKey(e => e.Subject)
                    .HasName("PRIMARY");

                entity.Property(e => e.Subject).HasColumnType("varchar(4)");

                entity.Property(e => e.Name).HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Enrolled>(entity =>
            {
                entity.HasKey(e => new { e.UId, e.ClassId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId)
                    .HasName("ClassID");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.Grade).HasColumnType("varchar(2)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("Enrolled_ibfk_2");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.UId)
                    .HasConstraintName("Enrolled_ibfk_1");
            });

            modelBuilder.Entity<Professors>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.WorksIn)
                    .HasName("WorksIn");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FName)
                    .HasColumnName("fName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LName)
                    .HasColumnName("lName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.WorksIn)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.WorksInNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.WorksIn)
                    .HasConstraintName("Professors_ibfk_1");
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Major)
                    .HasName("Major");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FName)
                    .HasColumnName("fName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LName)
                    .HasColumnName("lName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Major)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.MajorNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Major)
                    .HasConstraintName("Students_ibfk_1");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => new { e.AId, e.UId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.UId)
                    .HasName("uID");

                entity.Property(e => e.AId).HasColumnName("aID");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Contents).HasColumnType("varchar(8192)");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.A)
                    .WithMany(p => p.Submission)
                    .HasForeignKey(d => d.AId)
                    .HasConstraintName("Submission_ibfk_1");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Submission)
                    .HasForeignKey(d => d.UId)
                    .HasConstraintName("Submission_ibfk_2");
            });
        }
    }
}
