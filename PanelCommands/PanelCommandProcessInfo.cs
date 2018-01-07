using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace netCommander
{
    class PanelCommandProcessInfo : PanelCommandBase
    {

        public PanelCommandProcessInfo()
            : base(Options.GetLiteral(Options.LANG_PROPERTIES), System.Windows.Forms.Shortcut.F11)
        {

        }

        protected override void internal_command_proc()
        {
            try
            {
                var e_current = new QueryPanelInfoEventArgs();
                OnQueryCurrentPanel(e_current);

                var pl = (ProcessList)e_current.ItemCollection;
                var p = pl[e_current.FocusedIndex];

                var dialog = new ProcessInfoDialog();
                //p.Refresh();
                dialog.Fill(p);
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                Messages.ShowException(ex);
            }
        }
    }
}
