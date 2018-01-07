﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using netCommander.FileSystemEx;

namespace netCommander
{
    class PanelCommandFileAttributesEdit : PanelCommandBase
    {
    	// ctrl-a hides panel keyboard shortcut (see mFilePanel.ProcessDialogKey)
    	//ctrl-r now
        public PanelCommandFileAttributesEdit()
            : base(Options.GetLiteral(Options.LANG_FILE_ATTRIBUTES), Shortcut.CtrlR)
        {

        }

        protected override void internal_command_proc()
        {

            var e = new QueryPanelInfoEventArgs();
            OnQueryCurrentPanel(e);

            if (e.FocusedIndex == -1)
            {
                return;
            }

            var group_mode = false;
            var sel_indices = e.SelectedIndices;
            //we not need cache selection
            //becouse e.ItemsCollection must not change (DirectoryList not sort on attributes)
            group_mode = (sel_indices.Length > 1);
            var dl=(DirectoryList)e.ItemCollection;
            var f_data = new WIN32_FIND_DATA();

            var target_file = string.Empty;
            if (!group_mode)
            {
                if (sel_indices.Length == 0)
                {
                    target_file = Path.Combine(dl.DirectoryPath, dl.GetItemDisplayNameLong(e.FocusedIndex));
                }
                else
                {
                    target_file = Path.Combine(dl.DirectoryPath, dl.GetItemDisplayNameLong(sel_indices[0]));
                }
                if (target_file.EndsWith(".."))
                {
                    target_file = dl.DirectoryPath;
                }
                if (!WinAPiFSwrapper.GetFileInfo(target_file, ref f_data))
                {
                    return;
                }
            }



            var dialog = new FileAttributesEditDialog();
            if (!group_mode)
            {
                dialog.buttonClear.Enabled = false;
                set_attributes_to_dialog(dialog, f_data.dwFileAttributes);
                dialog.Text = String.Format
                    (Options.GetLiteral(Options.LANG_FILE_ATTRIBUTES) + " [{0}]", target_file);
            }
            else
            {
                dialog.buttonClear.Enabled = true;
                dialog.buttonOK.Text = Options.GetLiteral(Options.LANG_ADD);
                dialog.Text = string.Format
                    (Options.GetLiteral(Options.LANG_FILE_ATTRIBUTES) + " [{0} " + Options.GetLiteral(Options.LANG_ENTRIES) + "]", sel_indices.Length);
            }
            set_readonly_attributes(dialog);

            var d_res = dialog.ShowDialog();

            switch (d_res)
            {
                case DialogResult.OK:
                    //group mode = add attributes
                    //single mode = set attributes
                    if (!group_mode)
                    {
                        try
                        {
                            set_attributes_file(target_file, get_attributes_from_dialog(dialog));
                        }
                        catch (Exception ex)
                        {
                            Messages.ShowException(ex);
                        }
                    }
                    else
                    {
                        //group mode
                        var add_fa = get_attributes_from_dialog(dialog);
                        foreach(var one_index in sel_indices)
                        {
                            target_file = Path.Combine(dl.DirectoryPath, dl.GetItemDisplayNameLong(one_index));
                            try
                            {
                                add_attributes_file(target_file, add_fa);
                                OnItemProcessDone(new ItemEventArs(one_index));
                            }
                            catch (Exception ex)
                            {
                                Messages.ShowException(ex);
                            }
                        }
                    }
                    break;

                case DialogResult.Yes:
                    //group mode only = clear attributes
                    var clear_fa = get_attributes_from_dialog(dialog);
                    foreach (var one_index in sel_indices)
                    {
                        target_file = Path.Combine(dl.DirectoryPath, dl.GetItemDisplayNameLong(one_index));
                        try
                        {
                            clear_attributes_file(target_file, clear_fa);
                            OnItemProcessDone(new ItemEventArs(one_index));
                        }
                        catch (Exception ex)
                        {
                            Messages.ShowException(ex);
                        }
                    }
                    break;
            }

        }

        private void clear_attributes_file(string file_name, FileAttributes fa)
        {
            var fsi = IOhelper.GetFileSystemInfo(file_name);
            if (fsi == null)
            {
                
                throw new ApplicationException(string.Format(Options.GetLiteral(Options.LANG_FILE_0_NOT_FOUND),file_name));
            }
            var existing_fa = fsi.Attributes;
            var existing_fa_int = (int)existing_fa;
            int needed_fa_int;
            var fa_int = (int)fa;
            needed_fa_int = ~((~existing_fa_int) | fa_int);
            var needed_fa = (FileAttributes)needed_fa_int;
            fsi.Attributes = needed_fa;
        }

