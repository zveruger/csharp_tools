using System;
using System.Drawing;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tools.CSharp.Controls
{
    /// <summary>
    /// http://stackoverflow.com/questions/1732443/center-messagebox-in-parent-form
    /// </summary>
    public sealed class DialogCenteringService : IDisposable
    {
        #region private
        private const int _WhCallwndprocret = 12;
        private const int _HookActionActivate = 5;
        //---------------------------------------------------------------------
        private const SetWindowPosFlags _WindowPosFlags = SetWindowPosFlags.SWP_ASYNCWINDOWPOS
            | SetWindowPosFlags.SWP_NOSIZE
            | SetWindowPosFlags.SWP_NOACTIVATE
            | SetWindowPosFlags.SWP_NOOWNERZORDER
            | SetWindowPosFlags.SWP_NOZORDER;
        //---------------------------------------------------------------------
        private readonly IWin32Window _owner;
        private readonly IntPtr _hHook;
        //---------------------------------------------------------------------
        [Flags]
        private enum SetWindowPosFlags : uint
        {

            /// <summary>
            /// If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000,

            /// <summary>
            /// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOACTIVATE = 0x0010,

            /// <summary>
            /// Does not change the owner window's position in the Z order.
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200,

            /// <summary>
            /// Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            SWP_NOSIZE = 0x0001,

            /// <summary>
            /// Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOZORDER = 0x0004
        }
        //---------------------------------------------------------------------
        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct CWPRETSTRUCT
        {
            public IntPtr lResult;
            public IntPtr lParam;
            public IntPtr wParam;
            public uint message;
            public IntPtr hwnd;
        }
        //---------------------------------------------------------------------
        private IntPtr _DialogHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            { return CallNextHookEx(_hHook, nCode, wParam, lParam); }

            var msg = (CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPRETSTRUCT));
            var hook = _hHook;

            if (msg.message == _HookActionActivate)
            {
                try
                {
                    _CenterWindow(msg.hwnd);
                }
                finally
                {
                    UnhookWindowsHookEx(_hHook);
                }
            }

            return CallNextHookEx(hook, nCode, wParam, lParam);
        }
        //---------------------------------------------------------------------
        private void _CenterWindow(IntPtr hChildWnd)
        {
            var recParent = _GetWindowRect(_owner.Handle);

            if (recParent == null)
            {
                return;
            }

            _CenterWindow(hChildWnd, recParent.Value);
        }
        private static void _CenterWindow(IntPtr hChildWnd, Rectangle recParent)
        {
            var recChild = _GetWindowRect(hChildWnd);

            if (recChild == null)
            { return; }

            var centerRectangle = _GetCenterRectangle(recParent, recChild.Value);

            Task.Factory.StartNew(() => SetWindowPos(
                hChildWnd,
                (IntPtr)0,
                centerRectangle.X,
                centerRectangle.Y,
                centerRectangle.Width,
                centerRectangle.Height,
                _WindowPosFlags
            ));
        }
        //---------------------------------------------------------------------
        private static Rectangle? _GetWindowRect(IntPtr hWnd)
        {
            var rect = new Rectangle(0, 0, 0, 0);
            var success = GetWindowRect(hWnd, ref rect);

            if (!success)
            {
                return null;
            }

            return rect;
        }
        private static Rectangle _GetCenterRectangle(Rectangle recParent, Rectangle recChild)
        {
            var width = recChild.Width - recChild.X;
            var height = recChild.Height - recChild.Y;

            var ptCenter = new Point(
                recParent.X + ((recParent.Width - recParent.X) / 2),
                recParent.Y + ((recParent.Height - recParent.Y) / 2)
            );

            var ptStart = new Point(
                (ptCenter.X - (width / 2)),
                (ptCenter.Y - (height / 2))
            );

            var centeredRectangle = new Rectangle(ptStart.X, ptStart.Y, width, height);

            var parentScreen = Screen.FromRectangle(recParent);
            var workingArea = parentScreen.WorkingArea;

            if (workingArea.X > centeredRectangle.X)
            { centeredRectangle = new Rectangle(workingArea.X, centeredRectangle.Y, centeredRectangle.Width, centeredRectangle.Height); }

            if (workingArea.Y > centeredRectangle.Y)
            { centeredRectangle = new Rectangle(centeredRectangle.X, workingArea.Y, centeredRectangle.Width, centeredRectangle.Height); }

            if (workingArea.Right < centeredRectangle.Right)
            { centeredRectangle = new Rectangle(workingArea.Right - centeredRectangle.Width, centeredRectangle.Y, centeredRectangle.Width, centeredRectangle.Height); }

            if (workingArea.Bottom < centeredRectangle.Bottom)
            { centeredRectangle = new Rectangle(centeredRectangle.X, workingArea.Bottom - centeredRectangle.Height, centeredRectangle.Width, centeredRectangle.Height); }

            return centeredRectangle;
        }
        #endregion
        public DialogCenteringService(IWin32Window owner)
        {
            if (owner == null)
            { throw new ArgumentNullException(nameof(owner)); }

            _owner = owner;
            _hHook = SetWindowsHookEx(_WhCallwndprocret, _DialogHookProc, IntPtr.Zero, GetCurrentThreadId());
        }

        //---------------------------------------------------------------------
        public void Dispose()
        {
            UnhookWindowsHookEx(_hHook);
        }
        //---------------------------------------------------------------------
    }
}