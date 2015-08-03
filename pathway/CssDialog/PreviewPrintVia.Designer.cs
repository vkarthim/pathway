﻿namespace SIL.PublishingSolution
{
    partial class PreviewPrintVia
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
			this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewPrintVia));
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
            this.BtnOk = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnHelp = new System.Windows.Forms.Button();
            this.lblPreview = new System.Windows.Forms.Label();
            this.grid = new System.Windows.Forms.DataGridView();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Pathway";
			this.l10NSharpExtender1.PrefixForNewItems = "PreviewPrintVia";
            // 
            // BtnOk
            // 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.BtnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.BtnOk, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.BtnOk, L10NSharp.LocalizationPriority.High);
			this.l10NSharpExtender1.SetLocalizingId(this.BtnOk, "PreviewPrintVia.btnOK");
            this.BtnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOk.Location = new System.Drawing.Point(535, 518);
			this.BtnOk.Name = "BtnOk";
            this.BtnOk.Size = new System.Drawing.Size(75, 23);
            this.BtnOk.TabIndex = 2;
            this.BtnOk.Text = "OK";
            this.BtnOk.UseVisualStyleBackColor = true;
            this.BtnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // BtnCancel
            // 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.BtnCancel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.BtnCancel, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.BtnCancel, L10NSharp.LocalizationPriority.High);
			this.l10NSharpExtender1.SetLocalizingId(this.BtnCancel, "PreviewPrintVia.BtnCancel");
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.Location = new System.Drawing.Point(616, 518);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 3;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // BtnHelp
            // 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.BtnHelp, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.BtnHelp, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.BtnHelp, L10NSharp.LocalizationPriority.High);
			this.l10NSharpExtender1.SetLocalizingId(this.BtnHelp, "PreviewPrintVia.BtnHelp");
            this.BtnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnHelp.Location = new System.Drawing.Point(697, 518);
            this.BtnHelp.Name = "BtnHelp";
            this.BtnHelp.Size = new System.Drawing.Size(75, 23);
            this.BtnHelp.TabIndex = 4;
            this.BtnHelp.Text = "Help";
            this.BtnHelp.UseVisualStyleBackColor = true;
            this.BtnHelp.Click += new System.EventHandler(this.BtnHelp_Click);
            // 
            // lblPreview
            // 
            this.lblPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPreview.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblPreview, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblPreview, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.lblPreview, L10NSharp.LocalizationPriority.High);
			this.l10NSharpExtender1.SetLocalizingId(this.lblPreview, "PreviewPrintVia.lblPreview");
            this.lblPreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPreview.Location = new System.Drawing.Point(414, 21);
            this.lblPreview.Name = "lblPreview";
			this.lblPreview.Size = new System.Drawing.Size(157, 13);
            this.lblPreview.TabIndex = 6;
            //this.lblPreview.Text = "Sample data in this layout:";
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AllowUserToResizeRows = false;
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grid.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Location = new System.Drawing.Point(11, 15);
            this.grid.MultiSelect = false;
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.RowHeadersVisible = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.Size = new System.Drawing.Size(393, 497);
            this.grid.TabIndex = 7;
            this.grid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellClick);
            this.grid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellContentClick);
            this.grid.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_RowEnter);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(414, 44);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(358, 468);
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Enabled = false;
            this.btnNext.Location = new System.Drawing.Point(747, 15);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(25, 23);
            this.btnNext.TabIndex = 9;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrevious.Enabled = false;
            this.btnPrevious.Location = new System.Drawing.Point(720, 15);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(25, 23);
            this.btnPrevious.TabIndex = 10;
            this.btnPrevious.Text = "<";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnEdit
            // 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnEdit, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnEdit, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.btnEdit, L10NSharp.LocalizationPriority.High);
			this.l10NSharpExtender1.SetLocalizingId(this.btnEdit, "PreviewPrintVia.btnEdit");
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEdit.Location = new System.Drawing.Point(12, 518);
			this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 13;
            this.btnEdit.Text = "Edit Layout";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // PreviewPrintVia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 549);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.BtnHelp);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "PreviewPrintVia.WindowTitle");
            this.Name = "PreviewPrintVia";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Layout";
            this.Load += new System.EventHandler(this.PreviewPrintVia_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PreviewPrintVia_KeyUp);
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
        private System.Windows.Forms.Button BtnOk;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnHelp;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnEdit;
    }
}