using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using netCommander.winControls;

namespace netCommander.WNet
{
    class WinApiWNETwrapper
    {


        private const int REASONABLE_BUFFER_SIZE = 16384;

        

        public static NETRESOURCE GetResourceInfo(NETRESOURCE resource)
        {
            return GetResourceInfo(resource, 512);
        }

        public static void AddConnection
            (IntPtr owner_window,
            string remote_name,
            string provider,
            ResourceType resource_type,
            bool remember)
        {
            var net_struct = new NETRESOURCE();
            net_struct.dwType = resource_type;
            net_struct.lpProvider = provider;
            net_struct.lpRemoteName = remote_name;

            var opts = WNetConnectOptions.INTERACTIVE;
            if (remember)
            {
                opts = opts | WNetConnectOptions.UPDATE_PROFILE;
            }

            var res = WinApiWNET.WNetAddConnection3
                (owner_window,
                ref net_struct,
                string.Empty,
                string.Empty,
                opts);

            if (res != WinApiWNET.NO_ERROR)
            {
                throw new Win32Exception(res);
            }
        }

        public static NETRESOURCE GetResourceInfo(string net_path)
        {
            var res = new NETRESOURCE();
            res.lpProvider = null;
            res.lpRemoteName = net_path;
            return GetResourceInfo(res);
        }

        private static NETRESOURCE GetResourceInfo(NETRESOURCE resource,int buffer_size)
        {
            var info_buffer = IntPtr.Zero;
            //int buffer_size = 512;
            var system_ptr = IntPtr.Zero;
            try
            {
                info_buffer = Marshal.AllocHGlobal(buffer_size);

                var res = WinApiWNET.WNetGetResourceInformation
                    (ref resource,
                    info_buffer,
                    ref buffer_size,
                    ref system_ptr);
                //check res
                switch (res)
                {
                    case WinApiWNET.ERROR_EXTENDED_ERROR:
                        WNetException.ThrowOnError();
                        return new NETRESOURCE();

                    case WinApiWNET.ERROR_MORE_DATA:
                        //alloc new buffer
                        Marshal.FreeHGlobal(info_buffer);
                        return GetResourceInfo(resource, buffer_size);

                    case WinApiWNET.NO_ERROR:
                        var ret = new NETRESOURCE();
                        NETRESOURCE.FromBuffer(info_buffer, 0, ref ret);
                        return ret;

                    default:
                        throw new Win32Exception((int)res);
                }
            }
            finally
            {
                if (info_buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(info_buffer);
                }
            }

        }

        public static NETINFOSTRUCT GetNetworkInfo(string provider)
        {
            var ret = NETINFOSTRUCT.Prepare();
            var res = WinApiWNET.WNetGetNetworkInformation(provider, ref ret);
            if (res != WinApiWNET.NO_ERROR)
            {
                throw new Win32Exception(res);
            }
            return ret;
        }

        public static NETRESOURCE RootNetresource
        {
            get
            {
                var ret = new NETRESOURCE();
                ret.dwDisplayType = ResourceDisplayType.ROOT;
                ret.dwScope = ResourceScope.GLOBALNET;
                ret.dwType = ResourceType.ANY;
                ret.dwUsage = ResourceUsage.NONE;
                ret.lpComment = null;
                ret.lpLocalName = null;
                ret.lpProvider = null;
                ret.lpRemoteName = null;
                return ret;
            }
        }

        public static NETRESOURCE GetParentResource(NETRESOURCE resource)
        {
            var buffer = IntPtr.Zero;
            var buffer_size = 256;
            var ret = new NETRESOURCE();

            try
            {
               buffer = Marshal.AllocHGlobal(buffer_size);
                var res = WinApiWNET.WNetGetResourceParent
                    (ref resource,
                    buffer,
                    ref buffer_size);
                if (res == WinApiWNET.ERROR_MORE_DATA)
                {
                    res = WinApiWNET.WNetGetResourceParent
                    (ref resource,
                    buffer,
                    ref buffer_size);
                }
                if (res == WinApiWNET.ERROR_EXTENDED_ERROR)
                {
                    WNetException.ThrowOnError();
                }
                if (res != WinApiWNET.NO_ERROR)
                {
                    throw new Win32Exception((int)res);
                }
                NETRESOURCE.FromBuffer(buffer, 0, ref ret);
                return ret;
                
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
        }
    }
}
