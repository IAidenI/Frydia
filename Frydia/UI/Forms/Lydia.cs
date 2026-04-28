using Frank.Core;
using Frydia.Utils;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Frank
{
    public partial class Lydia : Form
    {
        [DllImport("user32.dll", SetLastError = true)] private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);
        private const uint _WDA_EXCLUDEFROMCAPTURE = 0x00000011;

        [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private Calculation _calcul;
        private bool _excelFound = false;
        private bool _snippingToolFound = false;
        private bool _closing = false;
        private string _lastClipboard = "";

        private Dictionary<IntPtr, Rectangle> _processCalc = new();
        private Dictionary<IntPtr, Hide> _hideForms = new();
        private HashSet<IntPtr> _pendingWindows = new();
        private List<ProcessWatcher> _watchers;

        private Taskmanager _taskmanager;

        public Lydia()
        {
            InitializeComponent();
            this._calcul = new Calculation();
            this.tbCalcul.AutoSize = false;

            this._watchers = new List<ProcessWatcher>
            {
                new ProcessWatcher("CalculatorApp",
                    onFound: (hWnd, bounds) => {
                        var hideForm = new Hide(bounds);
                        this._hideForms[hWnd] = hideForm;
                        hideForm.Show();
                        this.BringToFront();
                    },
                    onLost: (hWnd) => {
                        if (this._hideForms.TryGetValue(hWnd, out var f))
                        {
                            f.Close();
                            this._hideForms.Remove(hWnd);
                        }
                    }
                ),
                new ProcessWatcher("excel",
                    onFound: (hWnd, bounds) => this._excelFound = true,
                    onLost: (hWnd) => {}
                ),
                new ProcessWatcher("SnippingTool",
                    onFound: (hWnd, bounds) => this._snippingToolFound = true,
                    onLost: (hWnd) => {}
                ),
                new ProcessWatcher("ScreenClippingHost",
                    onFound: (hWnd, bounds) => this._snippingToolFound = true,
                    onLost: (hWnd) => {}
                ),
                new ProcessWatcher("SnipAndSketch",
                    onFound: (hWnd, bounds) => this._snippingToolFound = true,
                    onLost: (hWnd) => {}
                )
            };

            this._taskmanager = new Taskmanager();
        }

        private void Lydia_Load(object sender, EventArgs e)
        {
            this.tbCalcul.Text = this._calcul.Generate();
            this.AutoResizeTextBox(this.tbCalcul);

            // Centre les composents en X
            this.lblTitle.Location = new Point((this.ClientSize.Width - this.lblTitle.Width) / 2, this.lblTitle.Location.Y);
            this.tbCalcul.Location = new Point((this.ClientSize.Width - this.tbCalcul.Width) / 2, this.tbCalcul.Location.Y);
            this.tbUser.Location = new Point((this.ClientSize.Width - this.tbUser.Width) / 2, this.tbUser.Location.Y);
            this.btnValidate.Location = new Point(this.tbUser.Location.X + this.tbUser.Width + 10, this.btnValidate.Location.Y);


            this.CenterToScreen();

            Debug.WriteLine(this.tbCalcul.Text);
            Debug.WriteLine(this._calcul.GetResult());
        }

        private bool IsInvalidInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return true;

            bool hasDigit = false;
            bool hasDot = false;

            for (int i = 0; i < input.Length; i++)
            {
                int c = (int)input[i];

                // Chiffre
                if (char.IsDigit((char)c))
                {
                    hasDigit = true;
                    continue;
                }

                // Signe moins uniquement en première position
                if (c == 0x2d)
                {
                    if (i != 0) return true; // invalide
                    continue;
                }

                // Virgule
                if (c == 0x2c)
                {
                    if (hasDot) return true; // deuxième point interdit

                    // Point sans chiffre avant ou après => invalide (gère ".0", "12.")
                    if (i == 0 || i == input.Length - 1) return true;

                    hasDot = true;
                    continue;
                }

                // Caractère interdit
                return true;
            }

            return !hasDigit;
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            if (this._excelFound)
            {
                this.lblInfo.Text = "Excel à été ouvert --> nouvelle formule";
                this.tbCalcul.Text = this._calcul.Generate();
                Debug.WriteLine(this.tbCalcul.Text);
                Debug.WriteLine(this._calcul.GetResult());

                // Resets
                this._excelFound = false;
                this._snippingToolFound = false;
                return;
            }

            if (this.IsInvalidInput(this.tbUser.Text))
            {
                this.lblInfo.Text = "Caractère invalide.";
                return;
            }

            if (this._calcul.CheckResult(decimal.Parse(this.tbUser.Text)))
            {
                MessageBox.Show("C'est le bon résultat, tu es libre.");
                this._closing = true;
                this.Close();
            }
            else
            {
                this.lblInfo.Text = "Mauvais résultat, recommence.";
            }
        }

        private void tbUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnValidate.PerformClick();
            }

            if (e.Control && e.KeyCode == Keys.V)
            {
                this.tbUser.Text = "Oukilé le texte ?";
                e.SuppressKeyPress = true;
            }
        }

        private void AutoResizeTextBox(TextBox textBox)
        {
            using (Graphics g = textBox.CreateGraphics())
            {
                Size size = TextRenderer.MeasureText(textBox.Text, textBox.Font);

                textBox.Width = size.Width + 10;
            }
        }

        private void timerSpy_Tick(object sender, EventArgs e)
        {
            // Gestionnaire des tâches
            this._taskmanager.Monitor();
            if (this._taskmanager.Acknowledge)
            {
                this.lblInfo.Text = "Un gestionnaire des tâches...\nTu pensais que je le verrais pas ?";
                this._taskmanager.Acknowledge = false;
            }

            // Snipping Tool
            if (this._snippingToolFound) this.lblInfo.Text = "Et non, pas de Google Lens pour aujourd'hui";

            // Calculatrice
            foreach (var watcher in this._watchers)
            {
                var windows = ProcessUtils.GetAllWindows(watcher.ProcessName);

                foreach (var hWnd in windows)
                {
                    if (!this._processCalc.ContainsKey(hWnd) && !this._pendingWindows.Contains(hWnd))
                    {
                        this._pendingWindows.Add(hWnd);
                        Task.Run(async () =>
                        {
                            Rectangle previous = ProcessUtils.GetBounds(hWnd);
                            Rectangle bounds = previous;
                            int retries = 20;
                            while (bounds == previous && retries-- > 0)
                            {
                                await Task.Delay(500);
                                bounds = ProcessUtils.GetBounds(hWnd);
                            }
                            this.Invoke(() =>
                            {
                                this._processCalc[hWnd] = bounds;
                                this._pendingWindows.Remove(hWnd);
                                watcher.OnWindowFound(hWnd, bounds);
                            });
                        });
                    }
                }

                var alivePids = Process.GetProcessesByName(watcher.ProcessName)
                                       .Select(p => p.Id).ToHashSet();

                var closed = this._processCalc.Keys
                    .Where(h =>
                    {
                        ProcessUtils.GetWindowThreadProcessId(h, out uint pid);
                        return !alivePids.Contains((int)pid);
                    }).ToList();

                if (closed.Any())
                {
                    closed.ForEach(h =>
                    {
                        this._processCalc.Remove(h);
                        watcher.OnWindowLost(h);
                    });
                }
            }
        }

        private void timerMove_Tick(object sender, EventArgs e)
        {
            foreach (var (hWnd, hideForm) in this._hideForms)
            {
                Rectangle bounds = ProcessUtils.GetBounds(hWnd);
                if (hideForm.Location != new Point(bounds.X, bounds.Y) || hideForm.Size != new Size(bounds.Width, bounds.Height))
                {
                    hideForm.SetLocation(bounds.X, bounds.Y);
                    hideForm.SetSize(bounds.Width, bounds.Height);
                }
            }
        }

        private void Lydia_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            if (!this._closing)
            {
                this.lblInfo.Text = "Vraiment ?\nTu pensais juste pouvoir quitter comme ça ?";
                e.Cancel = true;
            }
        }

        private void Lydia_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this._closing) Application.ExitThread();
        }

        // Empêche le déplacement de la fenêtre
        protected override void WndProc(ref Message m)
        {
            const int WM_NCLBUTTONDOWN = 0x00A1;
            const int HTCAPTION = 0x2;

            if (m.Msg == WM_NCLBUTTONDOWN && (int)m.WParam == HTCAPTION)
                return;

            base.WndProc(ref m);
        }

        // Empêche la capture d'écran
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            SetWindowDisplayAffinity(this.Handle, _WDA_EXCLUDEFROMCAPTURE);
        }

        private void timerClipboard_Tick(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText()) return;

            string current = Clipboard.GetText();

            if (current != this._lastClipboard)
            {
                this._lastClipboard = current;
                Clipboard.SetText("Tu ne pensais pas que ça serait si simple quand même :)");
            }
        }
    }
}
