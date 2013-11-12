using System;
using System.Runtime.InteropServices;

namespace MediaIPC.WMP
{
    [ComImport,
     ComVisible(true),
     Guid("00000118-0000-0000-C000-000000000046"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleClientSite
    {
        void SaveObject();

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetMoniker(uint dwAssign, uint dwWhichMoniker);

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetContainer();

        void ShowObject();

        void OnShowWindow(bool fShow);

        void RequestNewObjectLayout();
    }
}
