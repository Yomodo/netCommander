using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace netCommander
{
    public class DirectoryListFilter
    {
        public void Load()
        {
            SettingsDeserialize(Options.FilterDirectory);
        }

        public void Save()
        {
            Options.FilterDirectory = SettingsSerialize();
        }

        public string[] Masks { get; set; }
        public bool IgnoreFileAttributes { get; set; }
        public FileAttributes FileAttributes { get; set; }
        public FilterSizeCriteria FilterSizeCriteria { get; set; }
        public long SizeMaximum { get; set; }
        public long SizeMinimum { get; set; }
        public bool IgnoreSize { get; set; }
        public bool IgnoreTimeModification { get; set; }
        public bool IgnoreTimeCreate { get; set; }
        public bool  IgnoreTimeAccess { get; set; }
        public DateTime ModificationBegin { get; set; }
        public DateTime  ModificationEnd { get; set; }
        public DateTime CreateBegin { get; set; }
        public DateTime CreateEnd { get; set; }
        public DateTime AccessBegin { get; set; }
        public DateTime AccessEnd { get; set; }

        public bool InCurrentDirectory { get; set; }
        public bool InCurrentDirectoryWithSubdirs { get; set; }
        public bool InCurrentDrive { get; set; }
        public bool InFixedDrives { get; set; }
        public bool InRemovableDrives { get; set; }
        public bool InNetworkDrives { get; set; }
        public string CurrentDirectory { get; set; }
        public string CurrentDrive { get; set; }

        private void SettingsDeserialize(byte[] settings)
        {
            if (settings.Length == 0)
            {
                fill_defaults();
                return;
            }

            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream(settings);

                //mask
                var int_bytes = new byte[4];
                ms.Read(int_bytes, 0, 4);
                var mask_len = BitConverter.ToInt64(int_bytes, 0);
                var mask_bytes = new byte[mask_len];
                ms.Read(mask_bytes, 0, (int) mask_len);
                var mask_string = Encoding.Unicode.GetString(mask_bytes);
                var masks = mask_string.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                Masks = masks;

                //ignore attr
                var bool_bytes = new byte[1];
                ms.Read(bool_bytes, 0, 1);
                IgnoreFileAttributes= BitConverter.ToBoolean(bool_bytes, 0);

                //attributes
                ms.Read(int_bytes, 0, 4);
                var attr_int = BitConverter.ToInt64(int_bytes, 0);
                var fa = (FileAttributes)attr_int;
                FileAttributes = fa;

                //ignore size
                ms.Read(bool_bytes, 0, 1);
                IgnoreSize = BitConverter.ToBoolean(bool_bytes, 0);

                //size criteria
                ms.Read(int_bytes, 0, 4);
                FilterSizeCriteria = (FilterSizeCriteria)BitConverter.ToInt64(int_bytes, 0);

                //size min
                var long_bytes = new byte[8];
                ms.Read(long_bytes, 0, 8);
                SizeMinimum = BitConverter.ToInt64(long_bytes, 0);

                //size max
                ms.Read(long_bytes, 0, 8);
                SizeMaximum = BitConverter.ToInt64(long_bytes, 0);

                //ignore create
                ms.Read(bool_bytes, 0, 1);
                IgnoreTimeCreate = BitConverter.ToBoolean(bool_bytes, 0);

                //create begin
                ms.Read(long_bytes, 0, 8);
                CreateBegin = DateTime.FromFileTime(BitConverter.ToInt64(long_bytes, 0));

                //create end
                ms.Read(long_bytes, 0, 8);
                CreateEnd = DateTime.FromFileTime(BitConverter.ToInt64(long_bytes, 0));

                //ignore modif
                ms.Read(bool_bytes, 0, 1);
                IgnoreTimeModification = BitConverter.ToBoolean(bool_bytes, 0);

                //modif begin
                ms.Read(long_bytes, 0, 8);
                ModificationBegin = DateTime.FromFileTime(BitConverter.ToInt64(long_bytes, 0));

                //modif end
                ms.Read(long_bytes, 0, 8);
                ModificationEnd = DateTime.FromFileTime(BitConverter.ToInt64(long_bytes, 0));

                //ignore access
                ms.Read(bool_bytes, 0, 1);
                IgnoreTimeAccess = BitConverter.ToBoolean(bool_bytes, 0);

                //access begin
                ms.Read(long_bytes, 0, 8);
                AccessBegin = DateTime.FromFileTime(BitConverter.ToInt64(long_bytes, 0));

                //access end
                ms.Read(long_bytes, 0, 8);
                AccessEnd = DateTime.FromFileTime(BitConverter.ToInt64(long_bytes, 0));

                //current directory only
                ms.Read(bool_bytes, 0, 1);
                InCurrentDirectory = BitConverter.ToBoolean(bool_bytes, 0);
              
                //include subdirs
                ms.Read(bool_bytes, 0, 1);
                InCurrentDirectoryWithSubdirs = BitConverter.ToBoolean(bool_bytes, 0);

                //current drive
                ms.Read(bool_bytes, 0, 1);
                InCurrentDrive = BitConverter.ToBoolean(bool_bytes, 0);

                //all drives
                ms.Read(bool_bytes, 0, 1);
                //this. = BitConverter.ToBoolean(bool_bytes, 0);

                //fixed drives
                ms.Read(bool_bytes, 0, 1);
                InFixedDrives = BitConverter.ToBoolean(bool_bytes, 0);

                //removable drives
                ms.Read(bool_bytes, 0, 1);
                InRemovableDrives = BitConverter.ToBoolean(bool_bytes, 0);

                //network drives
                ms.Read(bool_bytes, 0, 1);
                InNetworkDrives = BitConverter.ToBoolean(bool_bytes, 0);
            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message+" Failed to read settings. Load defaults.");
                fill_defaults();
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                }
            }
        }

        private byte[] SettingsSerialize()
        {
            var ms = new System.IO.MemoryStream();

            //4 byte - length of mask
            var mask_string = string.Empty;
            for (var i = 0; i < Masks.Length - 1; i++)
            {
                mask_string = mask_string + Masks[i] + ";";
            }
            mask_string = mask_string + Masks[Masks.Length - 1];
            var mask_len = Encoding.Unicode.GetByteCount(mask_string);
            var mask_len_bytes = BitConverter.GetBytes(mask_len);
            ms.Write(mask_len_bytes, 0, 4);

            //mask_len bytes - mask
            var mask_bytes = Encoding.Unicode.GetBytes(mask_string);
            ms.Write(mask_bytes, 0, mask_len);

            //ignore attributes
            var ignore_attr_bytes = BitConverter.GetBytes(IgnoreFileAttributes);
            ms.Write(ignore_attr_bytes, 0, 1);

            //attributes
            //FileAttributes fa = (int)flagBoxFileAttributes.FlagValue;
            var fa_uint = (uint)FileAttributes;
            var fa_bytes = BitConverter.GetBytes(fa_uint);
            ms.Write(fa_bytes, 0, 4);

            //ignore size
            var ignore_size_bytes = BitConverter.GetBytes(IgnoreSize);
            ms.Write(ignore_size_bytes, 0, 1);

            //size criteria
            var size_criteria_bytes = BitConverter.GetBytes((int)FilterSizeCriteria);
            ms.Write(size_criteria_bytes, 0, 4);

            //size minimum
            var size_min = SizeMinimum;
            var size_min_bytes = BitConverter.GetBytes(size_min);
            ms.Write(size_min_bytes, 0, 8);

            //size maximum
            var size_max = SizeMaximum;
            var size_max_bytes = BitConverter.GetBytes(size_max);
            ms.Write(size_max_bytes, 0, 8);

            //ignore create time
            var ignore_create_bytes = BitConverter.GetBytes(IgnoreTimeCreate);
            ms.Write(ignore_create_bytes, 0, 1);

            //create begin
            var create_begin_bytes = BitConverter.GetBytes(CreateBegin.ToFileTime());
            ms.Write(create_begin_bytes, 0, 8);

            //create end
            var create_end_bytes = BitConverter.GetBytes(CreateEnd.ToFileTime());
            ms.Write(create_end_bytes, 0, 8);

            //ignore modification
            var ignore_modif_bytes = BitConverter.GetBytes(IgnoreTimeModification);
            ms.Write(ignore_modif_bytes, 0, 1);

            //modification begin
            var modif_begin_bytes = BitConverter.GetBytes(ModificationBegin.ToFileTime());
            ms.Write(modif_begin_bytes, 0, 8);

            //modification end
            var modif_end_bytes = BitConverter.GetBytes(ModificationEnd.ToFileTime());
            ms.Write(modif_end_bytes, 0, 8);

            //ignore access
            var ignore_access_bytes = BitConverter.GetBytes(IgnoreTimeAccess);
            ms.Write(ignore_access_bytes, 0, 1);

            //access begin
            var access_begin_bytes = BitConverter.GetBytes(AccessBegin.ToFileTime());
            ms.Write(access_begin_bytes, 0, 8);

            //access end
            var access_end_bytes = BitConverter.GetBytes(AccessEnd.ToFileTime());
            ms.Write(access_end_bytes, 0, 8);

            //current directory only
            var current_dir_only_bytes = BitConverter.GetBytes(InCurrentDirectory);
            ms.Write(current_dir_only_bytes, 0, 1);

            //include subdirs
            var include_subdirs_bytes = BitConverter.GetBytes(InCurrentDirectoryWithSubdirs);
            ms.Write(include_subdirs_bytes, 0, 1);

            //current drive
            var current_drive_bytes = BitConverter.GetBytes(InCurrentDrive);
            ms.Write(current_dir_only_bytes, 0, 1);

            //all drives
            //byte[] all_drives_bytes = BitConverter.GetBytes(radioButtonLocalDrives.Checked);
            var all_drives_bytes = new byte[] { 0 };
            ms.Write(all_drives_bytes, 0, 1);

            //fixed drives
            var fixed_drives_bytes = BitConverter.GetBytes(InFixedDrives);
            ms.Write(fixed_drives_bytes, 0, 1);

            //removable drives
            var removable_drives_bytes = BitConverter.GetBytes(InRemovableDrives);
            ms.Write(removable_drives_bytes, 0, 1);

            //network drives
            var network_drives_bytes = BitConverter.GetBytes(InNetworkDrives);
            ms.Write(network_drives_bytes, 0, 1);

            ms.Position = 0;
            var ms_len = (int)ms.Length;
            var ret = new byte[ms_len];
            ms.Read(ret, 0, ms_len);

            ms.Close();

            return ret;
        }

        private void fill_defaults()
        {
            Masks = new string[] { "*" };
            SizeMaximum = 0;
            SizeMinimum = 0;
            FilterSizeCriteria = FilterSizeCriteria.Greater;
            IgnoreFileAttributes = true;
            IgnoreSize = true;
            IgnoreTimeAccess = true;
            IgnoreTimeCreate = true;
            IgnoreTimeModification = true;
            FileAttributes = FileAttributes.Normal;
            AccessBegin = DateTime.Now.AddMonths(-1);
            AccessEnd = DateTime.Now;
            CreateBegin = DateTime.Now.AddMonths(-1);
            CreateEnd = DateTime.Now;
            ModificationBegin = DateTime.Now.AddMonths(-1);
            ModificationEnd = DateTime.Now;

            CurrentDirectory = string.Empty;
            InCurrentDirectory = true;
        }
    }

    public enum FilterSizeCriteria
    {
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual,
        Equal,
        Between
    }
}
