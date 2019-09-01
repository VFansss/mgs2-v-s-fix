namespace mgs2_v_s_fix
{
    partial class ControlsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlsForm));
            this.btn_returnToForm1 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tab_KbMouse = new System.Windows.Forms.TabPage();
            this.tab_Gamepads = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbl_controllerGuide = new System.Windows.Forms.Label();
            this.pnl_EnableController = new System.Windows.Forms.Panel();
            this.EnableController_STEAM = new System.Windows.Forms.RadioButton();
            this.EnableController_DS4 = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.EnableController_XBOX = new System.Windows.Forms.RadioButton();
            this.EnableController_NO = new System.Windows.Forms.RadioButton();
            this.pnl_PreferredLayout = new System.Windows.Forms.Panel();
            this.pnl_LayoutChooser = new System.Windows.Forms.Panel();
            this.PreferredLayout_PS2 = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.PreferredLayout_V = new System.Windows.Forms.RadioButton();
            this.tab_ControlsHelp = new System.Windows.Forms.TabPage();
            this.help_controls = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tab_Gamepads.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnl_EnableController.SuspendLayout();
            this.pnl_PreferredLayout.SuspendLayout();
            this.pnl_LayoutChooser.SuspendLayout();
            this.tab_ControlsHelp.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_returnToForm1
            // 
            this.btn_returnToForm1.BackColor = System.Drawing.Color.White;
            this.btn_returnToForm1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_returnToForm1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_returnToForm1.FlatAppearance.BorderSize = 0;
            this.btn_returnToForm1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_returnToForm1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_returnToForm1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_returnToForm1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_returnToForm1.ForeColor = System.Drawing.Color.Black;
            this.btn_returnToForm1.Location = new System.Drawing.Point(283, 13);
            this.btn_returnToForm1.Margin = new System.Windows.Forms.Padding(4);
            this.btn_returnToForm1.Name = "btn_returnToForm1";
            this.btn_returnToForm1.Size = new System.Drawing.Size(515, 52);
            this.btn_returnToForm1.TabIndex = 2;
            this.btn_returnToForm1.Text = "Confirm and go back";
            this.btn_returnToForm1.UseVisualStyleBackColor = false;
            this.btn_returnToForm1.Click += new System.EventHandler(this.btn_returnToForm1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tab_KbMouse);
            this.tabControl1.Controls.Add(this.tab_Gamepads);
            this.tabControl1.Controls.Add(this.tab_ControlsHelp);
            this.tabControl1.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tabControl1.Location = new System.Drawing.Point(84, 73);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(919, 574);
            this.tabControl1.TabIndex = 2;
            // 
            // tab_KbMouse
            // 
            this.tab_KbMouse.BackColor = System.Drawing.Color.White;
            this.tab_KbMouse.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tab_KbMouse.Location = new System.Drawing.Point(4, 38);
            this.tab_KbMouse.Name = "tab_KbMouse";
            this.tab_KbMouse.Size = new System.Drawing.Size(911, 485);
            this.tab_KbMouse.TabIndex = 0;
            this.tab_KbMouse.Text = "Keyboard + Mouse";
            // 
            // tab_Gamepads
            // 
            this.tab_Gamepads.BackColor = System.Drawing.Color.White;
            this.tab_Gamepads.Controls.Add(this.pictureBox1);
            this.tab_Gamepads.Controls.Add(this.lbl_controllerGuide);
            this.tab_Gamepads.Controls.Add(this.pnl_EnableController);
            this.tab_Gamepads.Controls.Add(this.pnl_PreferredLayout);
            this.tab_Gamepads.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tab_Gamepads.Location = new System.Drawing.Point(4, 38);
            this.tab_Gamepads.Name = "tab_Gamepads";
            this.tab_Gamepads.Size = new System.Drawing.Size(911, 532);
            this.tab_Gamepads.TabIndex = 1;
            this.tab_Gamepads.Text = "Gamepads";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.Location = new System.Drawing.Point(112, 195);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(687, 332);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // lbl_controllerGuide
            // 
            this.lbl_controllerGuide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_controllerGuide.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl_controllerGuide.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_controllerGuide.Location = new System.Drawing.Point(109, 54);
            this.lbl_controllerGuide.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_controllerGuide.Name = "lbl_controllerGuide";
            this.lbl_controllerGuide.Size = new System.Drawing.Size(693, 23);
            this.lbl_controllerGuide.TabIndex = 19;
            this.lbl_controllerGuide.Text = "[RUNTIME]";
            this.lbl_controllerGuide.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_controllerGuide.Visible = false;
            // 
            // pnl_EnableController
            // 
            this.pnl_EnableController.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnl_EnableController.Controls.Add(this.EnableController_STEAM);
            this.pnl_EnableController.Controls.Add(this.EnableController_DS4);
            this.pnl_EnableController.Controls.Add(this.label8);
            this.pnl_EnableController.Controls.Add(this.EnableController_XBOX);
            this.pnl_EnableController.Controls.Add(this.EnableController_NO);
            this.pnl_EnableController.Location = new System.Drawing.Point(95, 9);
            this.pnl_EnableController.Margin = new System.Windows.Forms.Padding(4);
            this.pnl_EnableController.Name = "pnl_EnableController";
            this.pnl_EnableController.Size = new System.Drawing.Size(720, 39);
            this.pnl_EnableController.TabIndex = 18;
            // 
            // EnableController_STEAM
            // 
            this.EnableController_STEAM.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.EnableController_STEAM.Appearance = System.Windows.Forms.Appearance.Button;
            this.EnableController_STEAM.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.EnableController_STEAM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnableController_STEAM.Enabled = false;
            this.EnableController_STEAM.FlatAppearance.BorderSize = 0;
            this.EnableController_STEAM.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.EnableController_STEAM.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.EnableController_STEAM.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.EnableController_STEAM.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EnableController_STEAM.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnableController_STEAM.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.EnableController_STEAM.Location = new System.Drawing.Point(551, 6);
            this.EnableController_STEAM.Margin = new System.Windows.Forms.Padding(4);
            this.EnableController_STEAM.Name = "EnableController_STEAM";
            this.EnableController_STEAM.Size = new System.Drawing.Size(155, 37);
            this.EnableController_STEAM.TabIndex = 5;
            this.EnableController_STEAM.TabStop = true;
            this.EnableController_STEAM.Text = "Steam: Soon ™";
            this.EnableController_STEAM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.EnableController_STEAM.UseVisualStyleBackColor = false;
            // 
            // EnableController_DS4
            // 
            this.EnableController_DS4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.EnableController_DS4.Appearance = System.Windows.Forms.Appearance.Button;
            this.EnableController_DS4.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.EnableController_DS4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnableController_DS4.FlatAppearance.BorderSize = 0;
            this.EnableController_DS4.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.EnableController_DS4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.EnableController_DS4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.EnableController_DS4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EnableController_DS4.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnableController_DS4.Location = new System.Drawing.Point(384, 5);
            this.EnableController_DS4.Margin = new System.Windows.Forms.Padding(4);
            this.EnableController_DS4.Name = "EnableController_DS4";
            this.EnableController_DS4.Size = new System.Drawing.Size(160, 37);
            this.EnableController_DS4.TabIndex = 4;
            this.EnableController_DS4.TabStop = true;
            this.EnableController_DS4.Text = "DualShock 4";
            this.EnableController_DS4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.EnableController_DS4.UseVisualStyleBackColor = false;
            this.EnableController_DS4.CheckedChanged += new System.EventHandler(this.setNewColor);
            this.EnableController_DS4.Click += new System.EventHandler(this.EnableController_Click);
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Calibri", 15.75F);
            this.label8.Location = new System.Drawing.Point(31, 7);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(130, 33);
            this.label8.TabIndex = 3;
            this.label8.Text = "Controller:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // EnableController_XBOX
            // 
            this.EnableController_XBOX.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.EnableController_XBOX.Appearance = System.Windows.Forms.Appearance.Button;
            this.EnableController_XBOX.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.EnableController_XBOX.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnableController_XBOX.FlatAppearance.BorderSize = 0;
            this.EnableController_XBOX.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.EnableController_XBOX.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.EnableController_XBOX.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.EnableController_XBOX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EnableController_XBOX.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnableController_XBOX.Location = new System.Drawing.Point(255, 5);
            this.EnableController_XBOX.Margin = new System.Windows.Forms.Padding(4);
            this.EnableController_XBOX.Name = "EnableController_XBOX";
            this.EnableController_XBOX.Size = new System.Drawing.Size(125, 37);
            this.EnableController_XBOX.TabIndex = 1;
            this.EnableController_XBOX.TabStop = true;
            this.EnableController_XBOX.Text = "Xbox";
            this.EnableController_XBOX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.EnableController_XBOX.UseVisualStyleBackColor = false;
            this.EnableController_XBOX.CheckedChanged += new System.EventHandler(this.setNewColor);
            this.EnableController_XBOX.Click += new System.EventHandler(this.EnableController_Click);
            // 
            // EnableController_NO
            // 
            this.EnableController_NO.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.EnableController_NO.Appearance = System.Windows.Forms.Appearance.Button;
            this.EnableController_NO.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.EnableController_NO.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnableController_NO.FlatAppearance.BorderSize = 0;
            this.EnableController_NO.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.EnableController_NO.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.EnableController_NO.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.EnableController_NO.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EnableController_NO.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnableController_NO.Location = new System.Drawing.Point(172, 5);
            this.EnableController_NO.Margin = new System.Windows.Forms.Padding(4);
            this.EnableController_NO.Name = "EnableController_NO";
            this.EnableController_NO.Size = new System.Drawing.Size(77, 37);
            this.EnableController_NO.TabIndex = 0;
            this.EnableController_NO.TabStop = true;
            this.EnableController_NO.Text = "NO";
            this.EnableController_NO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.EnableController_NO.UseVisualStyleBackColor = false;
            this.EnableController_NO.CheckedChanged += new System.EventHandler(this.setNewColor);
            this.EnableController_NO.Click += new System.EventHandler(this.EnableController_Click);
            // 
            // pnl_PreferredLayout
            // 
            this.pnl_PreferredLayout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnl_PreferredLayout.Controls.Add(this.pnl_LayoutChooser);
            this.pnl_PreferredLayout.Location = new System.Drawing.Point(95, 87);
            this.pnl_PreferredLayout.Margin = new System.Windows.Forms.Padding(4);
            this.pnl_PreferredLayout.Name = "pnl_PreferredLayout";
            this.pnl_PreferredLayout.Size = new System.Drawing.Size(728, 445);
            this.pnl_PreferredLayout.TabIndex = 20;
            // 
            // pnl_LayoutChooser
            // 
            this.pnl_LayoutChooser.Controls.Add(this.PreferredLayout_PS2);
            this.pnl_LayoutChooser.Controls.Add(this.label10);
            this.pnl_LayoutChooser.Controls.Add(this.PreferredLayout_V);
            this.pnl_LayoutChooser.Location = new System.Drawing.Point(114, 56);
            this.pnl_LayoutChooser.Margin = new System.Windows.Forms.Padding(4);
            this.pnl_LayoutChooser.Name = "pnl_LayoutChooser";
            this.pnl_LayoutChooser.Size = new System.Drawing.Size(532, 48);
            this.pnl_LayoutChooser.TabIndex = 12;
            // 
            // PreferredLayout_PS2
            // 
            this.PreferredLayout_PS2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.PreferredLayout_PS2.Appearance = System.Windows.Forms.Appearance.Button;
            this.PreferredLayout_PS2.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PreferredLayout_PS2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PreferredLayout_PS2.FlatAppearance.BorderSize = 0;
            this.PreferredLayout_PS2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.PreferredLayout_PS2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.PreferredLayout_PS2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.PreferredLayout_PS2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PreferredLayout_PS2.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PreferredLayout_PS2.Location = new System.Drawing.Point(372, 5);
            this.PreferredLayout_PS2.Margin = new System.Windows.Forms.Padding(4);
            this.PreferredLayout_PS2.Name = "PreferredLayout_PS2";
            this.PreferredLayout_PS2.Size = new System.Drawing.Size(148, 37);
            this.PreferredLayout_PS2.TabIndex = 7;
            this.PreferredLayout_PS2.TabStop = true;
            this.PreferredLayout_PS2.Text = "PS2 Type";
            this.PreferredLayout_PS2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PreferredLayout_PS2.UseVisualStyleBackColor = false;
            this.PreferredLayout_PS2.CheckedChanged += new System.EventHandler(this.setNewColor);
            this.PreferredLayout_PS2.Click += new System.EventHandler(this.PreferredLayout_Click);
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Calibri", 15.75F);
            this.label10.Location = new System.Drawing.Point(12, 7);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(185, 33);
            this.label10.TabIndex = 6;
            this.label10.Text = "Buttons Layout:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PreferredLayout_V
            // 
            this.PreferredLayout_V.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.PreferredLayout_V.Appearance = System.Windows.Forms.Appearance.Button;
            this.PreferredLayout_V.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PreferredLayout_V.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PreferredLayout_V.FlatAppearance.BorderSize = 0;
            this.PreferredLayout_V.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.PreferredLayout_V.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.PreferredLayout_V.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.PreferredLayout_V.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PreferredLayout_V.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PreferredLayout_V.Location = new System.Drawing.Point(243, 5);
            this.PreferredLayout_V.Margin = new System.Windows.Forms.Padding(4);
            this.PreferredLayout_V.Name = "PreferredLayout_V";
            this.PreferredLayout_V.Size = new System.Drawing.Size(125, 37);
            this.PreferredLayout_V.TabIndex = 5;
            this.PreferredLayout_V.TabStop = true;
            this.PreferredLayout_V.Text = "V\'s Type";
            this.PreferredLayout_V.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PreferredLayout_V.UseVisualStyleBackColor = false;
            this.PreferredLayout_V.CheckedChanged += new System.EventHandler(this.setNewColor);
            this.PreferredLayout_V.Click += new System.EventHandler(this.PreferredLayout_Click);
            // 
            // tab_ControlsHelp
            // 
            this.tab_ControlsHelp.BackColor = System.Drawing.Color.White;
            this.tab_ControlsHelp.Controls.Add(this.help_controls);
            this.tab_ControlsHelp.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tab_ControlsHelp.Location = new System.Drawing.Point(4, 38);
            this.tab_ControlsHelp.Name = "tab_ControlsHelp";
            this.tab_ControlsHelp.Size = new System.Drawing.Size(911, 485);
            this.tab_ControlsHelp.TabIndex = 2;
            this.tab_ControlsHelp.Text = "HELP";
            // 
            // help_controls
            // 
            this.help_controls.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.help_controls.AutoSize = true;
            this.help_controls.Cursor = System.Windows.Forms.Cursors.Hand;
            this.help_controls.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.help_controls.Location = new System.Drawing.Point(168, 230);
            this.help_controls.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.help_controls.Name = "help_controls";
            this.help_controls.Size = new System.Drawing.Size(575, 24);
            this.help_controls.TabIndex = 18;
            this.help_controls.Text = "Be sure to select the right VGA from the list below! More info here!";
            this.help_controls.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.help_controls.Click += new System.EventHandler(this.help_controls_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(192, 71);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(192, 71);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ControlsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1065, 653);
            this.ControlBox = false;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btn_returnToForm1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ControlsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Choose your weapon";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlsForm_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tab_Gamepads.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnl_EnableController.ResumeLayout(false);
            this.pnl_EnableController.PerformLayout();
            this.pnl_PreferredLayout.ResumeLayout(false);
            this.pnl_LayoutChooser.ResumeLayout(false);
            this.pnl_LayoutChooser.PerformLayout();
            this.tab_ControlsHelp.ResumeLayout(false);
            this.tab_ControlsHelp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        internal System.Windows.Forms.Button btn_returnToForm1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tab_KbMouse;
        private System.Windows.Forms.TabPage tab_Gamepads;
        private System.Windows.Forms.TabPage tab_ControlsHelp;
        internal System.Windows.Forms.PictureBox pictureBox1;
        internal System.Windows.Forms.Label lbl_controllerGuide;
        internal System.Windows.Forms.Panel pnl_EnableController;
        internal System.Windows.Forms.RadioButton EnableController_STEAM;
        internal System.Windows.Forms.RadioButton EnableController_DS4;
        internal System.Windows.Forms.Label label8;
        internal System.Windows.Forms.RadioButton EnableController_XBOX;
        internal System.Windows.Forms.RadioButton EnableController_NO;
        internal System.Windows.Forms.Panel pnl_PreferredLayout;
        private System.Windows.Forms.Panel pnl_LayoutChooser;
        internal System.Windows.Forms.RadioButton PreferredLayout_PS2;
        internal System.Windows.Forms.Label label10;
        internal System.Windows.Forms.RadioButton PreferredLayout_V;
        private System.Windows.Forms.Label help_controls;
    }
}