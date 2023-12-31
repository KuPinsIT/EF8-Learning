﻿using System.ComponentModel.DataAnnotations;
using EF7ColumnJSON.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace EF7ColumnJSON.Entities
{
    public class Halfling
    {
        public Halfling()
        {
            
        }
        public Halfling(HierarchyId pathFromPatriarch, string name, int? yearOfBirth = null)
        {
            PathFromPatriarch = pathFromPatriarch;
            Name = name;
            YearOfBirth = yearOfBirth;
        }

        public int Id { get; private set; }
        public HierarchyId PathFromPatriarch { get; set; }
        public string Name { get; set; }
        public int? YearOfBirth { get; set; }
    }
}
