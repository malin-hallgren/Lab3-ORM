using System;
using System.Collections.Generic;

namespace SchoolDb2App.Models;

public partial class GradeScale
{
    public int GradeScaleId { get; set; }

    public string GradeLetter { get; set; } = null!;

    public int GradeNumeric { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
