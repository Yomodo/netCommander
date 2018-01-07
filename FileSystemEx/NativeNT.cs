﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace netCommander.FileSystemEx
{
    public class ntApiFS
    {
        private ntApiFS() { }

        [DllImport("ntdll.dll", SetLastError = false)]
        public static extern uint NtQueryVolumeInformationFile
            (IntPtr FileHandle,
            ref IO_STATUS_BLOCK IoStatusBlock,
            IntPtr FileSystemInformation,
            ulong Length,
            FS_INFORMATION_CLASS FileSystemInformationClass);

        [DllImport("ntdll.dll", SetLastError = false)]
        public static extern uint NtQueryVolumeInformationFile
            (IntPtr FileHandle,
            ref IO_STATUS_BLOCK IoStatusBlock,
            IntPtr FileSystemInformation,
            int Length,
            FS_INFORMATION_CLASS FileSystemInformationClass);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint NtQueryInformationFile
            (IntPtr fileHandle,
            ref IO_STATUS_BLOCK IoStatusBlock,
            IntPtr pInfoBlock,
            ulong length,
            FILE_INFORMATION_CLASS fileInformation);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint NtQueryInformationFile
            (IntPtr fileHandle,
            ref IO_STATUS_BLOCK IoStatusBlock,
            IntPtr pInfoBlock,
            int length,
            FILE_INFORMATION_CLASS fileInformation);

        //mot needed - CreateFile returns handle
        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint NtOpenFile
            (ref IntPtr FileHandle,
            Win32FileAccess DesiredAccess,
            IntPtr ObjectAttributes_ptr,
            ref IO_STATUS_BLOCK IoStatusBlock,
            ulong ShareAccess,
            ulong OpenOptions);

        //[DllImport("ntdll.dll", SetLastError = true)]
        //public static extern uint NtClose(IntPtr Handle);


        public const uint STATUS_SUCCESS = 0x00000000;

        public const ulong OBJ_CASE_INSENSITIVE = 0x00000040;
        public const ulong OBJ_INHERIT = 0x00000002;
        public const ulong OBJ_PERMANENT = 0x00000010;
        public const ulong OBJ_EXCLUSIVE = 0x00000020;
        public const ulong OBJ_OPENIF = 0x00000080;
        public const ulong OBJ_OPENLINK = 0x00000100;
        public const ulong OBJ_KERNEL_HANDLE = 0x00000200;
        public const ulong OBJ_FORCE_ACCESS_CHECK = 0x00000400;
        public const ulong OBJ_VALID_ATTRIBUTES = 0x000007F2;

        public const int FILE_DEVICE_CD_ROM = 0x00000002;
        public const int FILE_DEVICE_DISK = 0x00000007;

        public const int FILE_REMOVABLE_MEDIA = 0x00000001;
        public const int FILE_READ_ONLY_DEVICE = 0x00000002;
        public const int FILE_FLOPPY_DISKETTE = 0x00000004;
        public const int FILE_WRITE_ONCE_MEDIA = 0x00000008;
        public const int FILE_REMOTE_DEVICE = 0x00000010;
        public const int FILE_DEVICE_IS_MOUNTED = 0x00000020;
        public const int FILE_VIRTUAL_VOLUME = 0x00000040;
        public const int FILE_AUTOGENERATED_DEVICE_NAME = 0x00000080;
        public const int FILE_DEVICE_SECURE_OPEN = 0x00000100;
        public const int FILE_CHARACTERISTIC_PNP_DEVICE = 0x00000800;
        public const int FILE_CHARACTERISTIC_TS_DEVICE = 0x00001000;
        public const int FILE_CHARACTERISTIC_WEBDAV_DEVICE = 0x00002000;

        public const uint FILE_VC_CONTENT_INDEX_DISABLED = 0x00000008;
        public const uint FILE_VC_LOG_QUOTA_LIMIT = 0x00000020;
        public const uint FILE_VC_LOG_QUOTA_THRESHOLD = 0x00000010;
        public const uint FILE_VC_LOG_VOLUME_LIMIT = 0x00000080;
        public const uint FILE_VC_LOG_VOLUME_THRESHOLD = 0x00000040;
        public const uint FILE_VC_QUOTA_ENFORCE = 0x00000002;
        public const uint FILE_VC_QUOTA_TRACK = 0x00000001;
        public const uint FILE_VC_QUOTAS_INCOMPLETE = 0x00000100;
        public const uint FILE_VC_QUOTAS_REBUILDING = 0x00000200;

        public const int MAXIMUM_REPARSE_DATA_BUFFER_SIZE = 16 * 1024;
        public const int REPARSE_DATA_BUFFER_HEADER_SIZE = 8;
        public const uint SYMLINK_FLAG_RELATIVE = 1;

    }


    [StructLayout(LayoutKind.Sequential)]
    public struct OBJECT_ATTRIBUTES : IDisposable
    {
        public int Length;
        public IntPtr RootDirectory;
        private IntPtr objectName;
        public uint Attributes;
        public IntPtr SecurityDescriptor;
        public IntPtr SecurityQualityOfService;

        public OBJECT_ATTRIBUTES(string name, uint attrs)
        {
            Length = 0;
            RootDirectory = IntPtr.Zero;
            objectName = IntPtr.Zero;
            Attributes = attrs;
            SecurityDescriptor = IntPtr.Zero;
            SecurityQualityOfService = IntPtr.Zero;

            Length = Marshal.SizeOf(this);
            ObjectName = new UNICODE_STRING(name);
        }

        public UNICODE_STRING ObjectName
        {
            get
            {
                return (UNICODE_STRING)Marshal.PtrToStructure(
                 objectName, typeof(UNICODE_STRING));
            }

            set
            {
                var fDeleteOld = objectName != IntPtr.Zero;
                if (!fDeleteOld)
                    objectName = Marshal.AllocHGlobal(Marshal.SizeOf(value));
                Marshal.StructureToPtr(value, objectName, fDeleteOld);
            }
        }

        public void Dispose()
        {
            if (objectName != IntPtr.Zero)
            {
                Marshal.DestroyStructure(objectName, typeof(UNICODE_STRING));
                Marshal.FreeHGlobal(objectName);
                objectName = IntPtr.Zero;
            }
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct UNICODE_STRING : IDisposable
    {
        public ushort Length;
        public ushort MaximumLength;
        private IntPtr buffer;

        public UNICODE_STRING(string s)
        {
            Length = (ushort)(s.Length * 2);
            MaximumLength = (ushort)(Length + 2);
            buffer = Marshal.StringToHGlobalUni(s);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(buffer);
            buffer = IntPtr.Zero;
        }

        public override string ToString()
        {
            return Marshal.PtrToStringUni(buffer);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct IO_STATUS_BLOCK
    {
        public uint Status;
        public IntPtr Pointer;
        public ulong Information;

        public NT_STATUS Check_Status()
        {
            if (Status < 0x80000000)
            {
                return NT_STATUS.Success;
            }

            if (Status < 0xC0000000)
            {
                return NT_STATUS.Warning;
            }

            return NT_STATUS.Error;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FILE_FS_ATTRIBUTE_INFORMATION
    {
        public VolumeCaps VolumeAttributes;
        public int MaximumComponentNameLength;
        public uint FileSystemNameLength;
        public string FileSystemName;

        public static FILE_FS_ATTRIBUTE_INFORMATION FromBuffer(IntPtr buffer)
        {
            var ret = new FILE_FS_ATTRIBUTE_INFORMATION();

            ret.VolumeAttributes = (VolumeCaps)IOhelper.PtrToUint(buffer, 0);
            ret.MaximumComponentNameLength = Marshal.ReadInt32(buffer, 4);
            ret.FileSystemNameLength = IOhelper.PtrToUint(buffer, 8);
            ret.FileSystemName = IOhelper.PtrToStringUni(buffer, 12, ((int)ret.FileSystemNameLength)/2);

            return ret;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FILE_FS_VOLUME_INFORMATION
    {
        public long VolumeCreationTime;
        public uint VolumeSerialNumber;
        public int VolumeLabelLength;
        public byte SupportsObjects;
        public byte Reserved;
        public string VolumeLabel;

        public static FILE_FS_VOLUME_INFORMATION FromBuffer(IntPtr buffer)
        {
            var ret = new FILE_FS_VOLUME_INFORMATION();

            ret.VolumeCreationTime = Marshal.ReadInt64(buffer, 0);
            ret.VolumeSerialNumber = IOhelper.PtrToUint(buffer, 8);
            ret.VolumeLabelLength = Marshal.ReadInt32(buffer, 12);
            ret.SupportsObjects = Marshal.ReadByte(buffer, 13);

            //pack=4
            ret.VolumeLabel = IOhelper.PtrToStringUni(buffer, 18, ret.VolumeLabelLength / 2);

            return ret;
        }

        public DateTime VolumeCreationDateTime
        {
            get
            {
                return DateTime.FromFileTime(VolumeCreationTime);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FILE_FS_SIZE_INFORMATION
    {
        public long TotalAllocationUnits;
        public long ActualAvailableAllocationUnits;
        public uint SectorsPerAllocationUnit;
        public uint BytesPerSector;

        public long TotalAllocationSectors
        {
            get
            {
                return TotalAllocationUnits * SectorsPerAllocationUnit;
            }
        }

        public long TotalAllocationBytes
        {
            get
            {
                return TotalAllocationSectors * BytesPerSector;
            }
        }

        public long ActualAvailableAllocationSectors
        {
            get
            {
                return ActualAvailableAllocationUnits * SectorsPerAllocationUnit;
            }
        }

        public long ActualAvailableAllocationBytes
        {
            get
            {
                return ActualAvailableAllocationSectors * BytesPerSector;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FILE_FS_DEVICE_INFORMATION
    {
        public NT_FS_DEVICE_TYPE DeviceType;
        public NT_FS_DEVICE_CHARACTERISTICS Characteristics;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FILE_FS_CONTROL_INFORMATION
    {
        /// <summary>
        /// This value SHOULD be set to 0, and MUST be ignored.
        /// </summary>
        public long FreeSpaceStartFiltering;

        /// <summary>
        /// This value SHOULD be set to 0, and MUST be ignored.
        /// </summary>
        public long FreeSpaceThreshold;

        /// <summary>
        /// This value SHOULD be set to 0, and MUST be ignored.
        /// </summary>
        public long FreeSpaceStopFiltering;

        public long DefaultQuotaThreshold;
        public long DefaultQuotaLimit;
        public NT_FS_CONTROL_FLAGS FileSystemControlFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FILE_FS_FULLSIZE_INFORMATION
    {
        public long TotalAllocationUnits;
        public long CallerAvailableAllocationUnits;
        /// <summary>
        /// total
        /// </summary>
        public long ActualAvailableAllocationUnits;
        public uint SectorsPerAllocationUnit;
        public uint BytesPerSector;

        public long TotalAllocationUserSectors
        {
            get
            {
                return TotalAllocationUnits * SectorsPerAllocationUnit;
            }
        }

        public long TotalAllocationUserBytes
        {
            get
            {
                return TotalAllocationUserSectors * BytesPerSector;
            }
        }

        public long UserAvailableAllocationSectors
        {
            get
            {
                return CallerAvailableAllocationUnits * SectorsPerAllocationUnit;
            }
        }

        public long UserAvailableAllocationBytes
        {
            get
            {
                return UserAvailableAllocationSectors * BytesPerSector;
            }
        }

        public long TotalAvailableAllocationSectors
        {
            get
            {
                return ActualAvailableAllocationUnits * SectorsPerAllocationUnit;
            }
        }

        public long TotalAvailableAllocationBytes
        {
            get
            {
                return TotalAvailableAllocationSectors * BytesPerSector;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NT_FILE_STREAM_INFORMATION
    {
        public int NextEntryOffset;
        /// <summary>
        /// in bytes!
        /// </summary>
        public int StreamNameLength;
        public long StreamSize;
        public long StreamAllocationSize;
        public string StreamName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NT_FILE_STANDARD_INFORMATION
    {
        public long AllocationSize;
        public long EndOfFile;
        public int NumberOfLinks;
        public byte DeletePending;
        public byte IsDirectory;
        public short Reserved;
    }

    [StructLayout(LayoutKind.Sequential,Pack=1)]
    public struct NT_FILE_BASIC_INFO
    {
        public long CreationFileTime;
        public long LastAccessFileTime;
        public long LastWriteFileTime;
        public long ChangeFileTime;
        public uint Attributes;
        public int Reserved;

        /// <summary>
        /// not all of attributes returns
        /// </summary>
        public FileAttributes FileAttributes
        {
            get
            {
                return (FileAttributes)Attributes;
            }
        }

        public DateTime CreationTime
        {
            get
            {
                return DateTime.FromFileTime(CreationFileTime);
            }
        }

        public DateTime LastAccessTime
        {
            get
            {
                return DateTime.FromFileTime(LastAccessFileTime);
            }
        }

        public DateTime LastWriteTime
        {
            get
            {
                return DateTime.FromFileTime(LastWriteFileTime);
            }
        }

        public DateTime ChangeTime
        {
            get
            {
                return DateTime.FromFileTime(ChangeFileTime);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NT_FILE_NAME_INFORMATION
    {
        public ulong FileNameLength;
        public string FileName;

        public NT_FILE_NAME_INFORMATION(int max_len)
        {
            FileNameLength = 0;
            FileName = new string(' ', max_len);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct REPARSE_DATA_BUFFER_SYMLINK
    {
        /* As defined MSDN http://msdn.microsoft.com/en-us/library/ms791514.aspx
         * Symbolic lync member of union used
        typedef struct _REPARSE_DATA_BUFFER {
            ULONG  ReparseTag;
            USHORT  ReparseDataLength;
            USHORT  Reserved;
            union {
                struct {
                    USHORT  SubstituteNameOffset;
                    USHORT  SubstituteNameLength;
                    USHORT  PrintNameOffset;
                    USHORT  PrintNameLength;
                    ULONG  Flags;
                    WCHAR  PathBuffer[1];
                } SymbolicLinkReparseBuffer;
                struct {
                    USHORT  SubstituteNameOffset;
                    USHORT  SubstituteNameLength;
                    USHORT  PrintNameOffset;
                    USHORT  PrintNameLength;
                    WCHAR  PathBuffer[1];
                } MountPointReparseBuffer;
                struct {
                    UCHAR  DataBuffer[1];
                } GenericReparseBuffer;
            };
        } REPARSE_DATA_BUFFER, *PREPARSE_DATA_BUFFER;
        */

        /*--------HEADER----------------*/
        public uint ReparseTag; //ULONG
        public ushort ReparseDataLength; //USHORT
        public ushort Reserved; //USHORT
        /*------------------------------*/


        /*--------DATA-BUFFER-----------*/
        public ushort SubstituteNameOffset; //USHORT
        public ushort SubstituteNameLength; //USHORT
        public ushort PrintNameOffset; //USHORT
        public ushort PrintNameLength; //USHORT

        public uint Flags; //ULONG

        //use MAX_PATH chars + 2 ending null
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (WinApiFS.MAX_PATH * 2 + 2) * sizeof(char))]
        public byte[] PathBuffer; //WCHAR[0]

        public static REPARSE_DATA_BUFFER_SYMLINK Init()
        {
            var ret = new REPARSE_DATA_BUFFER_SYMLINK();
            ret.PathBuffer = new byte[(WinApiFS.MAX_PATH * 2 + 2) * sizeof(char)];
            ret.ReparseDataLength = (ushort)(ret.PathBuffer.Length + 4 * sizeof(ushort) + 1 * sizeof(uint));
            return ret;
        }

        public string SubstituteName
        {
            get
            {
                return Encoding.Unicode.GetString(PathBuffer, SubstituteNameOffset, SubstituteNameLength);
            }
        }

        public string PrintName
        {
            get
            {
                return Encoding.Unicode.GetString(PathBuffer, PrintNameOffset, PrintNameLength);
            }
        }

        public void SetNames(string SubstituteName, string PrintName)
        {
            var subst_len = Encoding.Unicode.GetBytes(SubstituteName, 0, SubstituteName.Length, PathBuffer, 0);
            //add space for null symbol
            var prn_len = Encoding.Unicode.GetBytes(PrintName, 0, PrintName.Length, PathBuffer, subst_len + sizeof(char));

            SubstituteNameOffset = 0;
            SubstituteNameLength = (ushort)subst_len;
            PrintNameOffset = (ushort)(subst_len + sizeof(char));
            PrintNameLength = (ushort)prn_len;
            ReparseDataLength = (ushort)(subst_len + prn_len + 4 * sizeof(ushort) + 1 * sizeof(uint) + 2 * sizeof(char));
        }

        public bool Relative
        {
            get
            {
                return Flags == ntApiFS.SYMLINK_FLAG_RELATIVE;
            }
            set
            {
                Flags = value ? ntApiFS.SYMLINK_FLAG_RELATIVE : 0;
            }
        }
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct REPARSE_DATA_BUFFER_MOUNTPOINT
    {
        /* As defined MSDN http://msdn.microsoft.com/en-us/library/ms791514.aspx
         * Mount point member of union used
        typedef struct _REPARSE_DATA_BUFFER {
            ULONG  ReparseTag;
            USHORT  ReparseDataLength;
            USHORT  Reserved;
            union {
                struct {
                    USHORT  SubstituteNameOffset;
                    USHORT  SubstituteNameLength;
                    USHORT  PrintNameOffset;
                    USHORT  PrintNameLength;
                    ULONG  Flags;
                    WCHAR  PathBuffer[1];
                } SymbolicLinkReparseBuffer;
                struct {
                    USHORT  SubstituteNameOffset;
                    USHORT  SubstituteNameLength;
                    USHORT  PrintNameOffset;
                    USHORT  PrintNameLength;
                    WCHAR  PathBuffer[1];
                } MountPointReparseBuffer;
                struct {
                    UCHAR  DataBuffer[1];
                } GenericReparseBuffer;
            };
        } REPARSE_DATA_BUFFER, *PREPARSE_DATA_BUFFER;
        */
         
        /*--------HEADER----------------*/
        public uint ReparseTag; //ULONG
        public ushort ReparseDataLength; //USHORT
        public ushort Reserved; //USHORT
        /*------------------------------*/


        /*--------DATA-BUFFER-----------*/
        public ushort SubstituteNameOffset; //USHORT
        public ushort SubstituteNameLength; //USHORT
        public ushort PrintNameOffset; //USHORT
        public ushort PrintNameLength; //USHORT

        //use MAX_PATH chars + 2 ending null
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (WinApiFS.MAX_PATH * 2 + 2) * sizeof(char))]
        public byte[] PathBuffer; //WCHAR[0]

        public static REPARSE_DATA_BUFFER_MOUNTPOINT Init()
        {
            var ret = new REPARSE_DATA_BUFFER_MOUNTPOINT();
            ret.PathBuffer = new byte[(WinApiFS.MAX_PATH * 2 + 2) * sizeof(char)];
            ret.ReparseDataLength = (ushort)(ret.PathBuffer.Length + 4 * 2);
            return ret;
        }

        public string SubstituteName
        {
            get
            {
                return Encoding.Unicode.GetString(PathBuffer, SubstituteNameOffset, SubstituteNameLength);
            }
        }

        public string PrintName
        {
            get
            {
                return Encoding.Unicode.GetString(PathBuffer, PrintNameOffset, PrintNameLength);
            }
        }

        public void SetNames(string SubstituteName, string PrintName)
        {
            var subst_len = Encoding.Unicode.GetBytes(SubstituteName, 0, SubstituteName.Length, PathBuffer, 0);
            //add space for null symbol
            var prn_len = Encoding.Unicode.GetBytes(PrintName, 0, PrintName.Length, PathBuffer, subst_len + sizeof(char));

            SubstituteNameOffset = 0;
            SubstituteNameLength = (ushort)subst_len;
            PrintNameOffset = (ushort)(subst_len + sizeof(char));
            PrintNameLength = (ushort)prn_len;
            ReparseDataLength = (ushort)(subst_len + prn_len + 4 * 2 + 2 * sizeof(char));
        }
    }

    public enum NT_STATUS
    {
        Success,
        //Information,
        Warning,
        Error
    }

    public enum FILE_INFORMATION_CLASS
    {
        FileAccessInformation = 8,
        FileAlignmentInformation = 17,
        FileAllInformation = 18,
        FileAllocationInformation = 19,
        FileAlternateNameInformation = 21,
        FileAttributeTagInformation = 35,
        FileBasicInformation = 4,
        FileBothDirectoryInformation = 3,
        FileCompletionInformation = 30,
        FileCompressionInformation = 28,
        FileDirectoryInformation = 1,
        FileDispositionInformation = 13,
        FileEaInformation = 7,
        FileEndOfFileInformation = 20,
        FileFullDirectoryInformation = 2,
        FileFullEaInformation = 15,
        FileHardLinkInformation = 46,
        FileIdBothDirectoryInformation = 37,
        FileIdFullDirectoryInformation = 38,
        FileInternalInformation = 6,
        FileLinkInformation = 11,
        FileMailslotQueryInformation = 26,
        FileMailslotSetInformation = 27,
        FileModeInformation = 16,
        FileMoveClusterInformation = 31,
        FileNameInformation = 9,
        FileNamesInformation = 12,
        FileNetworkOpenInformation = 34,
        FileObjectIdInformation = 29,
        FilePipeInformation = 23,
        FilePipeLocalInformation = 24,
        FilePipeRemoteInformation = 25,
        FilePositionInformation = 14,
        FileQuotaInformation = 32,
        FileRenameInformation = 10,
        FileReparsePointInformation = 33,
        FileShortNameInformation = 40,
        FileStandardInformation = 5,
        FileStreamInformation = 22,
        FileTrackingInformation = 36,
        FileValidDataLengthInformation = 39,
    }

    public enum FS_INFORMATION_CLASS
    {
        FileFsVolumeInformation = 1,
        FileFsLabelInformation = 2,
        FileFsSizeInformation = 3,
        FileFsDeviceInformation = 4,
        FileFsAttributeInformation = 5,
        FileFsControlInformation = 6,
        FileFsFullSizeInformation = 7,
        FileFsObjectIdInformation = 8,
        FileFsMaximumInformation = 9
    }

    public enum NT_FS_DEVICE_TYPE
    {
        CD_ROM = ntApiFS.FILE_DEVICE_CD_ROM,
        DISK = ntApiFS.FILE_DEVICE_DISK
    }

    [Flags()]
    public enum NT_FS_DEVICE_CHARACTERISTICS
    {
        None=0,
        Removable = ntApiFS.FILE_REMOVABLE_MEDIA,
        Readonly = ntApiFS.FILE_READ_ONLY_DEVICE,
        Floppy = ntApiFS.FILE_FLOPPY_DISKETTE,
        WriteOnce = ntApiFS.FILE_WRITE_ONCE_MEDIA,
        Remote = ntApiFS.FILE_REMOTE_DEVICE,
        Mounted = ntApiFS.FILE_DEVICE_IS_MOUNTED,
        Virtual = ntApiFS.FILE_VIRTUAL_VOLUME,
        AutoGeneratedName = ntApiFS.FILE_AUTOGENERATED_DEVICE_NAME,
        SecureOpen = ntApiFS.FILE_DEVICE_SECURE_OPEN,
        PNP = ntApiFS.FILE_CHARACTERISTIC_PNP_DEVICE,
        TS = ntApiFS.FILE_CHARACTERISTIC_TS_DEVICE,
        WebDAV = ntApiFS.FILE_CHARACTERISTIC_WEBDAV_DEVICE
    }

    [Flags()]
    public enum NT_FS_CONTROL_FLAGS : uint
    {
        None=0,

        /// <summary>
        /// Content indexing is disabled.
        /// </summary>
        CONTENT_INDEX_DISABLED = ntApiFS.FILE_VC_CONTENT_INDEX_DISABLED,

        /// <summary>
        /// An event log entry will be created when the user exceeds his or her assigned disk quota limit.
        /// </summary>
        LOG_QUOTA_LIMIT = ntApiFS.FILE_VC_LOG_QUOTA_LIMIT,

        /// <summary>
        /// An event log entry will be created when the user exceeds his or her assigned quota warning threshold.
        /// </summary>
        LOG_QUOTA_THRESHOLD = ntApiFS.FILE_VC_LOG_QUOTA_THRESHOLD,

        /// <summary>
        /// An event log entry will be created when the volume's free space limit is exceeded.
        /// </summary>
        LOG_VOLUME_LIMIT = ntApiFS.FILE_VC_LOG_VOLUME_LIMIT,

        /// <summary>
        /// An event log entry will be created when the volume's free space threshold is exceeded.
        /// </summary>
        LOG_VOLUME_THRESHOLD = ntApiFS.FILE_VC_LOG_VOLUME_THRESHOLD,

        /// <summary>
        /// Quotas are tracked and enforced on the volume.
        /// </summary>
        QUOTA_ENFORCE = ntApiFS.FILE_VC_QUOTA_ENFORCE,

        /// <summary>
        /// Quotas are tracked on the volume, but they are not enforced. Tracked quotas enable reporting on the file system space used by system users. If both this field and FILE_VC_QUOTA_ENFORCE are specified, FILE_VC_QUOTA_ENFORCE is ignored.
        /// </summary>
        QUOTA_TRACK = ntApiFS.FILE_VC_QUOTA_TRACK,

        /// <summary>
        /// The quota information for the volume is incomplete because it is corrupt, or the system is in the process of rebuilding the quota information.
        /// </summary>
        QUOTAS_INCOMPLETE = ntApiFS.FILE_VC_QUOTAS_INCOMPLETE,

        QUOTAS_REBUILDING = ntApiFS.FILE_VC_QUOTAS_REBUILDING
    }

}