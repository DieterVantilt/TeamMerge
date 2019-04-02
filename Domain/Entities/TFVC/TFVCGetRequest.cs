using Domain.Entities.TFVC.Enums;

namespace Domain.Entities.TFVC
{
    public class TFVCGetRequest
    {
        public string Item { get; set;  }
        public TFVCRecursionType Recursion { get; set;  }
        public bool LatestVersion { get; set;  }
    }
}