using Frank.Utils;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Frank
{
    public partial class Frank : Form
    {
        [DllImport("user32.dll")]
        private static extern bool LockWorkStation(); // Pour gérer le
        private readonly AppState _state;             // multiscreen

        private bool allowRun = false; // Autorise le programme a se lancer
        private bool allowClose = false; // Autorise le programme a se fermer
        private int durationLeft = 10;

        private TextStyle lblTitleStyle;
        private TextStyle lblTimerStyle;

        public Frank(AppState state)
        {
            InitializeComponent();
            this._state = state; // Multiscreen

            SystemEvents.SessionSwitch += OnSessionSwitch; // Envoie un espion pour savoir quand le pc se déverouille
            this.Shown += this.FrankShown; // Lock le pc
        }

        private void FrankShown(object? sender, EventArgs e)
        {
            LockWorkStation();
        }

        private void Start()
        {
            if (this.allowRun) return;
            this.allowRun = true;

            // Lance la désactivation du clavier et le début du timer
            Keyboard.DisableKeyboard();
            this.timer.Start();

            // Initialisation des labels
            this.lblTitleStyle = new TextStyle("Vous avez été infecté par le virus Frank", new FontFamily("Segoe UI"), (int)FontStyle.Bold, 48, new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2), Utils.Utils.GetCenterAlignment());
            this.lblTimerStyle = new TextStyle("Vos données serront supprimé dans " + this.durationLeft + " secondes", new FontFamily("Segoe UI"), (int)FontStyle.Regular, 48, new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2 + 100), Utils.Utils.GetCenterAlignment());
            this.btnEmergency.Location = new Point(this.ClientSize.Width - this.btnEmergency.Size.Width, this.ClientSize.Height - this.btnEmergency.Size.Height);
            this.pbBackground.Invalidate(); // Force la pictureBox a se rafraichir
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionUnlock) // Check si la session est unlock
            {
                this.BeginInvoke(new Action(this.Start)); // Si unlock on lance le programme
            }
        }

        private void Frank_FormClosing(object sender, FormClosingEventArgs e)
        {
            Keyboard.EnableKeyboard(); // Réactive les touches

            if (Program.IsClosing) // Gère le multiscreen
            {
                this.allowClose = true;
                e.Cancel = false;
                return;
            }

            if (!this.allowClose) // Autorise la fermeture a la fin du chrono
            {
                e.Cancel = true;
                this.CleanClose();
            }
        }

        private void Frank_FormClosed(object sender, FormClosedEventArgs e)
        {
            SystemEvents.SessionSwitch -= OnSessionSwitch;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!this.allowRun) return;

            // Affiche le compte a rebour
            this.lblTimerStyle.text = "Vos données serront supprimé dans " + (this.durationLeft--) + " secondes";
            this.pbBackground.Invalidate();

            // Quite si le compte a rebour est fini
            // Et oui ce n'est pas un vrai virus il quite juste
            // il ne supprime rien pas comme ce qui est écrit :)
            if (this.durationLeft < 0)
            {
                this.CleanClose();
            }
        }

        // Arrets des timers (pour gèrer les multiscreens
        public void StopTimers()
        {
            this.timer.Stop();
        }

        // Close propre e ngérant les multiscreen
        private void CleanClose()
        {
            Program.CleanCloseAll();
        }

        // Un bouton d'urgence qui permet de quitter l'application en cas de problèmes
        // Il s'agit d'un bouton de 5px/5px en bas a droite de l'écran
        private void btnEmergency_Click(object sender, EventArgs e)
        {
            this.CleanClose();
        }

        // Permet d'afficher les labels personalisés
        private void pbBackground_Paint(object sender, PaintEventArgs e)
        {
            if (!this.allowRun) return;
            Utils.Utils.DrawText(this.lblTitleStyle, e);
            Utils.Utils.DrawText(this.lblTimerStyle, e);
        }
    }
}
