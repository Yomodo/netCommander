using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace netCommander
{
    class PanelCommandFileInfo : PanelCommandBase
    {

        public PanelCommandFileInfo()
            : base(Options.GetLiteral(Options.LANG_PROPERTIES), Shortcut.F11)
        {

        }

        protected override void internal_command_proc()
        {
            var e = new QueryPanelInfoEventArgs();
            OnQueryCurrentPanel(e);



            var dl = (DirectoryList)e.ItemCollection;

            if (dl.GetItemDisplayNameLong(e.FocusedIndex) == "..")
            {
                return;
            }

            var file_name = Path.Combine(dl.DirectoryPath, dl.GetItemDisplayNameLong(e.FocusedIndex));

            var dialog = new FileInformationDialog();

            try
            {
                if (dl.MainWindow != null)
                {
                    dl.MainWindow.NotifyLongOperation(Options.GetLiteral(Options.LANG_QUERY_PROPERTIES), true);
                }

                dialog.FillContents(file_name);
            }
            finally
            {
                if (dl.MainWindow != null)
                {
                    dl.MainWindow.NotifyLongOperation(string.Empty, false);
                }
            }
            dialog.ShowDialog();
        }
    }
}
