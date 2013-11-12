using System;
using System.Runtime.InteropServices;

namespace MediaIPC.WMP
{
    [ComImport, ComVisible(true),
    Guid("00000112-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleObject
    {
        void SetClientSite(IOleClientSite pClientSite);

        [return: MarshalAs(UnmanagedType.Interface)]
        IOleClientSite GetClientSite();

        void SetHostNames(
            [MarshalAs(UnmanagedType.LPWStr)]string szContainerApp,
            [MarshalAs(UnmanagedType.LPWStr)]string szContainerObj);

        void Close(uint dwSaveOption);

        void SetMoniker(uint dwWhichMoniker, object pmk);

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetMoniker(uint dwAssign, uint dwWhichMoniker);

        void InitFromData(object pDataObject, bool fCreation, uint dwReserved);

        object GetClipboardData(uint dwReserved);

        void DoVerb(uint iVerb, uint lpmsg, [MarshalAs(UnmanagedType.Interface)]object pActiveSite,
            uint lindex, uint hwndParent, uint lprcPosRect);

        [return: MarshalAs(UnmanagedType.Interface)]
        object EnumVerbs();

        void Update();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.U4)]
        HResults IsUpToDate();

        Guid GetUserClassID();

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetUserType(uint dwFormOfType);

        void SetExtent(uint dwDrawAspect, [MarshalAs(UnmanagedType.Interface)] object psizel);

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetExtent(uint dwDrawAspect);

        uint Advise([MarshalAs(UnmanagedType.Interface)]object pAdvSink);

        void Unadvise(uint dwConnection);

        [return: MarshalAs(UnmanagedType.Interface)]
        object EnumAdvise();

        uint GetMiscStatus([MarshalAs(UnmanagedType.U4)] DVASPECT dwAspect);

        void SetColorScheme([MarshalAs(UnmanagedType.Interface)] object pLogpal);
    }
}
