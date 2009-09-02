namespace FogBugzCaseTracker
{
    partial class SettingsDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDlg));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpAppearance = new System.Windows.Forms.GroupBox();
            this.numOpacity = new System.Windows.Forms.NumericUpDown();
            this.lblOpacity = new System.Windows.Forms.Label();
            this.lblFont = new System.Windows.Forms.Label();
            this.lblChosenFont = new System.Windows.Forms.Label();
            this.btnChooseFont = new System.Windows.Forms.Button();
            this.grpBehavior = new System.Windows.Forms.GroupBox();
            this.lblPollEvery = new System.Windows.Forms.Label();
            this.numSeconds = new System.Windows.Forms.NumericUpDown();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.grpAppearance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOpacity)).BeginInit();
            this.grpBehavior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSeconds)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Location = new System.Drawing.Point(329, 181);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(248, 181);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // grpAppearance
            // 
            this.grpAppearance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAppearance.Controls.Add(this.btnChooseFont);
            this.grpAppearance.Controls.Add(this.lblChosenFont);
            this.grpAppearance.Controls.Add(this.lblFont);
            this.grpAppearance.Controls.Add(this.numOpacity);
            this.grpAppearance.Controls.Add(this.lblOpacity);
            this.grpAppearance.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.grpAppearance.ForeColor = System.Drawing.Color.Black;
            this.grpAppearance.Location = new System.Drawing.Point(12, 12);
            this.grpAppearance.Name = "grpAppearance";
            this.grpAppearance.Size = new System.Drawing.Size(407, 100);
            this.grpAppearance.TabIndex = 4;
            this.grpAppearance.TabStop = false;
            this.grpAppearance.Text = "Appearance";
            // 
            // numOpacity
            // 
            this.numOpacity.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numOpacity.Location = new System.Drawing.Point(118, 30);
            this.numOpacity.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numOpacity.Name = "numOpacity";
            this.numOpacity.Size = new System.Drawing.Size(53, 20);
            this.numOpacity.TabIndex = 3;
            this.numOpacity.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numOpacity.ValueChanged += new System.EventHandler(this.numOpacity_ValueChanged);
            // 
            // lblOpacity
            // 
            this.lblOpacity.AutoSize = true;
            this.lblOpacity.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOpacity.Location = new System.Drawing.Point(16, 32);
            this.lblOpacity.Name = "lblOpacity";
            this.lblOpacity.Size = new System.Drawing.Size(88, 13);
            this.lblOpacity.TabIndex = 2;
            this.lblOpacity.Text = "Window Opacity:";
            // 
            // lblFont
            // 
            this.lblFont.AutoSize = true;
            this.lblFont.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFont.Location = new System.Drawing.Point(16, 64);
            this.lblFont.Name = "lblFont";
            this.lblFont.Size = new System.Drawing.Size(31, 13);
            this.lblFont.TabIndex = 4;
            this.lblFont.Text = "Font:";
            // 
            // lblChosenFont
            // 
            this.lblChosenFont.AutoSize = true;
            this.lblChosenFont.Location = new System.Drawing.Point(115, 64);
            this.lblChosenFont.Name = "lblChosenFont";
            this.lblChosenFont.Size = new System.Drawing.Size(137, 13);
            this.lblChosenFont.TabIndex = 5;
            this.lblChosenFont.Text = "Microsoft Sans Serif, 8.25pt";
            // 
            // btnChooseFont
            // 
            this.btnChooseFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseFont.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChooseFont.Location = new System.Drawing.Point(317, 59);
            this.btnChooseFont.Name = "btnChooseFont";
            this.btnChooseFont.Size = new System.Drawing.Size(75, 23);
            this.btnChooseFont.TabIndex = 5;
            this.btnChooseFont.Text = "Change";
            this.btnChooseFont.UseVisualStyleBackColor = true;
            this.btnChooseFont.Click += new System.EventHandler(this.btnChooseFont_Click);
            // 
            // grpBehavior
            // 
            this.grpBehavior.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBehavior.Controls.Add(this.lblSeconds);
            this.grpBehavior.Controls.Add(this.numSeconds);
            this.grpBehavior.Controls.Add(this.lblPollEvery);
            this.grpBehavior.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.grpBehavior.ForeColor = System.Drawing.Color.Black;
            this.grpBehavior.Location = new System.Drawing.Point(12, 118);
            this.grpBehavior.Name = "grpBehavior";
            this.grpBehavior.Size = new System.Drawing.Size(407, 57);
            this.grpBehavior.TabIndex = 5;
            this.grpBehavior.TabStop = false;
            this.grpBehavior.Text = "Behavior";
            // 
            // lblPollEvery
            // 
            this.lblPollEvery.AutoSize = true;
            this.lblPollEvery.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPollEvery.Location = new System.Drawing.Point(16, 25);
            this.lblPollEvery.Name = "lblPollEvery";
            this.lblPollEvery.Size = new System.Drawing.Size(134, 13);
            this.lblPollEvery.TabIndex = 2;
            this.lblPollEvery.Text = "Refresh list of cases every ";
            // 
            // numSeconds
            // 
            this.numSeconds.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSeconds.Location = new System.Drawing.Point(156, 23);
            this.numSeconds.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numSeconds.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numSeconds.Name = "numSeconds";
            this.numSeconds.Size = new System.Drawing.Size(53, 20);
            this.numSeconds.TabIndex = 4;
            this.numSeconds.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSeconds.Location = new System.Drawing.Point(215, 25);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(47, 13);
            this.lblSeconds.TabIndex = 5;
            this.lblSeconds.Text = "seconds";
            // 
            // SettingsDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(432, 211);
            this.Controls.Add(this.grpBehavior);
            this.Controls.Add(this.grpAppearance);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDlg";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Case Tracker Settings";
            this.grpAppearance.ResumeLayout(false);
            this.grpAppearance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOpacity)).EndInit();
            this.grpBehavior.ResumeLayout(false);
            this.grpBehavior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSeconds)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpAppearance;
        private System.Windows.Forms.Label lblChosenFont;
        private System.Windows.Forms.Label lblFont;
        private System.Windows.Forms.NumericUpDown numOpacity;
        private System.Windows.Forms.Label lblOpacity;
        private System.Windows.Forms.Button btnChooseFont;
        private System.Windows.Forms.GroupBox grpBehavior;
        private System.Windows.Forms.Label lblSeconds;
        private System.Windows.Forms.NumericUpDown numSeconds;
        private System.Windows.Forms.Label lblPollEvery;
        private System.Windows.Forms.FontDialog fontDialog1;
    }
}