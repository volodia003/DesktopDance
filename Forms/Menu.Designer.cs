namespace DesktopDance.Forms
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            contextMenuStripCharacters = new ContextMenuStrip(components);
            renameMenuItem = new ToolStripMenuItem();
            deleteMenuItem = new ToolStripMenuItem();
            contextMenuStripAvailableGifs = new ContextMenuStrip(components);
            renameAvailableCharacterMenuItem = new ToolStripMenuItem();
            characterSettingsMenuItem = new ToolStripMenuItem();
            deleteGifMenuItem = new ToolStripMenuItem();
            menuStrip2 = new MenuStrip();
            loadCustomGifToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            singleCharacterModeToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            showTrayIconToolStripMenuItem = new ToolStripMenuItem();
            showMenuOnStartupToolStripMenuItem = new ToolStripMenuItem();
            minimizeOnCloseToolStripMenuItem = new ToolStripMenuItem();
            autoStartToolStripMenuItem = new ToolStripMenuItem();
            charactersListBox = new ListBox();
            charactersLabel = new Label();
            activeCharactersListBox = new ListBox();
            activeCharactersLabel = new Label();
            removeCharacterButton = new Button();
            button1 = new Button();
            button2 = new Button();
            scaleTrackBar = new TrackBar();
            scaleLabel = new Label();
            flipCheckBox = new CheckBox();
            lockCheckBox = new CheckBox();
            panel1 = new Panel();
            availableCharactersPanel = new Panel();
            activeCharactersPanel = new Panel();
            buttonsPanel = new Panel();
            loadGifButton = new Button();
            settingsButton = new Button();
            button3 = new Button();
            contextMenuStripCharacters.SuspendLayout();
            contextMenuStripAvailableGifs.SuspendLayout();
            menuStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)scaleTrackBar).BeginInit();
            panel1.SuspendLayout();
            availableCharactersPanel.SuspendLayout();
            activeCharactersPanel.SuspendLayout();
            buttonsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStripCharacters
            // 
            contextMenuStripCharacters.Items.AddRange(new ToolStripItem[] { renameMenuItem, deleteMenuItem });
            contextMenuStripCharacters.Name = "contextMenuStripCharacters";
            contextMenuStripCharacters.Size = new Size(196, 48);
            // 
            // renameMenuItem
            // 
            renameMenuItem.Name = "renameMenuItem";
            renameMenuItem.ShortcutKeys = Keys.F2;
            renameMenuItem.Size = new Size(195, 22);
            renameMenuItem.Text = "✏️ Переименовать";
            renameMenuItem.Click += renameMenuItem_Click;
            // 
            // deleteMenuItem
            // 
            deleteMenuItem.Name = "deleteMenuItem";
            deleteMenuItem.ShortcutKeys = Keys.Delete;
            deleteMenuItem.Size = new Size(195, 22);
            deleteMenuItem.Text = "🗑️ Удалить";
            deleteMenuItem.Click += deleteMenuItem_Click;
            // 
            // contextMenuStripAvailableGifs
            // 
            contextMenuStripAvailableGifs.Items.AddRange(new ToolStripItem[] { renameAvailableCharacterMenuItem, characterSettingsMenuItem, deleteGifMenuItem });
            contextMenuStripAvailableGifs.Name = "contextMenuStripAvailableGifs";
            contextMenuStripAvailableGifs.Size = new Size(214, 70);
            // 
            // renameAvailableCharacterMenuItem
            // 
            renameAvailableCharacterMenuItem.Name = "renameAvailableCharacterMenuItem";
            renameAvailableCharacterMenuItem.ShortcutKeys = Keys.F2;
            renameAvailableCharacterMenuItem.Size = new Size(213, 22);
            renameAvailableCharacterMenuItem.Text = "✏️ Переименовать";
            renameAvailableCharacterMenuItem.Click += renameAvailableCharacterMenuItem_Click;
            // 
            // characterSettingsMenuItem
            // 
            characterSettingsMenuItem.Name = "characterSettingsMenuItem";
            characterSettingsMenuItem.Size = new Size(213, 22);
            characterSettingsMenuItem.Text = "⚙️ Настройки персонажа";
            characterSettingsMenuItem.Click += characterSettingsMenuItem_Click;
            // 
            // deleteGifMenuItem
            // 
            deleteGifMenuItem.Name = "deleteGifMenuItem";
            deleteGifMenuItem.ShortcutKeys = Keys.Delete;
            deleteGifMenuItem.Size = new Size(213, 22);
            deleteGifMenuItem.Text = "🗑️ Удалить GIF";
            deleteGifMenuItem.Click += deleteGifMenuItem_Click;
            // 
            // menuStrip2
            // 
            menuStrip2.Items.AddRange(new ToolStripItem[] { loadCustomGifToolStripMenuItem, settingsToolStripMenuItem });
            menuStrip2.Location = new Point(0, 0);
            menuStrip2.Name = "menuStrip2";
            menuStrip2.Size = new Size(625, 24);
            menuStrip2.TabIndex = 1;
            menuStrip2.Text = "menuStrip2";
            menuStrip2.Visible = false;
            menuStrip2.ItemClicked += menuStrip2_ItemClicked;
            // 
            // loadCustomGifToolStripMenuItem
            // 
            loadCustomGifToolStripMenuItem.Font = new Font("Segoe UI", 9.5F);
            loadCustomGifToolStripMenuItem.Name = "loadCustomGifToolStripMenuItem";
            loadCustomGifToolStripMenuItem.Size = new Size(163, 20);
            loadCustomGifToolStripMenuItem.Text = "📁 Загрузить свой GIF...";
            loadCustomGifToolStripMenuItem.Click += loadCustomGifToolStripMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Alignment = ToolStripItemAlignment.Right;
            settingsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { singleCharacterModeToolStripMenuItem, toolStripSeparator1, showTrayIconToolStripMenuItem, showMenuOnStartupToolStripMenuItem, minimizeOnCloseToolStripMenuItem, autoStartToolStripMenuItem });
            settingsToolStripMenuItem.Font = new Font("Segoe UI", 9.5F);
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(105, 20);
            settingsToolStripMenuItem.Text = "⚙️ Настройки";
            // 
            // singleCharacterModeToolStripMenuItem
            // 
            singleCharacterModeToolStripMenuItem.CheckOnClick = true;
            singleCharacterModeToolStripMenuItem.Name = "singleCharacterModeToolStripMenuItem";
            singleCharacterModeToolStripMenuItem.Size = new Size(253, 22);
            singleCharacterModeToolStripMenuItem.Text = "👥 Много персонажей";
            singleCharacterModeToolStripMenuItem.Click += singleCharacterModeToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(250, 6);
            // 
            // showTrayIconToolStripMenuItem
            // 
            showTrayIconToolStripMenuItem.CheckOnClick = true;
            showTrayIconToolStripMenuItem.Name = "showTrayIconToolStripMenuItem";
            showTrayIconToolStripMenuItem.Size = new Size(253, 22);
            showTrayIconToolStripMenuItem.Text = "Показывать в панели задач";
            showTrayIconToolStripMenuItem.Click += showTrayIconToolStripMenuItem_Click;
            // 
            // showMenuOnStartupToolStripMenuItem
            // 
            showMenuOnStartupToolStripMenuItem.CheckOnClick = true;
            showMenuOnStartupToolStripMenuItem.Name = "showMenuOnStartupToolStripMenuItem";
            showMenuOnStartupToolStripMenuItem.Size = new Size(253, 22);
            showMenuOnStartupToolStripMenuItem.Text = "Открывать меню при запуске";
            showMenuOnStartupToolStripMenuItem.Click += showMenuOnStartupToolStripMenuItem_Click;
            // 
            // minimizeOnCloseToolStripMenuItem
            // 
            minimizeOnCloseToolStripMenuItem.Checked = true;
            minimizeOnCloseToolStripMenuItem.CheckOnClick = true;
            minimizeOnCloseToolStripMenuItem.CheckState = CheckState.Checked;
            minimizeOnCloseToolStripMenuItem.Name = "minimizeOnCloseToolStripMenuItem";
            minimizeOnCloseToolStripMenuItem.Size = new Size(253, 22);
            minimizeOnCloseToolStripMenuItem.Text = "Сворачивать при закрытии";
            minimizeOnCloseToolStripMenuItem.Click += minimizeOnCloseToolStripMenuItem_Click;
            // 
            // autoStartToolStripMenuItem
            // 
            autoStartToolStripMenuItem.CheckOnClick = true;
            autoStartToolStripMenuItem.Name = "autoStartToolStripMenuItem";
            autoStartToolStripMenuItem.Size = new Size(253, 22);
            autoStartToolStripMenuItem.Text = "Автозапуск";
            autoStartToolStripMenuItem.Click += autoStartToolStripMenuItem_Click;
            // 
            // charactersListBox
            // 
            charactersListBox.BackColor = Color.White;
            charactersListBox.BorderStyle = BorderStyle.None;
            charactersListBox.ContextMenuStrip = contextMenuStripAvailableGifs;
            charactersListBox.Font = new Font("Segoe UI", 9.5F);
            charactersListBox.FormattingEnabled = true;
            charactersListBox.ItemHeight = 17;
            charactersListBox.Items.AddRange(new object[] { "🎭 blin4iik Dance", "💝 Konata Love" });
            charactersListBox.Location = new Point(10, 40);
            charactersListBox.Name = "charactersListBox";
            charactersListBox.Size = new Size(240, 136);
            charactersListBox.TabIndex = 9;
            charactersListBox.DoubleClick += charactersListBox_DoubleClick;
            charactersListBox.KeyDown += charactersListBox_KeyDown;
            // 
            // charactersLabel
            // 
            charactersLabel.AutoSize = true;
            charactersLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            charactersLabel.ForeColor = Color.FromArgb(44, 62, 80);
            charactersLabel.Location = new Point(10, 10);
            charactersLabel.Name = "charactersLabel";
            charactersLabel.Size = new Size(115, 19);
            charactersLabel.TabIndex = 10;
            charactersLabel.Text = "🎭 Персонажи";
            // 
            // activeCharactersListBox
            // 
            activeCharactersListBox.BackColor = Color.White;
            activeCharactersListBox.BorderStyle = BorderStyle.None;
            activeCharactersListBox.ContextMenuStrip = contextMenuStripCharacters;
            activeCharactersListBox.Font = new Font("Segoe UI", 9.5F);
            activeCharactersListBox.FormattingEnabled = true;
            activeCharactersListBox.ItemHeight = 17;
            activeCharactersListBox.Location = new Point(10, 40);
            activeCharactersListBox.Name = "activeCharactersListBox";
            activeCharactersListBox.Size = new Size(240, 119);
            activeCharactersListBox.TabIndex = 12;
            activeCharactersListBox.SelectedIndexChanged += activeCharactersListBox_SelectedIndexChanged;
            activeCharactersListBox.KeyDown += activeCharactersListBox_KeyDown;
            // 
            // activeCharactersLabel
            // 
            activeCharactersLabel.AutoSize = true;
            activeCharactersLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            activeCharactersLabel.ForeColor = Color.FromArgb(44, 62, 80);
            activeCharactersLabel.Location = new Point(10, 10);
            activeCharactersLabel.Name = "activeCharactersLabel";
            activeCharactersLabel.Size = new Size(124, 19);
            activeCharactersLabel.TabIndex = 13;
            activeCharactersLabel.Text = "👥 Активные (0)";
            // 
            // removeCharacterButton
            // 
            removeCharacterButton.BackColor = Color.FromArgb(220, 53, 69);
            removeCharacterButton.FlatStyle = FlatStyle.Flat;
            removeCharacterButton.Font = new Font("Segoe UI", 9F);
            removeCharacterButton.ForeColor = Color.White;
            removeCharacterButton.Location = new Point(999, 999);
            removeCharacterButton.Name = "removeCharacterButton";
            removeCharacterButton.Size = new Size(1, 1);
            removeCharacterButton.TabIndex = 14;
            removeCharacterButton.Text = "❌ Удалить выбранного";
            removeCharacterButton.UseVisualStyleBackColor = false;
            removeCharacterButton.Visible = false;
            removeCharacterButton.Click += removeCharacterButton_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(220, 53, 69);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 9.5F);
            button1.ForeColor = Color.White;
            button1.Location = new Point(999, 999);
            button1.Name = "button1";
            button1.Size = new Size(1, 1);
            button1.TabIndex = 2;
            button1.Text = "❌ Выйти";
            button1.UseVisualStyleBackColor = false;
            button1.Visible = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(108, 117, 125);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI", 9.5F);
            button2.ForeColor = Color.White;
            button2.Location = new Point(999, 999);
            button2.Name = "button2";
            button2.Size = new Size(1, 1);
            button2.TabIndex = 3;
            button2.Text = "👁️ Скрыть";
            button2.UseVisualStyleBackColor = false;
            button2.Visible = false;
            button2.Click += button2_Click;
            // 
            // scaleTrackBar
            // 
            scaleTrackBar.Location = new Point(10, 40);
            scaleTrackBar.Maximum = 200;
            scaleTrackBar.Minimum = 25;
            scaleTrackBar.Name = "scaleTrackBar";
            scaleTrackBar.Size = new Size(298, 45);
            scaleTrackBar.TabIndex = 5;
            scaleTrackBar.TickFrequency = 25;
            scaleTrackBar.Value = 100;
            scaleTrackBar.Scroll += scaleTrackBar_Scroll;
            // 
            // scaleLabel
            // 
            scaleLabel.AutoSize = true;
            scaleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            scaleLabel.ForeColor = Color.FromArgb(44, 62, 80);
            scaleLabel.Location = new Point(10, 10);
            scaleLabel.Name = "scaleLabel";
            scaleLabel.Size = new Size(129, 19);
            scaleLabel.TabIndex = 6;
            scaleLabel.Text = "🎨 Размер: 100%";
            // 
            // flipCheckBox
            // 
            flipCheckBox.AutoSize = true;
            flipCheckBox.Font = new Font("Segoe UI", 10F);
            flipCheckBox.ForeColor = Color.FromArgb(44, 62, 80);
            flipCheckBox.Location = new Point(10, 100);
            flipCheckBox.Name = "flipCheckBox";
            flipCheckBox.Size = new Size(155, 23);
            flipCheckBox.TabIndex = 7;
            flipCheckBox.Text = "🔄 Отзеркаливание";
            flipCheckBox.UseVisualStyleBackColor = true;
            flipCheckBox.CheckedChanged += flipCheckBox_CheckedChanged;
            // 
            // lockCheckBox
            // 
            lockCheckBox.AutoSize = true;
            lockCheckBox.Font = new Font("Segoe UI", 10F);
            lockCheckBox.ForeColor = Color.FromArgb(44, 62, 80);
            lockCheckBox.Location = new Point(10, 135);
            lockCheckBox.Name = "lockCheckBox";
            lockCheckBox.Size = new Size(240, 23);
            lockCheckBox.TabIndex = 8;
            lockCheckBox.Text = "🔒 Заблокировать перемещение";
            lockCheckBox.UseVisualStyleBackColor = true;
            lockCheckBox.CheckedChanged += lockCheckBox_CheckedChanged;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(scaleLabel);
            panel1.Controls.Add(scaleTrackBar);
            panel1.Controls.Add(flipCheckBox);
            panel1.Controls.Add(lockCheckBox);
            panel1.Location = new Point(290, 35);
            panel1.Name = "panel1";
            panel1.Size = new Size(320, 190);
            panel1.TabIndex = 11;
            // 
            // availableCharactersPanel
            // 
            availableCharactersPanel.BackColor = Color.White;
            availableCharactersPanel.BorderStyle = BorderStyle.FixedSingle;
            availableCharactersPanel.Controls.Add(charactersLabel);
            availableCharactersPanel.Controls.Add(charactersListBox);
            availableCharactersPanel.Location = new Point(15, 35);
            availableCharactersPanel.Name = "availableCharactersPanel";
            availableCharactersPanel.Size = new Size(265, 190);
            availableCharactersPanel.TabIndex = 15;
            // 
            // activeCharactersPanel
            // 
            activeCharactersPanel.BackColor = Color.White;
            activeCharactersPanel.BorderStyle = BorderStyle.FixedSingle;
            activeCharactersPanel.Controls.Add(activeCharactersLabel);
            activeCharactersPanel.Controls.Add(activeCharactersListBox);
            activeCharactersPanel.Location = new Point(15, 235);
            activeCharactersPanel.Name = "activeCharactersPanel";
            activeCharactersPanel.Size = new Size(265, 175);
            activeCharactersPanel.TabIndex = 16;
            // 
            // buttonsPanel
            // 
            buttonsPanel.BackColor = Color.White;
            buttonsPanel.BorderStyle = BorderStyle.FixedSingle;
            buttonsPanel.Controls.Add(loadGifButton);
            buttonsPanel.Controls.Add(settingsButton);
            buttonsPanel.Controls.Add(button3);
            buttonsPanel.Location = new Point(290, 235);
            buttonsPanel.Name = "buttonsPanel";
            buttonsPanel.Size = new Size(320, 175);
            buttonsPanel.TabIndex = 17;
            // 
            // loadGifButton
            // 
            loadGifButton.BackColor = Color.FromArgb(0, 123, 255);
            loadGifButton.FlatStyle = FlatStyle.Flat;
            loadGifButton.Font = new Font("Segoe UI", 10F);
            loadGifButton.ForeColor = Color.White;
            loadGifButton.Location = new Point(10, 10);
            loadGifButton.Name = "loadGifButton";
            loadGifButton.Size = new Size(298, 40);
            loadGifButton.TabIndex = 0;
            loadGifButton.Text = "📁 Загрузить свой GIF";
            loadGifButton.UseVisualStyleBackColor = false;
            loadGifButton.Click += loadCustomGifToolStripMenuItem_Click;
            // 
            // settingsButton
            // 
            settingsButton.BackColor = Color.FromArgb(0, 123, 255);
            settingsButton.FlatStyle = FlatStyle.Flat;
            settingsButton.Font = new Font("Segoe UI", 10F);
            settingsButton.ForeColor = Color.White;
            settingsButton.Location = new Point(10, 56);
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(298, 40);
            settingsButton.TabIndex = 1;
            settingsButton.Text = "⚙️ Настройки";
            settingsButton.UseVisualStyleBackColor = false;
            settingsButton.Click += settingsButton_Click;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(0, 123, 255);
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI", 10F);
            button3.ForeColor = Color.Black;
            button3.Location = new Point(10, 119);
            button3.Name = "button3";
            button3.Size = new Size(298, 40);
            button3.TabIndex = 2;
            button3.Text = "\U0001f9f9 Очистить все";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // Menu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 242, 245);
            ClientSize = new Size(625, 425);
            Controls.Add(buttonsPanel);
            Controls.Add(activeCharactersPanel);
            Controls.Add(availableCharactersPanel);
            Controls.Add(panel1);
            Controls.Add(removeCharacterButton);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(menuStrip2);
            Font = new Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Menu";
            ShowInTaskbar = false;
            Text = "Desktop Dance - Управление персонажами";
            FormClosing += Menu_FormClosing;
            Load += Menu_Load;
            contextMenuStripCharacters.ResumeLayout(false);
            contextMenuStripAvailableGifs.ResumeLayout(false);
            menuStrip2.ResumeLayout(false);
            menuStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)scaleTrackBar).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            availableCharactersPanel.ResumeLayout(false);
            availableCharactersPanel.PerformLayout();
            activeCharactersPanel.ResumeLayout(false);
            activeCharactersPanel.PerformLayout();
            buttonsPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip2;
        private Button button1;
        private Button button2;
        private Button button3;
        private TrackBar scaleTrackBar;
        private Label scaleLabel;
        private CheckBox flipCheckBox;
        private CheckBox lockCheckBox;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem minimizeOnCloseToolStripMenuItem;
        private ToolStripMenuItem autoStartToolStripMenuItem;
        private ToolStripMenuItem loadCustomGifToolStripMenuItem;
        private ListBox charactersListBox;
        private Label charactersLabel;
        private Panel panel1;
        private Panel availableCharactersPanel;
        private Panel activeCharactersPanel;
        private Panel buttonsPanel;
        private Button loadGifButton;
        private Button settingsButton;
        private ListBox activeCharactersListBox;
        private Label activeCharactersLabel;
        private Button removeCharacterButton;
        private ToolStripMenuItem singleCharacterModeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem showTrayIconToolStripMenuItem;
        private ToolStripMenuItem showMenuOnStartupToolStripMenuItem;
        private ContextMenuStrip contextMenuStripCharacters;
        private ToolStripMenuItem renameMenuItem;
        private ToolStripMenuItem deleteMenuItem;
        private ContextMenuStrip contextMenuStripAvailableGifs;
        private ToolStripMenuItem renameAvailableCharacterMenuItem;
        private ToolStripMenuItem characterSettingsMenuItem;
        private ToolStripMenuItem deleteGifMenuItem;
    }
}