namespace UoFiddler.Controls.Forms
{
    partial class AITrainingExportForm
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
            this.assetTypeLabel = new System.Windows.Forms.Label();
            this.assetTypeComboBox = new System.Windows.Forms.ComboBox();
            this.targetSizeLabel = new System.Windows.Forms.Label();
            this.targetSizeComboBox = new System.Windows.Forms.ComboBox();
            this.backgroundLabel = new System.Windows.Forms.Label();
            this.backgroundComboBox = new System.Windows.Forms.ComboBox();
            this.captionLabel = new System.Windows.Forms.Label();
            this.captionTemplateTextBox = new System.Windows.Forms.TextBox();
            this.rangeGroupBox = new System.Windows.Forms.GroupBox();
            this.endIndexNumeric = new System.Windows.Forms.NumericUpDown();
            this.endIndexLabel = new System.Windows.Forms.Label();
            this.startIndexNumeric = new System.Windows.Forms.NumericUpDown();
            this.startIndexLabel = new System.Windows.Forms.Label();
            this.outputLabel = new System.Windows.Forms.Label();
            this.outputFolderTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.statusLabel = new System.Windows.Forms.Label();
            this.includeFlagsCheckBox = new System.Windows.Forms.CheckBox();
            this.captionHelpLabel = new System.Windows.Forms.Label();
            this.rangeGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endIndexNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startIndexNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // assetTypeLabel
            // 
            this.assetTypeLabel.AutoSize = true;
            this.assetTypeLabel.Location = new System.Drawing.Point(12, 15);
            this.assetTypeLabel.Name = "assetTypeLabel";
            this.assetTypeLabel.Size = new System.Drawing.Size(64, 15);
            this.assetTypeLabel.TabIndex = 0;
            this.assetTypeLabel.Text = "Asset Type:";
            // 
            // assetTypeComboBox
            // 
            this.assetTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.assetTypeComboBox.FormattingEnabled = true;
            this.assetTypeComboBox.Location = new System.Drawing.Point(120, 12);
            this.assetTypeComboBox.Name = "assetTypeComboBox";
            this.assetTypeComboBox.Size = new System.Drawing.Size(200, 23);
            this.assetTypeComboBox.TabIndex = 1;
            this.assetTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.assetTypeComboBox_SelectedIndexChanged);
            // 
            // targetSizeLabel
            // 
            this.targetSizeLabel.AutoSize = true;
            this.targetSizeLabel.Location = new System.Drawing.Point(12, 44);
            this.targetSizeLabel.Name = "targetSizeLabel";
            this.targetSizeLabel.Size = new System.Drawing.Size(65, 15);
            this.targetSizeLabel.TabIndex = 2;
            this.targetSizeLabel.Text = "Target Size:";
            // 
            // targetSizeComboBox
            // 
            this.targetSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.targetSizeComboBox.FormattingEnabled = true;
            this.targetSizeComboBox.Location = new System.Drawing.Point(120, 41);
            this.targetSizeComboBox.Name = "targetSizeComboBox";
            this.targetSizeComboBox.Size = new System.Drawing.Size(200, 23);
            this.targetSizeComboBox.TabIndex = 3;
            // 
            // backgroundLabel
            // 
            this.backgroundLabel.AutoSize = true;
            this.backgroundLabel.Location = new System.Drawing.Point(12, 73);
            this.backgroundLabel.Name = "backgroundLabel";
            this.backgroundLabel.Size = new System.Drawing.Size(74, 15);
            this.backgroundLabel.TabIndex = 4;
            this.backgroundLabel.Text = "Background:";
            // 
            // backgroundComboBox
            // 
            this.backgroundComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.backgroundComboBox.FormattingEnabled = true;
            this.backgroundComboBox.Location = new System.Drawing.Point(120, 70);
            this.backgroundComboBox.Name = "backgroundComboBox";
            this.backgroundComboBox.Size = new System.Drawing.Size(200, 23);
            this.backgroundComboBox.TabIndex = 5;
            // 
            // captionLabel
            // 
            this.captionLabel.AutoSize = true;
            this.captionLabel.Location = new System.Drawing.Point(12, 102);
            this.captionLabel.Name = "captionLabel";
            this.captionLabel.Size = new System.Drawing.Size(103, 15);
            this.captionLabel.TabIndex = 6;
            this.captionLabel.Text = "Caption Template:";
            // 
            // captionTemplateTextBox
            // 
            this.captionTemplateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.captionTemplateTextBox.Location = new System.Drawing.Point(120, 99);
            this.captionTemplateTextBox.Name = "captionTemplateTextBox";
            this.captionTemplateTextBox.Size = new System.Drawing.Size(352, 23);
            this.captionTemplateTextBox.TabIndex = 7;
            // 
            // rangeGroupBox
            // 
            this.rangeGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rangeGroupBox.Controls.Add(this.endIndexNumeric);
            this.rangeGroupBox.Controls.Add(this.endIndexLabel);
            this.rangeGroupBox.Controls.Add(this.startIndexNumeric);
            this.rangeGroupBox.Controls.Add(this.startIndexLabel);
            this.rangeGroupBox.Location = new System.Drawing.Point(12, 172);
            this.rangeGroupBox.Name = "rangeGroupBox";
            this.rangeGroupBox.Size = new System.Drawing.Size(460, 60);
            this.rangeGroupBox.TabIndex = 8;
            this.rangeGroupBox.TabStop = false;
            this.rangeGroupBox.Text = "Index Range";
            // 
            // endIndexNumeric
            // 
            this.endIndexNumeric.Location = new System.Drawing.Point(290, 25);
            this.endIndexNumeric.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.endIndexNumeric.Name = "endIndexNumeric";
            this.endIndexNumeric.Size = new System.Drawing.Size(120, 23);
            this.endIndexNumeric.TabIndex = 3;
            // 
            // endIndexLabel
            // 
            this.endIndexLabel.AutoSize = true;
            this.endIndexLabel.Location = new System.Drawing.Point(230, 27);
            this.endIndexLabel.Name = "endIndexLabel";
            this.endIndexLabel.Size = new System.Drawing.Size(54, 15);
            this.endIndexLabel.TabIndex = 2;
            this.endIndexLabel.Text = "End (hex):";
            // 
            // startIndexNumeric
            // 
            this.startIndexNumeric.Location = new System.Drawing.Point(90, 25);
            this.startIndexNumeric.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.startIndexNumeric.Name = "startIndexNumeric";
            this.startIndexNumeric.Size = new System.Drawing.Size(120, 23);
            this.startIndexNumeric.TabIndex = 1;
            // 
            // startIndexLabel
            // 
            this.startIndexLabel.AutoSize = true;
            this.startIndexLabel.Location = new System.Drawing.Point(15, 27);
            this.startIndexLabel.Name = "startIndexLabel";
            this.startIndexLabel.Size = new System.Drawing.Size(60, 15);
            this.startIndexLabel.TabIndex = 0;
            this.startIndexLabel.Text = "Start (hex):";
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Location = new System.Drawing.Point(12, 247);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(82, 15);
            this.outputLabel.TabIndex = 9;
            this.outputLabel.Text = "Output Folder:";
            // 
            // outputFolderTextBox
            // 
            this.outputFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputFolderTextBox.Location = new System.Drawing.Point(120, 244);
            this.outputFolderTextBox.Name = "outputFolderTextBox";
            this.outputFolderTextBox.Size = new System.Drawing.Size(271, 23);
            this.outputFolderTextBox.TabIndex = 10;
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(397, 243);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 25);
            this.browseButton.TabIndex = 11;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exportButton.Location = new System.Drawing.Point(316, 320);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(75, 28);
            this.exportButton.TabIndex = 12;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(397, 320);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 28);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 286);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(460, 23);
            this.progressBar.TabIndex = 14;
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 326);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(42, 15);
            this.statusLabel.TabIndex = 15;
            this.statusLabel.Text = "Ready.";
            // 
            // includeFlagsCheckBox
            // 
            this.includeFlagsCheckBox.AutoSize = true;
            this.includeFlagsCheckBox.Location = new System.Drawing.Point(120, 147);
            this.includeFlagsCheckBox.Name = "includeFlagsCheckBox";
            this.includeFlagsCheckBox.Size = new System.Drawing.Size(170, 19);
            this.includeFlagsCheckBox.TabIndex = 16;
            this.includeFlagsCheckBox.Text = "Include flags in caption";
            this.includeFlagsCheckBox.UseVisualStyleBackColor = true;
            // 
            // captionHelpLabel
            // 
            this.captionHelpLabel.AutoSize = true;
            this.captionHelpLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.captionHelpLabel.Location = new System.Drawing.Point(120, 125);
            this.captionHelpLabel.Name = "captionHelpLabel";
            this.captionHelpLabel.Size = new System.Drawing.Size(248, 15);
            this.captionHelpLabel.TabIndex = 17;
            this.captionHelpLabel.Text = "Placeholders: {name}, {index}, {flags}";
            // 
            // AITrainingExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 361);
            this.Controls.Add(this.captionHelpLabel);
            this.Controls.Add(this.includeFlagsCheckBox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.outputFolderTextBox);
            this.Controls.Add(this.outputLabel);
            this.Controls.Add(this.rangeGroupBox);
            this.Controls.Add(this.captionTemplateTextBox);
            this.Controls.Add(this.captionLabel);
            this.Controls.Add(this.backgroundComboBox);
            this.Controls.Add(this.backgroundLabel);
            this.Controls.Add(this.targetSizeComboBox);
            this.Controls.Add(this.targetSizeLabel);
            this.Controls.Add(this.assetTypeComboBox);
            this.Controls.Add(this.assetTypeLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AITrainingExportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AI Training Export";
            this.rangeGroupBox.ResumeLayout(false);
            this.rangeGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endIndexNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startIndexNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label assetTypeLabel;
        private System.Windows.Forms.ComboBox assetTypeComboBox;
        private System.Windows.Forms.Label targetSizeLabel;
        private System.Windows.Forms.ComboBox targetSizeComboBox;
        private System.Windows.Forms.Label backgroundLabel;
        private System.Windows.Forms.ComboBox backgroundComboBox;
        private System.Windows.Forms.Label captionLabel;
        private System.Windows.Forms.TextBox captionTemplateTextBox;
        private System.Windows.Forms.GroupBox rangeGroupBox;
        private System.Windows.Forms.NumericUpDown endIndexNumeric;
        private System.Windows.Forms.Label endIndexLabel;
        private System.Windows.Forms.NumericUpDown startIndexNumeric;
        private System.Windows.Forms.Label startIndexLabel;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.TextBox outputFolderTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.CheckBox includeFlagsCheckBox;
        private System.Windows.Forms.Label captionHelpLabel;
    }
}
