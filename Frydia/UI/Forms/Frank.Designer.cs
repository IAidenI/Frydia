namespace Frank
{
    partial class Frank
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frank));
            pbBackground = new PictureBox();
            timer = new System.Windows.Forms.Timer(components);
            btnEmergency = new Button();
            ((System.ComponentModel.ISupportInitialize)pbBackground).BeginInit();
            SuspendLayout();
            // 
            // pbBackground
            // 
            pbBackground.Dock = DockStyle.Fill;
            pbBackground.Image = (Image)resources.GetObject("pbBackground.Image");
            pbBackground.Location = new Point(0, 0);
            pbBackground.Name = "pbBackground";
            pbBackground.Size = new Size(800, 450);
            pbBackground.SizeMode = PictureBoxSizeMode.StretchImage;
            pbBackground.TabIndex = 5;
            pbBackground.TabStop = false;
            pbBackground.Paint += pbBackground_Paint;
            // 
            // timer
            // 
            timer.Interval = 1000;
            timer.Tick += timer_Tick;
            // 
            // btnEmergency
            // 
            btnEmergency.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEmergency.Location = new Point(795, 444);
            btnEmergency.Name = "btnEmergency";
            btnEmergency.Size = new Size(5, 5);
            btnEmergency.TabIndex = 8;
            btnEmergency.UseVisualStyleBackColor = true;
            btnEmergency.Click += btnEmergency_Click;
            // 
            // Frank
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnEmergency);
            Controls.Add(pbBackground);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Frank";
            ShowInTaskbar = false;
            Text = "Frank";
            TopMost = true;
            WindowState = FormWindowState.Maximized;
            FormClosing += Frank_FormClosing;
            FormClosed += Frank_FormClosed;
            ((System.ComponentModel.ISupportInitialize)pbBackground).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Timer timer;
        private PictureBox pbBackground;
        private Button btnEmergency;
    }
}
