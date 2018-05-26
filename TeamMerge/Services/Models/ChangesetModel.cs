using System;

namespace TeamMerge.Services.Models
{
    public class ChangesetModel
    {
        public int ChangesetId { get; set; }

        public string Owner { get; set; }

        public DateTime CreationDate { get; set; }

        public string Comment { get; set; }
    }
}