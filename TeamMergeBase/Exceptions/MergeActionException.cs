using System;

namespace TeamMergeBase.Exceptions
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