using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace Laan.AddIns.Ssms.VsExtension.Utils
{
    public class Window
    {
        #region DLL Imports

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr child, IntPtr parent);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr handle, ref WindowLocation location);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        private delegate int EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        internal struct Rectangle
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowLocation
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        #endregion

        private Window[] FindChildWindows(Func<Window, bool> filter)
        {
            List<Window> windows = new List<Window>();
            EnumChildWindows(
                Handle,
                (hwnd, lParam) =>
                {
                    Window window = new Window(hwnd);
                    if (filter(window))
                        windows.Add(window);
                    return 1;
                },
                new IntPtr(0)
            );
            return windows.ToArray();
        }

        private string GetClassName(IntPtr handle)
        {
            int length = 64;
            while (true)
            {
                StringBuilder sb = new StringBuilder(length);

                var result = GetClassName(handle, sb, sb.Capacity);
                if (result == 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                if (sb.Length != length - 1)
                    return sb.ToString();

                length *= 2;
            }
        }

        private Rectangle GetLocation(IntPtr handle)
        {
            var location = new WindowLocation();
            location.length = Marshal.SizeOf(location);
            GetWindowPlacement(handle, ref location);
            return location.rcNormalPosition;
        }

        private void Output(Window window, int indent)
        {
            int index = 0;

            foreach (var win in window.FindAllChildWindows())
            {
                Debug.WriteLine(
                    String.Format(
                        "{0}[{1}] - {2}",
                        new string(' ', indent * 4),
                        index++,
                        win.ClassName
                    )
                );

                Output(win, indent + 1);
            }
        }

        public Window(IntPtr handle)
        {
            Handle = handle;
        }

        public void DumpWindows()
        {
            Output(this, 0);
        }

        public void SetFocus()
        {
            SetFocus(Handle);
        }

        public void SetParent(IntPtr child)
        {
            SetParent(child, Handle);
        }

        public IEnumerable<Window> FindAllChildWindows()
        {
            return FindChildWindows(w => true);
        }

        public string ClassName { get { return GetClassName(Handle); } }
        public IntPtr Handle { get; private set; }
    }
}
