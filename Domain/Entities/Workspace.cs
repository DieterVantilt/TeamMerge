
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Workspace
    {
        public string Name { get; set; }
        public string OwnerName { get; set; }

        public override bool Equals(object obj)
        {
            var workspace = obj as Workspace;
            return workspace != null &&
                   Name == workspace.Name &&
                   OwnerName == workspace.OwnerName;
        }

        public override int GetHashCode()
        {
            var hashCode = 607827781;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(OwnerName);
            return hashCode;
        }
    }
}