using System.Runtime.InteropServices;

public static class Keyboard
{
    private const int WH_KEYBOARD_LL = 13;

    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;

    private const int VK_MENU = 0x12;
    private const int VK_TAB = 0x09;

    private static IntPtr hookHandle = IntPtr.Zero;
    private static LowLevelKeyboardProc hookCallback = HookCallback;

    private delegate IntPtr LowLevelKeyboardProc(
        int nCode,
        IntPtr wParam,
        IntPtr lParam
    );

    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public IntPtr dwExtraInfo;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string? lpModuleName);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    private static System.Threading.Timer? watchdog;

    public static void DisableKeyboard()
    {
        if (hookHandle != IntPtr.Zero) return;
        InstallHook();

        watchdog = new System.Threading.Timer(_ => { 
            if (hookHandle == IntPtr.Zero) InstallHook();
        }, null, 500, 500);
    }

    public static void EnableKeyboard()
    {
        watchdog?.Dispose();
        watchdog = null;

        if (hookHandle == IntPtr.Zero) return;
        UnhookWindowsHookEx(hookHandle);
        hookHandle = IntPtr.Zero;
    }

    public static void InstallHook()
    {
        hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, hookCallback, GetModuleHandle(null), 0);
    }

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            KBDLLHOOKSTRUCT keyInfo =
                Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

            bool altDown = (GetAsyncKeyState(VK_MENU) & 0x8000) != 0;

            // Bloque Alt + Tab
            if (keyInfo.vkCode == VK_TAB && altDown) return (IntPtr)1;

            // Bloque toutes les touches normales
            if (wParam == (IntPtr)WM_KEYDOWN ||
                wParam == (IntPtr)WM_KEYUP ||
                wParam == (IntPtr)WM_SYSKEYDOWN ||
                wParam == (IntPtr)WM_SYSKEYUP) return (IntPtr)1;
        }

        return CallNextHookEx(hookHandle, nCode, wParam, lParam);
    }
}
