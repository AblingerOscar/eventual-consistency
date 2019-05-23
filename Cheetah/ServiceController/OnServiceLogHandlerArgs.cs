using ViewService;

namespace Cheetah.ServiceController
{
    internal class OnServiceLogHandlerArgs
    {
        internal OnLogHandlerArgs OriginalArgs;
        internal ServiceInformation ServiceInformation;

        internal OnServiceLogHandlerArgs(ServiceInformation serviceInformation, OnLogHandlerArgs originalArgs)
        {
            ServiceInformation = serviceInformation;
            OriginalArgs = originalArgs;
        }
    }
}