        private void add_attributes_file(string file_name, FileAttributes fa)
        {
            var fsi = IOhelper.GetFileSystemInfo(file_name);
            if (fsi == null)
            {
                throw new ApplicationException(string.Format(Options.GetLiteral(Options.LANG_FILE_0_NOT_FOUND),file_name));
            }
            var existing_fa = fsi.Attributes;
            var needed_fa = fa | existing_fa;
            fsi.Attributes = needed_fa;
        }

        private void set_attributes_file(string file_name, FileAttributes fa)
        {
            var fsi = IOhelper.GetFileSystemInfo(file_name);
            if (fsi == null)
            {
                throw new ApplicationException(string.Format(Options.GetLiteral(Options.LANG_FILE_0_NOT_FOUND), file_name));
            }
            fsi.Attributes = fa;
        }

        private void set_readonly_attributes(FileAttributesEditDialog dial)
        {
            dial.checkBoxCompressed.Enabled = false;
            dial.checkBoxDirectory.Enabled = false;
            dial.checkBoxEncrypted.Enabled = false;
            dial.checkBoxReparsePoint.Enabled = false;
            dial.checkBoxSparseFile.Enabled = false;
        }

        private void set_attributes_to_dialog(FileAttributesEditDialog dial, FileAttributes fa)
        {
            dial.checkBoxArchive.Checked = ((fa & FileAttributes.Archive) == FileAttributes.Archive);
            dial.checkBoxCompressed.Checked = ((fa & FileAttributes.Compressed) == FileAttributes.Compressed);
            dial.checkBoxDirectory.Checked = ((fa & FileAttributes.Directory) == FileAttributes.Directory);
            dial.checkBoxEncrypted.Checked = ((fa & FileAttributes.Encrypted) == FileAttributes.Encrypted);
            dial.checkBoxHidden.Checked = ((fa & FileAttributes.Hidden) == FileAttributes.Hidden);
            dial.checkBoxNormal.Checked = ((fa & FileAttributes.Normal) == FileAttributes.Normal);
            dial.checkBoxNotcontentIndexed.Checked = ((fa & FileAttributes.NotContentIndexed) == FileAttributes.NotContentIndexed);
            dial.checkBoxOffline.Checked = ((fa & FileAttributes.Offline) == FileAttributes.Offline);
            dial.checkBoxReadonly.Checked = ((fa & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
            dial.checkBoxReparsePoint.Checked = ((fa & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint);
            dial.checkBoxSparseFile.Checked = ((fa & FileAttributes.SparseFile) == FileAttributes.SparseFile);
            dial.checkBoxSystem.Checked = ((fa & FileAttributes.System) == FileAttributes.System);
            dial.checkBoxTemporary.Checked = ((fa & FileAttributes.Temporary) == FileAttributes.Temporary);
        }

        private FileAttributes get_attributes_from_dialog(FileAttributesEditDialog dial)
        {
            FileAttributes ret = 0;

            //if (dial.checkBoxArchive.Checked)
            //{
            //    ret = ret | FileAttributes.Archive;
            //}
            ret = dial.checkBoxArchive.Checked ? ret | FileAttributes.Archive : ret;
            ret = dial.checkBoxCompressed.Checked ? ret | FileAttributes.Compressed : ret;
            ret = dial.checkBoxDirectory.Checked ? ret | FileAttributes.Directory : ret;
            ret = dial.checkBoxEncrypted.Checked ? ret | FileAttributes.Encrypted : ret;
            ret = dial.checkBoxHidden.Checked ? ret | FileAttributes.Hidden : ret;
            ret = dial.checkBoxNormal.Checked ? ret | FileAttributes.Normal : ret;
            ret = dial.checkBoxNotcontentIndexed.Checked ? ret | FileAttributes.NotContentIndexed : ret;
            ret = dial.checkBoxOffline.Checked ? ret | FileAttributes.Offline : ret;
            ret = dial.checkBoxReadonly.Checked ? ret | FileAttributes.ReadOnly : ret;
            ret = dial.checkBoxReparsePoint.Checked ? ret | FileAttributes.ReparsePoint : ret;
            ret = dial.checkBoxSparseFile.Checked ? ret | FileAttributes.SparseFile : ret;
            ret = dial.checkBoxSystem.Checked ? ret | FileAttributes.System : ret;
            ret = dial.checkBoxTemporary.Checked ? ret | FileAttributes.Temporary : ret;
            return ret;
        }
    }
}
