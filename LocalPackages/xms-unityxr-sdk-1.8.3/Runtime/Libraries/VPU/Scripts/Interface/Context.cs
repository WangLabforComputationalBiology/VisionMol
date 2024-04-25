using static Ximmerse.XR.XDevicePlugin;

namespace Ximmerse.Wrapper.XDeviceService.Interface
{
    public abstract class Context : IContext
    {
        protected long Handle;

        public Context()
        {
            Handle = GetHandle();
        }

        public abstract long GetHandle();

        public abstract string GetClientVersion();
        public abstract int GetClientBuildNumber();
        public abstract string GetClientBuildInformation();

        public abstract int GetClientAlgVersion(AlgType algType);

        public abstract string GetServerVersion();
        public abstract int GetServerBuildNumber();
        public abstract string GetServerBuildInformation();

        public abstract int GetServerAlgVersion(AlgType algType);
    }
}