using Microsoft.Win32;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Frank
{
    public partial class Frank : Form
    {
        [DllImport("user32.dll")]
        private static extern bool LockWorkStation();

        private readonly AppState _state;

        private bool allowRun    = false;
        private bool allowClose  = false;
        private int durationLeft = 10;
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
            this._state = state;
            SystemEvents.SessionSwitch += OnSessionSwitch;
            this.Shown += this.FrankShown;
        }

        private void FrankShown(object? sender, EventArgs e)
        {
            LockWorkStation();
        }

        private void Start()
        {
            if (this.allowRun) return;
            this.allowRun = true;

            Keyboard.DisableKeyboard();
            this.timer.Enabled = true;

            this.lblTitleStyle = new TextStyle("Vous avez été infecté par le virus Frank", new FontFamily("Segoe UI"), (int)FontStyle.Bold, 48, new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2), this.GetCenterAlignment());
            this.lblTimerStyle = new TextStyle("Vos données serront supprimé dans " + this.durationLeft + " secondes", new FontFamily("Segoe UI"), (int)FontStyle.Regular, 48, new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2 + 100), this.GetCenterAlignment());
            this.btnEmergency.Location = new Point(this.ClientSize.Width - this.btnEmergency.Size.Width, this.ClientSize.Height - this.btnEmergency.Size.Height);
            this.pbBackground.Invalidate();
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                this.BeginInvoke(new Action(this.Start));
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SystemEvents.SessionSwitch -= OnSessionSwitch;
            base.OnFormClosed(e);
        }

        private void Frank_FormClosing(object sender, FormClosingEventArgs e)
        {
            Keyboard.EnableKeyboard();

            if (Program.IsClosing)
            {
                this.allowClose = true;
                e.Cancel = false;
                return;
            }

            if (!this.allowClose)
            {
                e.Cancel = true;
                this.CleanClose();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!this.allowRun) return;
            this.lblTimerStyle.text = "Vos données serront supprimé dans " + (this.durationLeft--) + " secondes";
            this.pbBackground.Invalidate();
            if (this.durationLeft < 0)
            {
                this.CleanClose();
            }
        }
        public void StopTimers()
        {
            this.timer.Stop();
        }

        private void CleanClose()
        {
            Program.CleanCloseAll();
        }

        private void btnEmergency_Click(object sender, EventArgs e)
        {
            this.CleanClose();
        }

        private void pbBackground_Paint(object sender, PaintEventArgs e)
        {
            if (!this.allowRun) return;
            DrawText(this.lblTitleStyle, e);
            DrawText(this.lblTimerStyle, e);
        }

        private void DrawText(TextStyle message, PaintEventArgs e)
        {
            if (!this.allowRun) return;
            using GraphicsPath path = new GraphicsPath();

            path.AddString(message.text, message.fontFamily, message.fontStyle, message.fontSize, message.position, message.stringFormat);

            using Pen outline = new Pen(Color.Black, 4);
            using Brush fill = new SolidBrush(Color.White);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawPath(outline, path);
            e.Graphics.FillPath(fill, path);
        }
    }
}
