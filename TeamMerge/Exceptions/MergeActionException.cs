using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMerge.Exceptions
{
    public class MergeActionException
        : Exception
    {
        public MergeActionException(string message) 
            : base(message)
        {
        }
    }
}
