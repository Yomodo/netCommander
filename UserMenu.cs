using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace netCommander
{
    public class UserMenu
    {
        public event QueryPanelInfoEventHandler QueryCurrentPanel;
        public event QueryPanelInfoEventHandler QueryOtherPanel;

        //first menu is 'Edit menu'
        //second menu is divider

        public UserMenu(MenuItem parent_menu)
        {
            ParentMenuItem = parent_menu;
            var edit_menu = ParentMenuItem.MenuItems.Add(Options.GetLiteral(Options.LANG_EDIT));
            edit_menu.Click += new EventHandler(edit_menu_Click);
            ParentMenuItem.MenuItems.Add("-");

            Options.ReadUserMenu(this);
        }

        void edit_menu_Click(object sender, EventArgs e)
        {
            var dialog = new UserMenuEditDialog();
            dialog.SetMenu(this);

            dialog.ShowDialog();
            Options.WriteUserMenu(this);
        }

        private void OnQueryCurrentPanel(QueryPanelInfoEventArgs e)
        {
            if (QueryCurrentPanel != null)
            {
                QueryCurrentPanel(this,e);
            }
        }

        private void OnQueryOtherPanel(QueryPanelInfoEventArgs e)
        {
            if (QueryOtherPanel != null)
            {
                QueryOtherPanel(this,e);
            }
        }

        public UserMenuEntry this[int index]
        {
            get
            {
                return (UserMenuEntry)ParentMenuItem.MenuItems[index + 2];
            }
        }

        public MenuItem  ParentMenuItem
        {
            get;
            private set;
        }

        public void Clear()
        {
            for (var i = 2; i < ParentMenuItem.MenuItems.Count; i++)
            {
                ParentMenuItem.MenuItems.RemoveAt(i);
            }
        }

        public void Add(UserMenuEntry entry)
        {
            ParentMenuItem.MenuItems.Add(entry);
            entry.Click += new EventHandler(MenuItem_Click);
        }

        public void RemoveAt(int index)
        {
            ParentMenuItem.MenuItems.RemoveAt(index + 2);
        }

        public int Count
        {
            get
            {
                return ParentMenuItem.MenuItems.Count - 2;
            }
        }

        void MenuItem_Click(object sender, EventArgs e)
        {
            execute((UserMenuEntry)sender);
        }

        private void execute(UserMenuEntry entry)
        {
            var command_expanded = entry.CommandText;

            try
            {
                var e_current = new QueryPanelInfoEventArgs();
                var e_other = new QueryPanelInfoEventArgs();
                OnQueryCurrentPanel(e_current);
                OnQueryOtherPanel(e_other);
                var repl_text = string.Empty;

                //process %1
                if (command_expanded.Contains("%1"))
                {
                    repl_text = e_current.ItemCollection.GetCommandlineTextShort(e_current.FocusedIndex);
                    if (repl_text.Contains(" "))
                    {
                        repl_text = '"' + repl_text + '"';
                    }
                    command_expanded = command_expanded.Replace("%1", repl_text);
                }

                //process %2
                if (command_expanded.Contains("%2"))
                {
                    repl_text = e_current.ItemCollection.GetCommandlineTextLong(e_current.FocusedIndex);
                    if (repl_text.Contains(" "))
                    {
                        repl_text = '"' + repl_text + '"';
                    }
                   command_expanded= command_expanded.Replace("%2", repl_text);
                }

                //process %3
                if (command_expanded.Contains("%3"))
                {
                    var dl = (DirectoryList)e_other.ItemCollection;
                    repl_text = dl.DirectoryPath;
                    if (repl_text.Contains(" "))
                    {
                        repl_text = '"' + repl_text + '"';
                    }
                    command_expanded = command_expanded.Replace("%3", repl_text);
                }

                var reg_pattern = Options.REGEX_PARSE_COMMAND;
                var rex = new Regex(reg_pattern);

                var splitted = rex.Split(command_expanded, 2);
                if (splitted.Length < 2)
                {
                    throw new ApplicationException(Options.GetLiteral(Options.LANG_CANNOT_PARSE_COMMAND_LINE));
                }

                var command_name = splitted[1];
                var command_args = splitted.Length > 2 ? splitted[2] : string.Empty;

                var psi = new ProcessStartInfo();
                psi.Arguments = command_args;
                psi.CreateNoWindow = (entry.Options & ProcessStartFlags.NoWindow) == ProcessStartFlags.NoWindow;
                psi.ErrorDialog = false;
                psi.FileName = command_name;
                psi.UseShellExecute = (entry.Options & ProcessStartFlags.UseShellexecute) == ProcessStartFlags.UseShellexecute;
                psi.WorkingDirectory = Directory.GetCurrentDirectory();

                var p = new Process();
                p.StartInfo = psi;
                p.Start();

            }
            catch (Exception ex)
            {
                Messages.ShowException
                    (ex,
                    string.Format
                    (Options.GetLiteral(Options.LANG_CANNOT_EXECUTE_0),
                    command_expanded));
            }
        }
    }

    public class UserMenuEntry : MenuItem
    {
        public UserMenuEntry(string command_text, string menu_text)
            : base(menu_text)
        {
            CommandText = command_text;
        }

        public string CommandText
        {
            get;
            set;
        }

        public ProcessStartFlags Options
        {
            get;
            set;
        }
    }

    [Flags()]
    public enum ProcessStartFlags
    {
        None = 0,
        NoWindow = 0x1,
        UseShellexecute = 0x2
    }
}
