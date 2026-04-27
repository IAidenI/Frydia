using System.Runtime.CompilerServices;

namespace Frank
{
    internal static class Program
    {
        public static List<Frank> OpenForms = new();
        private static bool _isClosing = false;
        public static bool IsClosing => _isClosing;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var state = new AppState();

            foreach (var screen in Screen.AllScreens)
            {
                System.Diagnostics.Debug.WriteLine(screen);
                var form = new Frank(state);

                form.StartPosition = FormStartPosition.Manual;
                form.Location = screen.WorkingArea.Location;
                form.Size = screen.WorkingArea.Size;
                OpenForms.Add(form);
                form.Show();
            }
            Application.Run();
        }

        public static void CleanCloseAll()
        {
            if (_isClosing) return;
            _isClosing = true;

            foreach(var form in OpenForms)
            {
                form.Hide();
                form.StopTimers();
            }

            var primaryForm = OpenForms.FirstOrDefault(f => Screen.FromControl(f).Primary);

            MessageBox.Show(primaryForm, "T'as de la chance cette fois ci, c'était pas un vrai virus...\n\nLa prochaine fois pense a verrouiller ton pc :)", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            Application.Exit();
        }
    }
}