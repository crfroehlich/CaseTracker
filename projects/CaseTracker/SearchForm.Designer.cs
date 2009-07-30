namespace FogBugzCaseTracker
{
    partial class SearchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SearchFilterBox = new System.Windows.Forms.GroupBox();
            this.lnkSearchHelp = new System.Windows.Forms.LinkLabel();
            this.lblNarrowSearch = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtBaseSearch = new System.Windows.Forms.TextBox();
            this.lblBaseSearch = new System.Windows.Forms.Label();
            this.txtNarrowSearch = new System.Windows.Forms.TextBox();
            this.SearchResultBox = new System.Windows.Forms.GroupBox();
            this.listTestResults = new System.Windows.Forms.ListBox();
            this.chkIgnoreBaseSearch = new System.Windows.Forms.CheckBox();
            this.SearchFilterBox.SuspendLayout();
            this.SearchResultBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(540, 429);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(621, 429);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // SearchFilterBox
            // 
            this.SearchFilterBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchFilterBox.Controls.Add(this.chkIgnoreBaseSearch);
            this.SearchFilterBox.Controls.Add(this.lnkSearchHelp);
            this.SearchFilterBox.Controls.Add(this.lblNarrowSearch);
            this.SearchFilterBox.Controls.Add(this.btnTest);
            this.SearchFilterBox.Controls.Add(this.txtBaseSearch);
            this.SearchFilterBox.Controls.Add(this.lblBaseSearch);
            this.SearchFilterBox.Controls.Add(this.txtNarrowSearch);
            this.SearchFilterBox.Location = new System.Drawing.Point(12, 12);
            this.SearchFilterBox.Name = "SearchFilterBox";
            this.SearchFilterBox.Size = new System.Drawing.Size(684, 96);
            this.SearchFilterBox.TabIndex = 0;
            this.SearchFilterBox.TabStop = false;
            this.SearchFilterBox.Text = "Search Filter";
            // 
            // lnkSearchHelp
            // 
            this.lnkSearchHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkSearchHelp.AutoSize = true;
            this.lnkSearchHelp.Location = new System.Drawing.Point(429, 74);
            this.lnkSearchHelp.Name = "lnkSearchHelp";
            this.lnkSearchHelp.Size = new System.Drawing.Size(158, 13);
            this.lnkSearchHelp.TabIndex = 6;
            this.lnkSearchHelp.TabStop = true;
            this.lnkSearchHelp.Text = "what\'s the search syntax again?";
            this.lnkSearchHelp.Click += new System.EventHandler(this.lnkSearchHelp_Click);
            // 
            // lblNarrowSearch
            // 
            this.lblNarrowSearch.AutoSize = true;
            this.lblNarrowSearch.Location = new System.Drawing.Point(17, 54);
            this.lblNarrowSearch.Name = "lblNarrowSearch";
            this.lblNarrowSearch.Size = new System.Drawing.Size(81, 13);
            this.lblNarrowSearch.TabIndex = 5;
            this.lblNarrowSearch.Text = "Narrow Search:";
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(591, 49);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtBaseSearch
            // 
            this.txtBaseSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBaseSearch.Location = new System.Drawing.Point(203, 22);
            this.txtBaseSearch.Name = "txtBaseSearch";
            this.txtBaseSearch.ReadOnly = true;
            this.txtBaseSearch.Size = new System.Drawing.Size(382, 20);
            this.txtBaseSearch.TabIndex = 4;
            this.txtBaseSearch.TabStop = false;
            this.txtBaseSearch.Text = "AssignedTo:\"Me\" AND Status:\"Active\" AND -EstimateCurrent:\"0\"";
            // 
            // lblBaseSearch
            // 
            this.lblBaseSearch.AutoSize = true;
            this.lblBaseSearch.Location = new System.Drawing.Point(17, 25);
            this.lblBaseSearch.Name = "lblBaseSearch";
            this.lblBaseSearch.Size = new System.Drawing.Size(180, 13);
            this.lblBaseSearch.TabIndex = 2;
            this.lblBaseSearch.Text = "Base Search (mandatory conditions):";
            // 
            // txtNarrowSearch
            // 
            this.txtNarrowSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNarrowSearch.Location = new System.Drawing.Point(104, 51);
            this.txtNarrowSearch.Name = "txtNarrowSearch";
            this.txtNarrowSearch.Size = new System.Drawing.Size(481, 20);
            this.txtNarrowSearch.TabIndex = 0;
            this.txtNarrowSearch.Text = "AND ";
            this.txtNarrowSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // SearchResultBox
            // 
            this.SearchResultBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchResultBox.Controls.Add(this.listTestResults);
            this.SearchResultBox.Location = new System.Drawing.Point(12, 114);
            this.SearchResultBox.Name = "SearchResultBox";
            this.SearchResultBox.Size = new System.Drawing.Size(684, 309);
            this.SearchResultBox.TabIndex = 3;
            this.SearchResultBox.TabStop = false;
            this.SearchResultBox.Text = "Test Search Results";
            // 
            // listTestResults
            // 
            this.listTestResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listTestResults.DisplayMember = "LongDescription";
            this.listTestResults.FormattingEnabled = true;
            this.listTestResults.Location = new System.Drawing.Point(20, 28);
            this.listTestResults.Name = "listTestResults";
            this.listTestResults.Size = new System.Drawing.Size(646, 251);
            this.listTestResults.TabIndex = 2;
            // 
            // chkIgnoreBaseSearch
            // 
            this.chkIgnoreBaseSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkIgnoreBaseSearch.AutoSize = true;
            this.chkIgnoreBaseSearch.Location = new System.Drawing.Point(591, 24);
            this.chkIgnoreBaseSearch.Name = "chkIgnoreBaseSearch";
            this.chkIgnoreBaseSearch.Size = new System.Drawing.Size(56, 17);
            this.chkIgnoreBaseSearch.TabIndex = 7;
            this.chkIgnoreBaseSearch.Text = "Ignore";
            this.chkIgnoreBaseSearch.UseVisualStyleBackColor = true;
            this.chkIgnoreBaseSearch.CheckedChanged += new System.EventHandler(this.chkIgnoreBaseSearch_CheckedChanged);
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 462);
            this.Controls.Add(this.SearchResultBox);
            this.Controls.Add(this.SearchFilterBox);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(574, 262);
            this.Name = "SearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search Filter";
            this.SearchFilterBox.ResumeLayout(false);
            this.SearchFilterBox.PerformLayout();
            this.SearchResultBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox SearchFilterBox;
        private System.Windows.Forms.TextBox txtNarrowSearch;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label lblBaseSearch;
        private System.Windows.Forms.Label lblNarrowSearch;
        private System.Windows.Forms.TextBox txtBaseSearch;
        private System.Windows.Forms.GroupBox SearchResultBox;
        private System.Windows.Forms.ListBox listTestResults;
        private System.Windows.Forms.LinkLabel lnkSearchHelp;
        private System.Windows.Forms.CheckBox chkIgnoreBaseSearch;
    }
}