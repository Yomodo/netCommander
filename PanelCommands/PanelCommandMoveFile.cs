﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace netCommander
{
    public class PanelCommandMoveFile : PanelCommandBase
    {
        public PanelCommandMoveFile()
            : base(Options.GetLiteral(Options.LANG_MOVE_RENAME), Shortcut.F6)
        {

        }

        private CopyFileProgressDialog dialog_progress;

        protected override void internal_command_proc()
        {
            var e_current = new QueryPanelInfoEventArgs();
            var e_other = new QueryPanelInfoEventArgs();
            OnQueryCurrentPanel(e_current);
            OnQueryOtherPanel(e_other);

            if (e_current.FocusedIndex == -1)
            {
                return;
            }

            if (!(e_other.ItemCollection is DirectoryList))
            {
                Messages.ShowMessage(Options.GetLiteral(Options.LANG_WRONG_DESTINATION));
                return;
            }

            //see source
            var dl_source = (DirectoryList)e_current.ItemCollection;
            var dl_target = (DirectoryList)e_other.ItemCollection;
            var source_list = new List<FileInfoEx>();
            var sel_indices = e_current.SelectedIndices;
            if (sel_indices.Length == 0)
            {
                if (dl_source.GetItemDisplayNameLong(e_current.FocusedIndex) == "..")
                {
                    return;
                }

                source_list.Add(dl_source[e_current.FocusedIndex]);
            }
            else
            {
                for (var i = 0; i < sel_indices.Length; i++)
                {
                    source_list.Add(dl_source[sel_indices[i]]);
                }
            }

            //prepare move dialog
            var dialog = new MoveFileDialog();
            dialog.MoveEngineOptions = Options.MoveEngineOptions;
            dialog.Text = Options.GetLiteral(Options.LANG_MOVE_RENAME);
            dialog.textBoxMask.Text = "*";
            dialog.textBoxDestination.Text = string.Empty;

            //and show
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var move_opts = dialog.MoveEngineOptions;
            Options.MoveEngineOptions = move_opts;

            //prepare progress dialog
            dialog_progress = new CopyFileProgressDialog();
            dialog_progress.labelError.Visible = false;
            dialog_progress.labelSpeed.Text = string.Empty;
            dialog_progress.labelStatus.Text = string.Empty;
            dialog_progress.labelStatusTotal.Text = string.Empty;
            dialog_progress.checkBoxCloseOnFinish.Checked = Options.CopyCloseProgress;

            //calc location - it is not modal!
            dialog_progress.TopLevel = true;
            var x_center = Program.MainWindow.Left + Program.MainWindow.Width / 2;
            var y_center = Program.MainWindow.Top + Program.MainWindow.Height / 2;
            var x_dialog = x_center - dialog_progress.Width / 2;
            var y_dialog = y_center - dialog_progress.Height / 2;
            if (x_dialog < 0)
            {
                x_dialog = 0;
            }
            if (x_dialog < 0)
            {
                y_dialog = 0;
            }

            //show progress
            dialog_progress.Show();
            dialog_progress.Location = new System.Drawing.Point(x_dialog, y_dialog);

            //prepare move engine
            var move_engine = new MoveFileEngine
            (source_list,
            dialog.textBoxDestination.Text == string.Empty ? dl_target.DirectoryPath : Path.Combine(dl_target.DirectoryPath, dialog.textBoxDestination.Text),
            dialog.textBoxMask.Text,
            move_opts,
            dialog_progress);
            move_engine.Done += new EventHandler(move_engine_Done);

            //and run
            move_engine.Do();

        }

        void move_engine_Done(object sender, EventArgs e)
        {
            //call may be from back tread!
            var engine = (MoveFileEngine)sender;
            engine.Dispose();

            if (dialog_progress.InvokeRequired)
            {
                dialog_progress.Invoke(new MethodInvoker(on_copy_done));
            }
            else
            {
                on_copy_done();
            }
        }

        private void on_copy_done()
        {
            Options.CopyCloseProgress = dialog_progress.checkBoxCloseOnFinish.Checked;
            if (dialog_progress.checkBoxCloseOnFinish.Checked)
            {
                dialog_progress.Close();
            }
        }
    }
}
