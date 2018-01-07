using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace netCommander
{
    public class PanelCommandDeleteZipEntry:PanelCommandBase
    {
        public PanelCommandDeleteZipEntry()
            : base(Options.GetLiteral(Options.LANG_DELETE), Shortcut.F8)
        {

        }

        protected override void internal_command_proc()
        {
            var e = new QueryPanelInfoEventArgs();
            OnQueryCurrentPanel(e);

            if (!(e.ItemCollection is ZipDirectory))
            {
                return;
            }

            var zd = (ZipDirectory)e.ItemCollection;
            var initial_sources = new List<string>();
            var initial_zip_dir = zd.CurrentZipDirectory;
            if (initial_zip_dir.StartsWith("/"))
            {
                initial_zip_dir = initial_zip_dir.Substring(1);
            }
            if ((!initial_zip_dir.EndsWith("/")) && (initial_zip_dir != string.Empty))
            {
                initial_zip_dir = initial_zip_dir + "/";
            }
            if (e.SelectedIndices.Length == 0)
            {
                if (zd.GetItemDisplayName(e.FocusedIndex) == "..")
                {
                    return;
                }
                initial_sources.Add(initial_zip_dir + zd.GetItemDisplayName(e.FocusedIndex));
            }
            else
            {
                for (var i = 0; i < e.SelectedIndices.Length; i++)
                {
                    initial_sources.Add(initial_zip_dir + zd.GetItemDisplayName(e.SelectedIndices[i]));
                }
            }

        //show dialog
            var dialog = new DeleteFileDialog();
            dialog.Text = Options.GetLiteral(Options.LANG_DELETE);
            dialog.DeleteFileOptions = DeleteFileOptions.DeleteEmptyDirectories | DeleteFileOptions.DeleteReadonly | DeleteFileOptions.RecursiveDeleteFiles;
            dialog.textBoxMask.Text = "*";
            dialog.checkBoxForceReadonly.Enabled = false;
            dialog.checkBoxRecursive.Enabled = false;
            dialog.checkBoxRemoveEmptyDirs.Enabled = false;
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var del_list = new List<ZipEntry>();
            create_delete_list(del_list, initial_sources, dialog.textBoxMask.Text, zd.ZipFile);

            var main_win = (mainForm)Program.MainWindow;

            try
            {
                zd.ZipFile.BeginUpdate();
                for (var i = 0; i < del_list.Count; i++)
                {
                    main_win.NotifyLongOperation
                        (string.Format
                        (Options.GetLiteral(Options.LANG_DELETE_NOW_0),
                        del_list[i].Name),
                        true);
                    zd.ZipFile.Delete(del_list[i]);
                    
                }
                main_win.NotifyLongOperation(Options.GetLiteral(Options.LANG_COMMIT_ARCHIVE_UPDATES), true);
                zd.ZipFile.CommitUpdate();

                zd.Refill();
            }
            catch (Exception ex)
            {
                Messages.ShowException(ex);
            }
            finally
            {
                main_win.NotifyLongOperation("Done", false);
            }
        }

        private void create_delete_list(List<ZipEntry> list, List<string> root_srcs, string mask, ZipFile zf)
        {
            foreach (ZipEntry one_entry in zf)
            {
                if (!Wildcard.Match(mask, one_entry.Name))
                {
                    continue;
                }

                foreach (var root_src in root_srcs)
                {
                    if (one_entry.Name.StartsWith(root_src))
                    {
                        if (!list.Contains(one_entry))
                        {
                            list.Add(one_entry);
                            break;
                        }
                    }
                }
            }
        }


    }
}
