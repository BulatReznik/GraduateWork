﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace BPMNWorkFlow.DbModels;

public partial class NodeType
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<TaskHandlerMapping> TaskHandlerMappings { get; set; } = new List<TaskHandlerMapping>();
}