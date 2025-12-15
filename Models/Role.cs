using System;
using System.Collections.Generic;

namespace SchoolDb2App.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;
}
