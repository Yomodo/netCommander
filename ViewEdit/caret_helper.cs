using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;

namespace netCommander.FileView
{
    class Caret
    {
        #region caret
        [DllImport("user32.dll", SetLastError = true,EntryPoint="CreateCaret")]
        private static extern int CreateCaretApi(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);
        /*
         * Windows allows only one caret per message queue.
         * To add a caret to a control, handle the WM_SETFOCUS message,
         * or the GotFocus event, or override OnGotFocus if you're writing
         * a custom control, and call CreateCaret from the message
         * or event handler. You should also handle WM_KILLFOCUS,
         * LostFocus or OnLostFocus and call DestroyCaret.
         * You will also need to call ShowCaret to make the caret visible,
         * and SetCaretPos to set its position.
         */
        public static void CreateCaretBitmap(IntPtr winHandle, IntPtr hBitmap)
        {
            var res = CreateCaretApi(winHandle, hBitmap, 0, 0);
            if (res == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        public static void CreateCaretSolid(IntPtr winHandle, int width, int height)
        {
            var res = CreateCaretApi(winHandle, IntPtr.Zero, width, height);
            if (res == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        public static void CreateCaretGrey(IntPtr winHandle, int width, int height)
        {
            var res = CreateCaretApi(winHandle, new IntPtr(1), width, height);
            if (res == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "DestroyCaret")]
        private static extern int DestroyCaretApi();
        public static void DestroyCaret()
        {
            var res = DestroyCaretApi();
            if (res == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        [DllImport("user32.dll",SetLastError=true,EntryPoint="GetCaretPos")]
        private static extern int GetCaretPosApi(out Point lpPoint);
        public static Point GetCaretPos()
        {
            var ret = new Point();
            var res = GetCaretPosApi(out ret);
            if (res == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return ret;
        }
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "HideCaret")]
        private static extern int HideCaretApi(IntPtr hWnd);
        public static void HideCaret()
        {
            var res = HideCaretApi(IntPtr.Zero);
            if (res == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        public static void HideCaret(IntPtr winHandle)
        {
            var res = HideCaretApi(winHandle);
            if (res == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetCaretPos")]
        private static extern int SetCaretPosApi(int X, int Y);
        public static void SetCaretPos(int X, int Y)
        {
            var res = SetCaretPosApi(X, Y);
            if (res == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        public static void SetCaretPos(Point p)
        {
            var res = SetCaretPosApi(p.X, p.Y);
            if (res == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "ShowCaret")]
        private static extern int ShowCaretApi(IntPtr hWnd);
        public static void ShowCaret()
        {
            var res = ShowCaretApi(IntPtr.Zero);
            if (res == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        public static void ShowCaret(IntPtr winHandle)
        {
            var res = ShowCaretApi(winHandle);
            if (res == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        #endregion
    }
}
