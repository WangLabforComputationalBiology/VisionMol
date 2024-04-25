using static Ximmerse.XR.XDevicePlugin;

namespace Ximmerse.Wrapper.XDeviceService.Interface
{
    public interface IContext
    {
        long GetHandle();

        string GetClientVersion();
        int GetClientBuildNumber();
        string GetClientBuildInformation();

        int GetClientAlgVersion(AlgType algType);

        string GetServerVersion();
        int GetServerBuildNumber();
        string GetServerBuildInformation();

        int GetServerAlgVersion(AlgType algType);
    }
}