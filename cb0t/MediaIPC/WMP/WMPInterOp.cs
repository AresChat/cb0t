using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MediaIPC.WMP
{
    [AxHost.ClsidAttribute("{6bf52a52-394a-11d3-b153-00c04f79faa6}")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    class WMPInterOp : AxHost, IOleServiceProvider, IOleClientSite
    {
        public WMPInterOp() :
            base("6bf52a52-394a-11d3-b153-00c04f79faa6")
        {

        }

        protected override void AttachInterfaces()
        {
            IOleObject oleObject = (IOleObject)this.GetOcx();

            if (oleObject != null)
                oleObject.SetClientSite((IOleClientSite)this);
        }

        IntPtr IOleServiceProvider.QueryService(ref Guid guidService, ref Guid riid)
        {
            if (riid == new Guid("cbb92747-741f-44fe-ab5b-f1a48f3b2a59"))
            {
                IWMPRemoteMediaServices iwmp = new RemoteHostInfo();
                return Marshal.GetComInterfaceForObject(iwmp, typeof(IWMPRemoteMediaServices));
            }

            return IntPtr.Zero;
        }

        void IOleClientSite.SaveObject()
        {
            throw new COMException("Not Implemented", (int)HResults.E_NOTIMPL);
        }

        object IOleClientSite.GetMoniker(uint dwAssign, uint dwWhichMoniker)
        {
            throw new COMException("Not Implemented", (int)HResults.E_NOTIMPL);
        }

        object IOleClientSite.GetContainer()
        {
            return (int)HResults.E_NOINTERFACE;
        }

        void IOleClientSite.ShowObject()
        {
            throw new COMException("Not Implemented", (int)HResults.E_NOTIMPL);
        }

        void IOleClientSite.OnShowWindow(bool fShow)
        {
            throw new COMException("Not Implemented", (int)HResults.E_NOTIMPL);
        }

        void IOleClientSite.RequestNewObjectLayout()
        {
            throw new COMException("Not Implemented", (int)HResults.E_NOTIMPL);
        }
    }
}
