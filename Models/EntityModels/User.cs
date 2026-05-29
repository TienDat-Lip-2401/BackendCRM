using System;
using System.Collections.Generic;

namespace RedmineApp.Models.EntityModels;

public partial class User : EntityBaseModel
{

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? PhoneNumber { get; set; }

    public int? Gender { get; set; }

    public DateTime? Birthday { get; set; }

    public DateTime? JoinedDate { get; set; }

    public DateTime? LeavedDate { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsFirstLogin { get; set; } = true;

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    = new List<RefreshToken>();
}
