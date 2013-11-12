using System.Runtime.InteropServices;

namespace MediaIPC.WMP
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class RemoteHostInfo :
        IWMPRemoteMediaServices
    {
        public string GetServiceType()
        {
            return "Remote";
        }

        public string GetApplicationName()
        {
            return System.Diagnostics.Process.GetCurrentProcess().ProcessName;
        }

        public HResults GetScriptableObject(out string name, out object dispatch)
        {
            name = null;
            dispatch = null;
            return HResults.E_NOTIMPL;
        }

        public HResults GetCustomUIMode(out string file)
        {
            file = null;
            return HResults.E_NOTIMPL;
        }
    }
}
