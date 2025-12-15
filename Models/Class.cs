using System;
using System.Collections.Generic;

namespace SchoolDb2App.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public int? EmployeeId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
