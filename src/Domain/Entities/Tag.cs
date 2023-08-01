﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo_App.Domain.Entities;
public class Tag : BaseAuditableEntity
{
    public string Name { get; set; }
    public ICollection<TodoItemTag> TodoItemTags { get; set; }
}
