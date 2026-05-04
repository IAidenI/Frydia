using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

class InterceptKeys
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;
	private static Process _running = null;
	private static readonly object _lock = new object();

    public static void Main()
    {
        _hookID = SetHook(_proc);
        Application.Run();
        UnhookWindowsHookEx(_hookID);
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule) return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Keys key = (Keys)vkCode;

            bool ctrl = IsKeyDown(Keys.LControlKey) || IsKeyDown(Keys.RControlKey);
            bool alt = IsKeyDown(Keys.LMenu) || IsKeyDown(Keys.RMenu);
            bool shift = IsKeyDown(Keys.LShiftKey) || IsKeyDown(Keys.RShiftKey);

            if (key == Keys.F12 && ctrl && alt && shift)
            {
				Console.WriteLine("Combinaison secret");
				lock(_lock)
				{
					if (_running == null || _running.HasExited)
					{
						string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Frydia.exe");
						if (System.IO.File.Exists(path))
						{
							_running = new Process();
							_running.StartInfo.FileName = "Frydia.exe";
							_running.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
							Console.WriteLine(_running.StartInfo.FileName);
							Console.WriteLine(_running.StartInfo.WorkingDirectory);
							_running.EnableRaisingEvents = true;
							_running.Exited += (s, e) =>
							{
								lock(_lock)
								{
									_running.Dispose();
									_running = null;
								}
							};
							_running.Start();
						}
						else
						{
							DialogResult result = MessageBox.Show("Le fichier " + path + " n'existe pas.\nVoulez-vous quitter l'application ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
							if (result == DialogResult.Yes) Application.Exit();
						}
					}
				}
            }
			Console.WriteLine(key);
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private static bool IsKeyDown(Keys key)
    {
        return (GetAsyncKeyState(key) & 0x8000) != 0;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool UnhookWindowsHookEx(IntPtr hhk);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr GetModuleHandle(string lpModuleName);
    [DllImport("user32.dll")] private static extern short GetAsyncKeyState(Keys vKey);
}