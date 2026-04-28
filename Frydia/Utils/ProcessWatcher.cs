namespace Frydia.Utils
{
    public class ProcessWatcher
    {
        public string ProcessName { get; }
        public Action<IntPtr, Rectangle> OnWindowFound { get; }
        public Action<IntPtr> OnWindowLost { get; }

        public ProcessWatcher(string processName, Action<IntPtr, Rectangle> onFound, Action<IntPtr> onLost)
        {
            ProcessName = processName;
            OnWindowFound = onFound;
            OnWindowLost = onLost;
        }
    }
}
