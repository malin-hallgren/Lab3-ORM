using System;
using System.Collections.Generic;

namespace SchoolDb2App.Models;

public partial class LastMonthSGrade
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Course { get; set; }

    public string Grade { get; set; } = null!;

    public DateOnly? DateSet { get; set; }
}
