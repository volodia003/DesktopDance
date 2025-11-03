namespace DesktopKonata.Forms
{
    partial class Menu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            menuStrip2 = new MenuStrip();
            konataDanceToolStripMenuItem = new ToolStripMenuItem();
            KonataLoveStripMenuItem = new ToolStripMenuItem();
            bananaKonataToolStripMenuItem = new ToolStripMenuItem();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            menuStrip2.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip2
            // 
            menuStrip2.Items.AddRange(new ToolStripItem[] { konataDanceToolStripMenuItem, KonataLoveStripMenuItem, bananaKonataToolStripMenuItem });
            menuStrip2.Location = new Point(0, 0);
            menuStrip2.Name = "menuStrip2";
            menuStrip2.Size = new Size(395, 24);
            menuStrip2.TabIndex = 1;
            menuStrip2.Text = "menuStrip2";
            menuStrip2.ItemClicked += menuStrip2_ItemClicked;
            // 
            // konataDanceToolStripMenuItem
            // 
            konataDanceToolStripMenuItem.Name = "konataDanceToolStripMenuItem";
            konataDanceToolStripMenuItem.Size = new Size(92, 20);
            konataDanceToolStripMenuItem.Text = "Konata Dance";
            konataDanceToolStripMenuItem.Click += konataDanceToolStripMenuItem_Click;
            // 
            // KonataLoveStripMenuItem
            // 
            KonataLoveStripMenuItem.Name = "KonataLoveStripMenuItem";
            KonataLoveStripMenuItem.Size = new Size(84, 20);
            KonataLoveStripMenuItem.Text = "Konata Love";
            KonataLoveStripMenuItem.Click += KonataLoveToolStripMenuItem_Click;
            // 
            // bananaKonataToolStripMenuItem
            // 
            bananaKonataToolStripMenuItem.Name = "bananaKonataToolStripMenuItem";
            bananaKonataToolStripMenuItem.Size = new Size(98, 20);
            bananaKonataToolStripMenuItem.Text = "Banana Konata";
            bananaKonataToolStripMenuItem.Click += bananaKonataToolStripMenuItem_Click;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ButtonFace;
            button1.ForeColor = SystemColors.ActiveCaptionText;
            button1.Location = new Point(308, 160);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 2;
            button1.Text = "Exit";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackColor = SystemColors.ButtonFace;
            button2.Location = new Point(227, 160);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 3;
            button2.Text = "Hide";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.BackColor = SystemColors.ButtonFace;
            button3.Location = new Point(146, 160);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 4;
            button3.Text = "Clear\r\n";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // Menu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(395, 195);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(menuStrip2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Menu";
            Text = "Desktop Konata";
            TopMost = true;
            FormClosing += Menu_FormClosing;
            Load += Menu_Load;
            menuStrip2.ResumeLayout(false);
            menuStrip2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip2;
        private ToolStripMenuItem KonataLoveStripMenuItem;
        private Button button1;
        private Button button2;
        private Button button3;
        private ToolStripMenuItem bananaKonataToolStripMenuItem;
        private ToolStripMenuItem konataDanceToolStripMenuItem;
    }
}