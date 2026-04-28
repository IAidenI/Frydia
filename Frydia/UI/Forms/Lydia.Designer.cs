namespace Frank
{
    partial class Lydia
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tbUser = new TextBox();
            lblTitle = new Label();
            btnValidate = new Button();
            tbCalcul = new TextBox();
            timerSpy = new System.Windows.Forms.Timer(components);
            timerMove = new System.Windows.Forms.Timer(components);
            lblInfo = new Label();
            SuspendLayout();
            // 
            // tbUser
            // 
            tbUser.Location = new Point(292, 199);
            tbUser.Name = "tbUser";
            tbUser.PlaceholderText = "Saisir le résultat";
            tbUser.Size = new Size(206, 23);
            tbUser.TabIndex = 0;
            tbUser.KeyDown += tbUser_KeyDown;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(292, 82);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(217, 15);
            lblTitle.TabIndex = 2;
            lblTitle.Text = "Punition : Vous devez résoudre ce calcul";
            // 
            // btnValidate
            // 
            btnValidate.Location = new Point(516, 199);
            btnValidate.Name = "btnValidate";
            btnValidate.Size = new Size(75, 23);
            btnValidate.TabIndex = 3;
            btnValidate.Text = "Valider";
            btnValidate.UseVisualStyleBackColor = true;
            btnValidate.Click += btnValidate_Click;
            // 
            // tbCalcul
            // 
            tbCalcul.BorderStyle = BorderStyle.None;
            tbCalcul.Location = new Point(187, 128);
            tbCalcul.Name = "tbCalcul";
            tbCalcul.ReadOnly = true;
            tbCalcul.Size = new Size(151, 16);
            tbCalcul.TabIndex = 4;
            tbCalcul.Text = "Calcul";
            tbCalcul.KeyDown += tbCalcul_KeyDown;
            // 
            // timerSpy
            // 
            timerSpy.Enabled = true;
            timerSpy.Tick += timerSpy_Tick;
            // 
            // timerMove
            // 
            timerMove.Enabled = true;
            timerMove.Tick += timerMove_Tick;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(284, 328);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(28, 15);
            lblInfo.TabIndex = 5;
            lblInfo.Text = "Info";
            // 
            // Lydia
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblInfo);
            Controls.Add(tbCalcul);
            Controls.Add(btnValidate);
            Controls.Add(lblTitle);
            Controls.Add(tbUser);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "Lydia";
            ShowInTaskbar = false;
            Text = "Lydia";
            TopMost = true;
            FormClosing += Lydia_FormClosing;
            FormClosed += Lydia_FormClosed;
            Load += Lydia_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tbUser;
        private Label lblTitle;
        private Button btnValidate;
        private TextBox tbCalcul;
        private System.Windows.Forms.Timer timerSpy;
        private System.Windows.Forms.Timer timerMove;
        private Label lblInfo;
    }
}