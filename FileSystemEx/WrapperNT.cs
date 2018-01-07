using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace netCommander.FileSystemEx
{
    class ntApiFSwrapper
    {
        private ntApiFSwrapper() { }

        private const int STREAM_INFO_BUFFER_LEN = 4096;
        private const int VOLUME_INFO_BUFFER_LEN = 512;

        public static FILE_FS_DEVICE_INFORMATION GetFileVolumeDeviceInfo(string file_name)
        {
            var file_handle = IntPtr.Zero;

            try
            {
                file_handle = WinApiFS.CreateFile_intptr
                    (file_name,
                    Win32FileAccess.GENERIC_READ,
                    FileShare.ReadWrite,
                    IntPtr.Zero,
                    FileMode.Open,
                    CreateFileOptions.None,
                    IntPtr.Zero);
                if (file_handle.ToInt64() == WinApiFS.INVALID_HANDLE_VALUE)
                {
                    var win_err = Marshal.GetLastWin32Error();
                    var win_ex = new Win32Exception(win_err);
                    throw win_ex;
                }
                return GetFileVolumeDeviceInfo(file_handle);
            }
            finally
            {
                if ((file_handle != IntPtr.Zero) && (file_handle.ToInt64() != WinApiFS.INVALID_HANDLE_VALUE))
                {
                    WinApiFS.CloseHandle(file_handle);
                }
            }
        }

        public static FILE_FS_DEVICE_INFORMATION GetFileVolumeDeviceInfo(IntPtr file_handle)
        {
            var status = new IO_STATUS_BLOCK();
            var ret_buffer = IntPtr.Zero;

            try
            {
                var ret_buffer_len = Marshal.SizeOf(typeof(FILE_FS_DEVICE_INFORMATION));
                ret_buffer = Marshal.AllocHGlobal(ret_buffer_len);
                var res = ntApiFS.NtQueryVolumeInformationFile
                    (file_handle,
                    ref status,
                    ret_buffer,
                    ret_buffer_len,
                    FS_INFORMATION_CLASS.FileFsDeviceInformation);
                NTSTATUS_helper.ThrowOnError(res, status, NTSTATUS_severity.Warning);
                var ret = (FILE_FS_DEVICE_INFORMATION)Marshal.PtrToStructure
                    (ret_buffer,
                    typeof(FILE_FS_DEVICE_INFORMATION));
                return ret;
            }
            finally
            {
                if (ret_buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ret_buffer);
                }
            }
        }

        public static FILE_FS_VOLUME_INFORMATION GetFileVolumeInfo(string file_name)
        {
            var file_handle = IntPtr.Zero;

            try
            {
                file_handle = WinApiFS.CreateFile_intptr
                    (file_name,
                    Win32FileAccess.GENERIC_READ,
                    FileShare.ReadWrite,
                    IntPtr.Zero,
                    FileMode.Open,
                    CreateFileOptions.None,
                    IntPtr.Zero);
                if (file_handle.ToInt64() == WinApiFS.INVALID_HANDLE_VALUE)
                {
                    var win_err = Marshal.GetLastWin32Error();
                    var win_ex = new Win32Exception(win_err);
                    throw win_ex;
                }
                return GetFileVolumeInfo(file_handle);
            }
            finally
            {
                if ((file_handle != IntPtr.Zero) && (file_handle.ToInt64() != WinApiFS.INVALID_HANDLE_VALUE))
                {
                    WinApiFS.CloseHandle(file_handle);
                }
            }
        }

        public static FILE_FS_VOLUME_INFORMATION GetFileVolumeInfo(IntPtr file_handle)
        {
            var status = new IO_STATUS_BLOCK();
            var ret_buffer = IntPtr.Zero;
            try
            {
                var ret_buffer_len = VOLUME_INFO_BUFFER_LEN;
                ret_buffer = Marshal.AllocHGlobal(ret_buffer_len);
                var res = ntApiFS.NtQueryVolumeInformationFile
                    (file_handle,
                    ref status,
                    ret_buffer,
                    ret_buffer_len,
                    FS_INFORMATION_CLASS.FileFsVolumeInformation);
                NTSTATUS_helper.ThrowOnError(res, status, NTSTATUS_severity.Warning);
                var ret = FILE_FS_VOLUME_INFORMATION.FromBuffer(ret_buffer);
                return ret;
            }
            finally
            {
                if (ret_buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ret_buffer);
                }
            }
        }

        public static FILE_FS_FULLSIZE_INFORMATION GetFileVolumeFullsizeInfo(string file_name)
        {
            var file_handle = IntPtr.Zero;

            try
            {
                file_handle = WinApiFS.CreateFile_intptr
                    (file_name,
                    Win32FileAccess.GENERIC_READ,
                    FileShare.ReadWrite,
                    IntPtr.Zero,
                    FileMode.Open,
                    CreateFileOptions.None,
                    IntPtr.Zero);
                if (file_handle.ToInt64() == WinApiFS.INVALID_HANDLE_VALUE)
                {
                    var win_err = Marshal.GetLastWin32Error();
                    var win_ex = new Win32Exception(win_err);
                    throw win_ex;
                }
                return GetFileVolumeFullsizeInfo(file_handle);
            }
            finally
            {
                if ((file_handle != IntPtr.Zero) && (file_handle.ToInt64() != WinApiFS.INVALID_HANDLE_VALUE))
                {
                    WinApiFS.CloseHandle(file_handle);
                }
            }
        }

        public static FILE_FS_FULLSIZE_INFORMATION GetFileVolumeFullsizeInfo(IntPtr file_handle)
        {
            var status = new IO_STATUS_BLOCK();
            var ret_buffer = IntPtr.Zero;

            try
            {
                var ret_buffer_len = Marshal.SizeOf(typeof(FILE_FS_FULLSIZE_INFORMATION));
                ret_buffer = Marshal.AllocHGlobal(ret_buffer_len);
                var res = ntApiFS.NtQueryVolumeInformationFile
                            (file_handle,
                            ref status,
                            ret_buffer,
                            ret_buffer_len,
                            FS_INFORMATION_CLASS.FileFsFullSizeInformation);

                //check res
                NTSTATUS_helper.ThrowOnError(res, status, NTSTATUS_severity.Warning);

                var ret = (FILE_FS_FULLSIZE_INFORMATION)Marshal.PtrToStructure
                    (ret_buffer,
                    typeof(FILE_FS_FULLSIZE_INFORMATION));
                return ret;
            }
            finally
            {
                if (ret_buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ret_buffer);
                }
            }
        }

        public static FILE_FS_CONTROL_INFORMATION GetFileVolumeControlInfo(string file_name)
        {
            var file_handle = IntPtr.Zero;

            try
            {
                file_handle = WinApiFS.CreateFile_intptr
                    (file_name,
                    Win32FileAccess.GENERIC_READ,
                    FileShare.ReadWrite,
                    IntPtr.Zero,
                    FileMode.Open,
                    CreateFileOptions.None,
                    IntPtr.Zero);
                if (file_handle.ToInt64() == WinApiFS.INVALID_HANDLE_VALUE)
                {
                    var win_err = Marshal.GetLastWin32Error();
                    var win_ex = new Win32Exception(win_err);
                    throw win_ex;
                }

                return GetFileVolumeControlInfo(file_handle);
            }
            finally
            {
                if ((file_handle != IntPtr.Zero) && (file_handle.ToInt64() != WinApiFS.INVALID_HANDLE_VALUE))
                {
                    WinApiFS.CloseHandle(file_handle);
                }
            }
        }

        public static FILE_FS_CONTROL_INFORMATION GetFileVolumeControlInfo(IntPtr file_handle)
        {
            var status = new IO_STATUS_BLOCK();
            var ret_buffer = IntPtr.Zero;

            try
            {
                var ret_buffer_len = Marshal.SizeOf(typeof(FILE_FS_CONTROL_INFORMATION));
                ret_buffer = Marshal.AllocHGlobal(ret_buffer_len);
                var res = ntApiFS.NtQueryVolumeInformationFile
                        (file_handle,
                        ref status,
                        ret_buffer,
                        ret_buffer_len,
                        FS_INFORMATION_CLASS.FileFsControlInformation);

                //check res
                NTSTATUS_helper.ThrowOnError(res, status, NTSTATUS_severity.Warning);

                var ret = (FILE_FS_CONTROL_INFORMATION)Marshal.PtrToStructure
                    (ret_buffer,
                    typeof(FILE_FS_CONTROL_INFORMATION));
                return ret;
            }
            finally
            {
                if (ret_buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ret_buffer);
                }
            }
        }

        public static FILE_FS_ATTRIBUTE_INFORMATION GetFileVolumeAttributeInfo(string file_name)
        {
            var file_handle = IntPtr.Zero;
            try
            {
                file_handle = WinApiFS.CreateFile_intptr
                    (file_name,
                    Win32FileAccess.GENERIC_READ,
                    FileShare.ReadWrite,
                    IntPtr.Zero,
                    FileMode.Open,
                    CreateFileOptions.None,
                    IntPtr.Zero);
                if (file_handle.ToInt64() == WinApiFS.INVALID_HANDLE_VALUE)
                {
                    var win_err = Marshal.GetLastWin32Error();
                    var ex = new Win32Exception(win_err);
                    throw ex;
                }

                var ret = GetFileVolumeAttributeInfo
                    (file_handle,
                    IntPtr.Zero,
                    0);

                return ret;
            }
            finally
            {
                if ((file_handle != IntPtr.Zero) && (file_handle.ToInt64() != WinApiFS.INVALID_HANDLE_VALUE))
                {
                    WinApiFS.CloseHandle(file_handle);
                }
            }
        }

        public static FILE_FS_ATTRIBUTE_INFORMATION GetFileVolumeAttributeInfo(IntPtr file_handle, IntPtr buffer, int buffer_len)
        {
            var status = new IO_STATUS_BLOCK();
            var need_free_buffer = false;
            var ret_buffer = buffer;
            var ret_buffer_len = buffer_len;

            try
            {
                if (buffer_len == 0)
                {
                    need_free_buffer = true;
                    ret_buffer_len = VOLUME_INFO_BUFFER_LEN;
                    ret_buffer = Marshal.AllocHGlobal(VOLUME_INFO_BUFFER_LEN);
                }

                var res = ntApiFS.NtQueryVolumeInformationFile
                    (file_handle,
                    ref status,
                    ret_buffer,
                    ret_buffer_len,
                    FS_INFORMATION_CLASS.FileFsAttributeInformation);

                //check res
                NTSTATUS_helper.ThrowOnError(res, status, NTSTATUS_severity.Warning);

                var ret = FILE_FS_ATTRIBUTE_INFORMATION.FromBuffer(ret_buffer);
                return ret;
            }
            finally
            {
                if (need_free_buffer)
                {
                    Marshal.FreeHGlobal(ret_buffer);
                }
            }
        }

        public static NT_FILE_STREAM_INFORMATION[] GetFileInfo_stream(string file_name)
        {
            var file_handle = IntPtr.Zero;

            try
            {
                file_handle = WinApiFS.CreateFile_intptr
                    (file_name,
                    Win32FileAccess.GENERIC_READ,
                    FileShare.ReadWrite,
                    IntPtr.Zero,
                    FileMode.Open,
                    CreateFileOptions.BACKUP_SEMANTICS, //need for retrieve info about directory
                    IntPtr.Zero);
                if (file_handle.ToInt64() == WinApiFS.INVALID_HANDLE_VALUE)
                {
                    var win_err = Marshal.GetLastWin32Error();
                    Exception ex = new Win32Exception(win_err);
                    throw ex;
                }

                return GetFileInfo_stream(file_handle, IntPtr.Zero, 0);
            }
            finally
            {
                if ((file_handle != IntPtr.Zero)&&(file_handle.ToInt64()!=WinApiFS.INVALID_HANDLE_VALUE))
                {
                    WinApiFS.CloseHandle(file_handle);
                }
            }
        }

        public static NT_FILE_STREAM_INFORMATION[] GetFileInfo_stream(IntPtr file_handle, IntPtr buffer, int buffer_size)
        {
            //init IO_STATUS_BLOCK
            var status = new IO_STATUS_BLOCK();
            var need_free_buffer = false;

            try
            {
                if (buffer_size == 0)
                {
                    buffer_size = STREAM_INFO_BUFFER_LEN;
                }
                if (buffer == IntPtr.Zero)
                {
                    need_free_buffer = true;
                    buffer = Marshal.AllocHGlobal(buffer_size);
                }

                //trick
                //how to determine, if buffer unchanged?
                //write 0 to StreamNameLength member
                Marshal.WriteInt32(buffer, 4, 0);

                var res = ntApiFS.NtQueryInformationFile
                    (file_handle,
                    ref status,
                    buffer,
                    buffer_size,
                    FILE_INFORMATION_CLASS.FileStreamInformation);

                //check res
                NTSTATUS_helper.ThrowOnError(res, status, NTSTATUS_severity.Warning);

                //if success, continue
                var ret_list = new List<NT_FILE_STREAM_INFORMATION>();
                var current_offset = 0;
                while (true)
                {
                    var current_info = new NT_FILE_STREAM_INFORMATION();

                    //first 4 bytes - offset to next entry
                    current_info.NextEntryOffset = Marshal.ReadInt32(buffer, current_offset);

                    //next 4 bytes - stream name len (in bytes)
                    current_info.StreamNameLength = Marshal.ReadInt32(buffer, current_offset + 4);

                    if ((current_info.StreamNameLength == 0) && (ret_list.Count == 0))
                    {
                        //that is no streams at all (as directory without AFS)
                        break;
                    }

                    //next 8 bytes - stream size
                    current_info.StreamSize = Marshal.ReadInt64(buffer, current_offset + 8);

                    //next 8 bytes - stream allocastion size
                    current_info.StreamAllocationSize = Marshal.ReadInt64(buffer, current_offset + 16);

                    //next StreamNameLength bytes - unicode stream name
                    current_info.StreamName = Marshal.PtrToStringUni
                        (new IntPtr(buffer.ToInt64() + (long)current_offset + 24L), (int)current_info.StreamNameLength / 2);

                    //add to result
                    ret_list.Add(current_info);

                    if (current_info.NextEntryOffset == 0)
                    {
                        //that is last member
                        break;
                    }
                    else
                    {
                        current_offset += (int)current_info.NextEntryOffset;
                    }
                }
                return ret_list.ToArray();
            }
            finally
            {
                if (need_free_buffer)
                {
                    if (buffer != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(buffer);
                    }
                }
            }
        }

        public static NT_FILE_STANDARD_INFORMATION GetFileInfo_standard(string file_name)
        {
            var file_handle = IntPtr.Zero;
            try
            {
                file_handle = WinApiFS.CreateFile_intptr
                    (file_name,
                    Win32FileAccess.GENERIC_READ,
                    FileShare.ReadWrite,
                    IntPtr.Zero,
                    FileMode.Open,
                    CreateFileOptions.None,
                    IntPtr.Zero);
                if (file_handle.ToInt64() == WinApiFS.INVALID_HANDLE_VALUE)
                {
                    var win_err = Marshal.GetLastWin32Error();
                    Exception ex = new Win32Exception(win_err);
                    throw ex;
                }

                var ret = GetFileInfo_standard(file_handle);
                return ret;
            }
            finally
            {
                if ((file_handle != IntPtr.Zero) && (file_handle.ToInt64() != WinApiFS.INVALID_HANDLE_VALUE))
                {
                    WinApiFS.CloseHandle(file_handle);
                }
            }
        }

        public static NT_FILE_STANDARD_INFORMATION GetFileInfo_standard(IntPtr file_handle)
        {
            var ret_ptr = IntPtr.Zero;
            try
            {
                var status_block = new IO_STATUS_BLOCK();
                var ret_len = Marshal.SizeOf(typeof(NT_FILE_STANDARD_INFORMATION));
                ret_ptr = Marshal.AllocHGlobal(ret_len);
                var res = ntApiFS.NtQueryInformationFile
                    (file_handle,
                    ref status_block,
                    ret_ptr,
                    ret_len,
                    FILE_INFORMATION_CLASS.FileStandardInformation);
                NTSTATUS_helper.ThrowOnError(res, status_block, NTSTATUS_severity.Warning);
                var ret = (NT_FILE_STANDARD_INFORMATION)Marshal.PtrToStructure
                       (ret_ptr,
                       typeof(NT_FILE_STANDARD_INFORMATION));
                return ret;
            }
            finally
            {
                if (ret_ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ret_ptr);
                }
            }
        }

        public static NT_FILE_BASIC_INFO GetFileInfo_basic(IntPtr file_handle)
        {
            var ret_ptr = IntPtr.Zero;
            try
            {
                //нинциализируем IO_STATUS
                var status_block = new IO_STATUS_BLOCK();

                //инициализируем NT_FILE_BASIC_INFO
                var ret_len = Marshal.SizeOf(typeof(NT_FILE_BASIC_INFO));
                ret_ptr = Marshal.AllocHGlobal(ret_len);

                //вызываем - по некторым сведениям вызов может в некоторых случаях блокироваться до
                //бесконечности!
                var res = ntApiFS.NtQueryInformationFile
                    (file_handle,
                    ref status_block,
                    ret_ptr,
                    ret_len,
                    FILE_INFORMATION_CLASS.FileBasicInformation);

                //обрабатываем исключения
                NTSTATUS_helper.ThrowOnError(res, status_block, NTSTATUS_severity.Warning);

                //получаем управляемую структуру из указателя
                var ret = (NT_FILE_BASIC_INFO)Marshal.PtrToStructure
                   (ret_ptr,
                   typeof(NT_FILE_BASIC_INFO));

                return ret;
            }
            finally
            {
                if (ret_ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ret_ptr);
                }
            }
        }

        public static NT_FILE_BASIC_INFO GetFileInfo_basic(string file_name)
        {
            var file_handle = IntPtr.Zero;

            try
            {
                //open file
                file_handle = WinApiFS.CreateFile_intptr
                    (file_name,
                    Win32FileAccess.GENERIC_READ,
                    FileShare.ReadWrite,
                    IntPtr.Zero,
                    FileMode.Open,
                    CreateFileOptions.BACKUP_SEMANTICS,
                    IntPtr.Zero);

                if (file_handle.ToInt64() == WinApiFS.INVALID_HANDLE_VALUE)
                {
                    var win_err = Marshal.GetLastWin32Error();
                    Exception ex = new Win32Exception(win_err);
                    throw ex;
                }

                var ret = GetFileInfo_basic(file_handle);

                return ret;
            }
            finally
            {
                if ((file_handle != IntPtr.Zero) && (file_handle.ToInt64() != WinApiFS.INVALID_HANDLE_VALUE))
                {
                    WinApiFS.CloseHandle(file_handle);
                }
            }
        }

        /// <summary>
        /// currently BUGGY - not use, use CreateFile instead
        /// </summary>
        /// <param name="file_name"></param>
        /// <param name="file_access"></param>
        /// <param name="file_share"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IntPtr CreateFileHandle
            (string file_name,
            Win32FileAccess file_access,
            FileShare file_share,
            CreateFileOptions options)
        {
            var ret = IntPtr.Zero;
            var obj_attr_ptr = IntPtr.Zero;
            var file_name_struct_ptr = IntPtr.Zero;
            var obj_attr=new OBJECT_ATTRIBUTES();

            try
            {
                //выделяем память под OBJECT_ATTRIBUTES
                var obj_attr_len = Marshal.SizeOf(typeof(OBJECT_ATTRIBUTES));
                obj_attr_ptr = Marshal.AllocHGlobal(obj_attr_len);

                //инициализируем OBJECT_ATTRIBUTES
                obj_attr = new OBJECT_ATTRIBUTES(IOhelper.GetKernelFileName(file_name), (uint)ntApiFS.OBJ_CASE_INSENSITIVE);
                Marshal.StructureToPtr(obj_attr, obj_attr_ptr, true);

                //инициализируем IO_STATUS_BLOCK
                var status_block = new IO_STATUS_BLOCK();

                //открываем...
                var res = ntApiFS.NtOpenFile
                    (ref ret,
                    file_access,
                    obj_attr_ptr,
                    ref status_block,
                    (ulong)file_share,
                    (ulong)options);

                NTSTATUS_helper.ThrowOnError(res, status_block, NTSTATUS_severity.Warning);
                return ret;
            }
            finally
            {
                obj_attr.Dispose();

            }
        }

        public class FileStream_enum : IEnumerable<NT_FILE_STREAM_INFORMATION>
        {
            private IntPtr buffer = IntPtr.Zero;
            private string file_name = string.Empty;
            private IntPtr file_handle = IntPtr.Zero;
            private int buffer_size = 0;

            public FileStream_enum(string file_name)
            {
                this.file_name = file_name;
            }

            public FileStream_enum(IntPtr file_handle, IntPtr buffer, int buffer_size)
            {
                this.file_handle = file_handle;
                this.buffer = buffer;
                this.buffer_size = buffer_size;
            }

            #region IEnumerable<NT_FILE_STREAM_INFORMATION> Members

            public IEnumerator<NT_FILE_STREAM_INFORMATION> GetEnumerator()
            {
                if (file_name == string.Empty)
                {
                    return new InternalEnumerator(file_handle, buffer, buffer_size);
                }
                else
                {
                    return new InternalEnumerator(file_name);
                }
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            private class InternalEnumerator : IEnumerator<NT_FILE_STREAM_INFORMATION>
            {
                private IntPtr file_h = IntPtr.Zero;
                private IntPtr buffer_h = IntPtr.Zero;
                private bool need_free_buffer_h = false;
                private bool need_free_file_h = false;
                private int current_offset = 0;
                private NT_FILE_STREAM_INFORMATION current_info;
                private bool end = false;

                public InternalEnumerator(IntPtr file_handle, IntPtr buffer, int buffer_size)
                {
                    file_h = file_handle;

                    if (buffer_size == 0)
                    {
                        buffer_size = STREAM_INFO_BUFFER_LEN;
                    }
                    if (buffer == IntPtr.Zero)
                    {
                        need_free_buffer_h = true;
                        buffer = Marshal.AllocHGlobal(buffer_size);
                    }
                    buffer_h = buffer;

                    Marshal.WriteInt32(buffer, 4, 0);

                    var status = new IO_STATUS_BLOCK();
                    
                    var res = ntApiFS.NtQueryInformationFile
                    (file_h,
                    ref status,
                    buffer_h,
                    buffer_size,
                    FILE_INFORMATION_CLASS.FileStreamInformation);

                    //check result
                    NTSTATUS_helper.ThrowOnError(res, status, NTSTATUS_severity.Warning);
                }

                public InternalEnumerator(string file_name)
                {
                    file_h = WinApiFS.CreateFile_intptr
                    (file_name,
                    Win32FileAccess.GENERIC_READ,
                    FileShare.ReadWrite,
                    IntPtr.Zero,
                    FileMode.Open,
                    CreateFileOptions.BACKUP_SEMANTICS, //need for retrieve info about directory
                    IntPtr.Zero);
                    if (file_h.ToInt64() == WinApiFS.INVALID_HANDLE_VALUE)
                    {
                        var win_err = Marshal.GetLastWin32Error();
                        Exception ex = new Win32Exception(win_err);
                        throw ex;
                    }
                    need_free_file_h = true;
                    
                    var buffer_size = STREAM_INFO_BUFFER_LEN;
                    need_free_buffer_h = true;
                    buffer_h = Marshal.AllocHGlobal(buffer_size);

                    Marshal.WriteInt32(buffer_h, 4, 0);

                    var status = new IO_STATUS_BLOCK();

                    var res = ntApiFS.NtQueryInformationFile
                    (file_h,
                    ref status,
                    buffer_h,
                    buffer_size,
                    FILE_INFORMATION_CLASS.FileStreamInformation);

                    //check result
                    NTSTATUS_helper.ThrowOnError(res, status, NTSTATUS_severity.Warning);
                }

                #region IEnumerator<NT_FILE_STREAM_INFORMATION> Members

                public NT_FILE_STREAM_INFORMATION Current
                {
                    get
                    {
                        return current_info;
                    }
                }

                #endregion

                #region IDisposable Members

                public void Dispose()
                {
                    if (need_free_buffer_h)
                    {
                        if (buffer_h != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(buffer_h);
                        }
                    }
                    if (need_free_file_h)
                    {
                        if ((file_h != IntPtr.Zero) || (file_h.ToInt64() != WinApiFS.INVALID_HANDLE_VALUE))
                        {
                            WinApiFS.CloseHandle(file_h);
                        }
                    }
                }

                #endregion

                #region IEnumerator Members

                object System.Collections.IEnumerator.Current
                {
                    get { return current_info; }
                }

                public bool MoveNext()
                {
                    if (end)
                    {
                        return false;
                    }

                    current_info = new NT_FILE_STREAM_INFORMATION();

                    //first 4 bytes - offset to next entry
                    current_info.NextEntryOffset = Marshal.ReadInt32(buffer_h, current_offset);

                    //next 4 bytes - stream name len (in bytes)
                    current_info.StreamNameLength = Marshal.ReadInt32(buffer_h, current_offset + 4);

                    if (current_info.StreamNameLength == 0)
                    {
                        //that is no streams at all (as directory without AFS)
                        return false;
                    }

                    //next 8 bytes - stream size
                    current_info.StreamSize = Marshal.ReadInt64(buffer_h, current_offset + 8);

                    //next 8 bytes - stream allocastion size
                    current_info.StreamAllocationSize = Marshal.ReadInt64(buffer_h, current_offset + 16);

                    //next StreamNameLength bytes - unicode stream name
                    current_info.StreamName = Marshal.PtrToStringUni
                        (new IntPtr(buffer_h.ToInt64() + (long)current_offset + 24L),
                        (int)current_info.StreamNameLength / 2);

                    if (current_info.NextEntryOffset == 0)
                    {
                        end = true;
                    }
                    else
                    {
                        current_offset += current_info.NextEntryOffset;
                    }

                    return true;
                }

                public void Reset()
                {
                    throw new NotImplementedException();
                }

                #endregion
            }
        }
    }

    
}
