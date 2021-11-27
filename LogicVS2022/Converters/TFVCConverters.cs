using Domain.Entities.TFVC;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace LogicVS2022.Converters
{
    public static class TFVCConverters
    {
        public static VersionSpec Convert(this TFVCChangesetVersionSpec changesetVersionSpec)
        {
            return new ChangesetVersionSpec(changesetVersionSpec.ChangesetId);
        }

        public static GetRequest Convert(this TFVCGetRequest getRequest)
        {
            return new GetRequest(getRequest.Item, (RecursionType)(int)getRequest.Recursion, VersionSpec.Latest);
        }
    }
}