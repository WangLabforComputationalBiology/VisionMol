using static Ximmerse.XR.XDevicePlugin;

namespace Ximmerse.Wrapper.XDeviceService.Client {
    public class ClientContext : Interface.Context
    {
        public override long GetHandle()
        {
            return XDeviceClientApi.GetXContext();
        }

        public override string GetClientVersion()
        {
            return XDeviceClientApi.GetClientVersion();
        }

        public override int GetClientBuildNumber()
        {
            return XDeviceClientApi.GetClientBuildNumber();
        }

        public override string GetClientBuildInformation()
        {
            return XDeviceClientApi.GetClientBuildInformation();
        }

        public override int GetClientAlgVersion(AlgType algType)
        {
            return XDeviceClientApi.GetClientAlgVersion(algType);
        }

        public override string GetServerVersion()
        {
            return XDeviceClientApi.GetServerVersion();
        }

        public override int GetServerBuildNumber()
        {
            return XDeviceClientApi.GetServerBuildNumber();
        }

        public override string GetServerBuildInformation()
        {
            return XDeviceClientApi.GetServerBuildInformation();
        }

        public override int GetServerAlgVersion(AlgType algType)
        {
            return XDeviceClientApi.GetServerAlgVersion(algType);
        }
    }
}