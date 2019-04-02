using System;

namespace Domain.Entities
{
    public class Changeset
    {
        public int ChangesetId { get; set; }

        public string Owner { get; set; }

        public DateTime CreationDate { get; set; }

        public string Comment { get; set; }
    }
}