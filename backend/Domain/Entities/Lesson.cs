using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Lesson : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid CourseId { get; set; }
        public Course? Course { get; set; }
    }
}
