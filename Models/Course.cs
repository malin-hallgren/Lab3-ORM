using System;
using System.Collections.Generic;

namespace SchoolDb2App.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public int? EmployeeId { get; set; }

    public string? CourseName { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
