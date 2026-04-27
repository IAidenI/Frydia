using System.Runtime.InteropServices;

public static class Keyboard
{
    // Type de hook pour intercepter globalement toutes les touches du clavier
    private const int WH_KEYBOARD_LL = 13;

    // Appuis et relachement de touches
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;

    // Pour le alt tab
    private const int VK_MENU = 0x12;
    private const int VK_TAB = 0x09;

    private static IntPtr hookHandle = IntPtr.Zero;
    private static LowLevelKeyboardProc hookCallback = HookCallback;

    private delegate IntPtr LowLevelKeyboardProc(
        int nCode,
        IntPtr wParam,
        IntPtr lParam
    );

    // Structure native de windows avec toutes les informations sur la touche pressé
    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public IntPtr dwExtraInfo;
    }

    // Fonctions Win32 utilisées pour installé, retirer et chaîner le hook du clavier
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

    // Watchdog pour réinstaller le hook si il disparait
    private static System.Threading.Timer? watchdog;

    public static void DisableKeyboard()
    {
        // Pour ne pas installer plusieurs fois le même hook
        if (hookHandle != IntPtr.Zero) return;
        InstallHook();

        // Vérifie régulièrement si me hook est toujours actif
        watchdog = new System.Threading.Timer(_ => { 
            if (hookHandle == IntPtr.Zero) InstallHook();
        }, null, 500, 500);
    }

    public static void EnableKeyboard()
    {
        // Arrêt de la surveillance
        watchdog?.Dispose();
        watchdog = null;

        if (hookHandle == IntPtr.Zero) return;
        UnhookWindowsHookEx(hookHandle); // Réstauration des touches
        hookHandle = IntPtr.Zero;
    }

    public static void InstallHook()
    {
        // Installe un hook clavier
        hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, hookCallback, GetModuleHandle(null), 0);
    }

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0) // Indique que l'événement peut être traité
        {
            KBDLLHOOKSTRUCT keyInfo = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

            // Si alt est maintenu
            bool altDown = (GetAsyncKeyState(VK_MENU) & 0x8000) != 0;

            // Bloque Alt + Tab
            if (keyInfo.vkCode == VK_TAB && altDown) return (IntPtr)1;

            // Bloque appuie et relachement des touches classiques
            if (wParam == (IntPtr)WM_KEYDOWN ||
                wParam == (IntPtr)WM_KEYUP ||
                wParam == (IntPtr)WM_SYSKEYDOWN ||
                wParam == (IntPtr)WM_SYSKEYUP) return (IntPtr)1;
        }

        return CallNextHookEx(hookHandle, nCode, wParam, lParam);
    }
}
