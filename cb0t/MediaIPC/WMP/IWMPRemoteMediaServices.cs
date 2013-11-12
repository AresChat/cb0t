using System;
using System.Runtime.InteropServices;

namespace MediaIPC.WMP
{
    [ComImport,
    ComVisible(true),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("CBB92747-741F-44fe-AB5B-F1A48F3B2A59")]
    public interface IWMPRemoteMediaServices
    {
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetServiceType();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetApplicationName();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.U4)]
        HResults GetScriptableObject([MarshalAs(UnmanagedType.BStr)] out string name,
            [MarshalAs(UnmanagedType.IDispatch)] out object dispatch);

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.U4)]
        HResults GetCustomUIMode([MarshalAs(UnmanagedType.BStr)] out string file);
    }
}
