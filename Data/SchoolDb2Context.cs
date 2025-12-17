using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SchoolDb2App.Models;

namespace SchoolDb2App.Data;

public partial class SchoolDb2Context : DbContext
{
    public SchoolDb2Context()
    {
    }

    public SchoolDb2Context(DbContextOptions<SchoolDb2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<GradeScale> GradeScales { get; set; }

    public virtual DbSet<GradeStatistic> GradeStatistics { get; set; }

    public virtual DbSet<LastMonthSGrade> LastMonthSGrades { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("connectionString"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__7577347E9386DA12");

            entity.Property(e => e.ClassId).HasColumnName("classId");
            entity.Property(e => e.ClassName)
                .HasMaxLength(10)
                .HasColumnName("className");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

            entity.HasOne(d => d.Employee).WithMany(p => p.Classes)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Classes__employe__440B1D61");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__2AA84FD18B0F5FE7");

            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.CourseName)
                .HasMaxLength(25)
                .HasColumnName("courseName");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

            entity.HasOne(d => d.Employee).WithMany(p => p.Courses)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Courses__employe__412EB0B6");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__C134C9C110083E89");

            entity.Property(e => e.EmployeeId).HasColumnName("employeeId");
            entity.Property(e => e.EmployeeFirstName)
                .HasMaxLength(25)
                .HasColumnName("employeeFirstName");
            entity.Property(e => e.EmployeeLastName)
                .HasMaxLength(50)
                .HasColumnName("employeeLastName");
            entity.Property(e => e.EmployeeSalary).HasColumnName("employeeSalary");
            entity.Property(e => e.EmployeeStartDate).HasColumnName("employeeStartDate");
        });

        modelBuilder.Entity<EmployeeRole>(entity =>
        {
            entity.HasKey(e => new { e.EmployeeId, e.RoleId });

            entity.Property(e => e.EmployeeId).HasColumnName("employeeId");
            entity.Property(e => e.RoleId).HasColumnName("roleId");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeRoles)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__EmployeeR__emplo__3A81B327");

            entity.HasOne(d => d.Role).WithMany(p => p.EmployeeRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__EmployeeR__roleI__3B75D760");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grades__FB4362F9E7AEA952");

            entity.Property(e => e.GradeId).HasColumnName("gradeId");
            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.GradeDate).HasColumnName("gradeDate");
            entity.Property(e => e.GradeScaleId).HasColumnName("gradeScaleId");
            entity.Property(e => e.StudentId).HasColumnName("studentId");

            entity.HasOne(d => d.Course).WithMany(p => p.Grades)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Grades__courseId__4AB81AF0");

            entity.HasOne(d => d.GradeScale).WithMany(p => p.Grades)
                .HasForeignKey(d => d.GradeScaleId)
                .HasConstraintName("FK_Grades_Scale");

            entity.HasOne(d => d.Student).WithMany(p => p.Grades)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Grades__studentI__49C3F6B7");
        });

        modelBuilder.Entity<GradeScale>(entity =>
        {
            entity.HasKey(e => e.GradeScaleId).HasName("PK__GradeSca__1C85981CBAAB0756");

            entity.ToTable("GradeScale");

            entity.Property(e => e.GradeScaleId).HasColumnName("gradeScaleId");
            entity.Property(e => e.GradeLetter)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gradeLetter");
            entity.Property(e => e.GradeNumeric).HasColumnName("gradeNumeric");
        });

        modelBuilder.Entity<GradeStatistic>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Grade Statistics");

            entity.Property(e => e.AverageGradeNumeric).HasColumnName("Average Grade - Numeric");
            entity.Property(e => e.Course).HasMaxLength(25);
            entity.Property(e => e.HighestGradeLetter)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Highest Grade - Letter");
            entity.Property(e => e.HighestGradeNumeric).HasColumnName("Highest Grade - Numeric");
            entity.Property(e => e.LowestGradeLetter)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Lowest Grade - Letter");
            entity.Property(e => e.LowestGradeNumeric).HasColumnName("Lowest Grade - Numeric");
        });

        modelBuilder.Entity<LastMonthSGrade>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Last Month's Grades");

            entity.Property(e => e.Course).HasMaxLength(25);
            entity.Property(e => e.DateSet).HasColumnName("Date Set");
            entity.Property(e => e.FirstName)
                .HasMaxLength(25)
                .HasColumnName("First Name");
            entity.Property(e => e.Grade)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("Last Name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__CD98462A2730A98E");

            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__4D11D63C48384FA2");

            entity.Property(e => e.StudentId).HasColumnName("studentId");
            entity.Property(e => e.StudentClass).HasColumnName("studentClass");
            entity.Property(e => e.StudentFirstName)
                .HasMaxLength(25)
                .HasColumnName("studentFirstName");
            entity.Property(e => e.StudentLastName)
                .HasMaxLength(50)
                .HasColumnName("studentLastName");
            entity.Property(e => e.StudentSsn)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("studentSsn");

            entity.HasOne(d => d.StudentClassNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.StudentClass)
                .HasConstraintName("FK__Students__studen__46E78A0C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
