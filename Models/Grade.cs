using System;
using System.Collections.Generic;

namespace SchoolDb2App.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public int? StudentId { get; set; }

    public int? CourseId { get; set; }

    public DateOnly? GradeDate { get; set; }

    public int? GradeScaleId { get; set; }

    public virtual Course? Course { get; set; }

    public virtual GradeScale? GradeScale { get; set; }

    public virtual Student? Student { get; set; }
}
