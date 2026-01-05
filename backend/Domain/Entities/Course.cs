using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class Course : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        public ICollection<Lesson> Lessons { get; set; } = [];
    }
}
