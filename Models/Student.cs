using System;
using System.Collections.Generic;

namespace SchoolDb2App.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string StudentFirstName { get; set; } = null!;

    public string StudentLastName { get; set; } = null!;

    public string? StudentSsn { get; set; }

    public int? StudentClass { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual Class? StudentClassNavigation { get; set; }
}
