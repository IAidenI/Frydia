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
            Application.SetHighDpiMode(HighDpiMode.PerMonitor);
            ApplicationConfiguration.Initialize();
            //Application.Run(new Lydia());

            // Gère le multiscreen
            var state = new AppState();
            foreach (var screen in Screen.AllScreens)
            {
                // Sur chaques fenêtre on crée une nouvelle instance de Frank
                var form = new Frank(state);
                form.StartPosition = FormStartPosition.Manual;
                form.Location = screen.WorkingArea.Location;
                form.Size = screen.WorkingArea.Size;
                OpenForms.Add(form); // Ajoute un historique des fenêtres ouvertes
                form.Show();
            }
            Application.Run(); // lance l'application
        }

        public static void CleanCloseAll()
        {
            if (_isClosing) return;
            _isClosing = true;

            Keyboard.EnableKeyboard();

            foreach (var form in OpenForms.ToList())
            {
                // Cache la fenêtre parent et stop les timers
                form.Hide();
                form.StopTimers();
            }

            // Cherche parmis l'historique, la fenêtre définit comme étant principale par le système
            var primaryForm = OpenForms.FirstOrDefault(f => Screen.FromControl(f).Primary);

            // Affiche un petit message
            MessageBox.Show(primaryForm, "T'as de la chance cette fois ci, c'était pas un vrai virus...\n\nLa prochaine fois pense a verrouiller ton pc :)", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Donne la main a Lydia
            // Doit être fait avant de fermer Frank
            // car sinon l'application risque de se fermer
            var lydia = new Lydia();
            lydia.Show();

            foreach (var form in OpenForms.ToList())
            {
                form.Close(); // Ferme Frank sur tout les écrans
            }

            _isClosing = false;
        }
    }
}