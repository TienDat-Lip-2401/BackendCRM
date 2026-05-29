using System;
using System.Collections.Generic;

namespace RedmineApp.Models.EntityModels;

public partial class Position : EntityBaseModel
{

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
