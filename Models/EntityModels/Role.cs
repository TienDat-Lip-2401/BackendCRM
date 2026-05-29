using System;
using System.Collections.Generic;

namespace RedmineApp.Models.EntityModels;

public partial class Role : EntityBaseModel
{

    public string Name { get; set; } = null!;

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
}
