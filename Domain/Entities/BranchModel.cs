using System.Collections.Generic;

namespace Domain.Entities
{
    public class Branch
    {
        public string Name { get; set; }

        public List<string> Branches { get; set; }
    }
}