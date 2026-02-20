using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDescGenerator;

public class ProjectMetadata
{
    public string Title { get; set; }
    public string Description { get; set; }
    public List<string> TechnologyUsed { get; set; }
}
