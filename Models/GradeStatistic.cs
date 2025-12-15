using System;
using System.Collections.Generic;

namespace SchoolDb2App.Models;

public partial class GradeStatistic
{
    public string? Course { get; set; }

    public int? AverageGradeNumeric { get; set; }

    public string? LowestGradeLetter { get; set; }

    public int? LowestGradeNumeric { get; set; }

    public string? HighestGradeLetter { get; set; }

    public int? HighestGradeNumeric { get; set; }
}
