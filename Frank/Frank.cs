using Microsoft.Win32;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Frank
{
    public partial class Frank : Form
    {
        [DllImport("user32.dll")]
        private static extern bool LockWorkStation(); // Pour gérer le
        private readonly AppState _state;             // multiscreen

        private bool allowRun    = false; // Autorise le programme a se lancer
        private bool allowClose  = false; // Autorise le programme a se fermer
        private int durationLeft = 10;

        // Pour afficher un texte personalisé dans une picturebox
        private struct TextStyle
        {
            public string text;
            public FontFamily fontFamily;
            public int fontStyle;
            public float fontSize;
            public Point position;
            public StringFormat stringFormat;

            public TextStyle(string text, FontFamily fontFamily, int fontStyle, float fontSize, Point position, StringFormat stringFormat)
            {
                this.text         = text;
                this.fontFamily   = fontFamily;
                this.fontStyle    = fontStyle;
                this.fontSize     = fontSize;
                this.position     = position;
                this.stringFormat = stringFormat;
            }
        }

        private StringFormat GetCenterAlignment()
        {
            return new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        }

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
            this.lblTitleStyle = new TextStyle("Vous avez été infecté par le virus Frank", new FontFamily("Segoe UI"), (int)FontStyle.Bold, 48, new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2), this.GetCenterAlignment());
            this.lblTimerStyle = new TextStyle("Vos données serront supprimé dans " + this.durationLeft + " secondes", new FontFamily("Segoe UI"), (int)FontStyle.Regular, 48, new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2 + 100), this.GetCenterAlignment());
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
            DrawText(this.lblTitleStyle, e);
            DrawText(this.lblTimerStyle, e);
        }

        private void DrawText(TextStyle message, PaintEventArgs e)
        {
            if (!this.allowRun) return;

            // Ajoute le texte voulu
            using GraphicsPath path = new GraphicsPath();
            path.AddString(message.text, message.fontFamily, message.fontStyle, message.fontSize, message.position, message.stringFormat);

            // Crée un contour et un remplissage
            using Pen outline = new Pen(Color.Black, 4);
            using Brush fill = new SolidBrush(Color.White);

            // Dessine le texte avec un contour d'une autre couleur
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawPath(outline, path);
            e.Graphics.FillPath(fill, path);
        }
    }
}
