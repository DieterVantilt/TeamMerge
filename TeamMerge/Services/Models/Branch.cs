using System.Collections.Generic;

namespace TeamMerge.Services.Models
{
    public class Branch
    {
        public string Name { get; set; }

        public List<string> Branches { get; set; }
    }
}