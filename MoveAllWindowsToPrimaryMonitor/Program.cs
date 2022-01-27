using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MoveAllWindowsToPrimaryMonitor
{
	class Program
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		static void Main(string[] args)
		{
			// primary screen data
			Rectangle mainMonitor = Screen.PrimaryScreen.WorkingArea;
			Point dest = new Point(mainMonitor.Left + 10, mainMonitor.Top + 10);

			// loop over all processes
			Process[] processList = Process.GetProcesses();
			foreach (Process process in processList)
			{
				// check for main window
				if (process.MainWindowHandle == IntPtr.Zero)
					continue;

				// check for position
				GetWindowRect(process.MainWindowHandle, out RECT window);
				if (!mainMonitor.Contains(new Point(window.Top, window.Left)))
				{
					// move to main monitor
					int width = window.Right - window.Left;
					int height = window.Bottom - window.Top;
					MoveWindow(process.MainWindowHandle, dest.X, dest.Y, width, height, true);

					Console.WriteLine($"Moving {process.MainWindowTitle}");
				}
			}
		}
	}
}
