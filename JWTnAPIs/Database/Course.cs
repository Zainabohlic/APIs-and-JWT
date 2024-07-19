using System;
using System.Collections.Generic;

namespace Project2.Database;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public string? Prerequisite { get; set; }
}
