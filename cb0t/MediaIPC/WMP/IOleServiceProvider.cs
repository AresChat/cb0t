using System;
using System.Runtime.InteropServices;

namespace MediaIPC.WMP
{
    [ComImport,
     GuidAttribute("6d5140c1-7436-11ce-8034-00aa006009fa"),
     InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown),
     ComVisible(true)]
    public interface IOleServiceProvider
    {
        IntPtr QueryService(ref Guid guidService, ref Guid riid);
    }
}
