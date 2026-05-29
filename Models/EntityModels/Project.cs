using System;
using System.Collections.Generic;

namespace RedmineApp.Models.EntityModels;

public partial class Project : EntityBaseModel
{

    public string ProjectCode { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? ManagerId { get; set; }

    public int? Status { get; set; }

    public bool? IsPublic { get; set; }

    public bool? IsActive { get; set; }

    public virtual User? Manager { get; set; }

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
}
