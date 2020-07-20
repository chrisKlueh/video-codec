namespace Codec
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.inputSizeLabel = new System.Windows.Forms.Label();
            this.outputSizeLabel = new System.Windows.Forms.Label();
            this.timeBar = new System.Windows.Forms.TrackBar();
            this.convertButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.Label();
            this.keyFrameInput = new System.Windows.Forms.TextBox();
            this.keyFrameLabel1 = new System.Windows.Forms.Label();
            this.keyFrameLabel2 = new System.Windows.Forms.Label();
            this.keyFrameSaveButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.outputPictureBox = new System.Windows.Forms.PictureBox();
            this.inputPictureBox = new System.Windows.Forms.PictureBox();
            this.ColorSubSamplingLabel = new System.Windows.Forms.Label();
            this.inputCheckBox = new System.Windows.Forms.CheckBox();
            this.outputCheckBox = new System.Windows.Forms.CheckBox();
            this.multiThreadInput = new System.Windows.Forms.NumericUpDown();
            this.multiThreadLabel = new System.Windows.Forms.Label();
            this.multiThreadSaveButton = new System.Windows.Forms.Button();
            this.qualityLabel = new System.Windows.Forms.Label();
            this.qualityInput = new System.Windows.Forms.NumericUpDown();
            this.qualitySaveButton = new System.Windows.Forms.Button();
            this.frameInput = new System.Windows.Forms.NumericUpDown();
            this.frameLimiter = new System.Windows.Forms.CheckBox();
            this.chromaBox = new System.Windows.Forms.CheckedListBox();
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.multiThreadInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.qualityInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameInput)).BeginInit();
            this.SuspendLayout();
            // 
            // inputSizeLabel
            // 
            this.inputSizeLabel.AutoSize = true;
            this.inputSizeLabel.Location = new System.Drawing.Point(9, 255);
            this.inputSizeLabel.Name = "inputSizeLabel";
            this.inputSizeLabel.Size = new System.Drawing.Size(77, 13);
            this.inputSizeLabel.TabIndex = 1;
            this.inputSizeLabel.Text = "Input file size: -";
            // 
            // outputSizeLabel
            // 
            this.outputSizeLabel.AutoSize = true;
            this.outputSizeLabel.Location = new System.Drawing.Point(335, 255);
            this.outputSizeLabel.Name = "outputSizeLabel";
            this.outputSizeLabel.Size = new System.Drawing.Size(85, 13);
            this.outputSizeLabel.TabIndex = 3;
            this.outputSizeLabel.Text = "Output file size: -";
            // 
            // timeBar
            // 
            this.timeBar.LargeChange = 30;
            this.timeBar.Location = new System.Drawing.Point(15, 333);
            this.timeBar.Margin = new System.Windows.Forms.Padding(0);
            this.timeBar.Maximum = 299;
            this.timeBar.Name = "timeBar";
            this.timeBar.Size = new System.Drawing.Size(646, 45);
            this.timeBar.SmallChange = 30;
            this.timeBar.TabIndex = 4;
            this.timeBar.TickFrequency = 30;
            this.timeBar.ValueChanged += new System.EventHandler(this.timeBar_ValueChanged);
            // 
            // convertButton
            // 
            this.convertButton.Location = new System.Drawing.Point(252, 500);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(152, 33);
            this.convertButton.TabIndex = 5;
            this.convertButton.Text = "Convert video";
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 562);
            this.progressBar.Maximum = 300;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(646, 23);
            this.progressBar.TabIndex = 6;
            this.progressBar.Visible = false;
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(12, 543);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(53, 13);
            this.progressLabel.TabIndex = 7;
            this.progressLabel.Text = "working...";
            this.progressLabel.Visible = false;
            // 
            // keyFrameInput
            // 
            this.keyFrameInput.Location = new System.Drawing.Point(108, 421);
            this.keyFrameInput.Name = "keyFrameInput";
            this.keyFrameInput.Size = new System.Drawing.Size(33, 20);
            this.keyFrameInput.TabIndex = 8;
            this.keyFrameInput.Text = "30";
            this.keyFrameInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // keyFrameLabel1
            // 
            this.keyFrameLabel1.AutoSize = true;
            this.keyFrameLabel1.Location = new System.Drawing.Point(19, 424);
            this.keyFrameLabel1.Name = "keyFrameLabel1";
            this.keyFrameLabel1.Size = new System.Drawing.Size(83, 13);
            this.keyFrameLabel1.TabIndex = 9;
            this.keyFrameLabel1.Text = "Key frame every";
            // 
            // keyFrameLabel2
            // 
            this.keyFrameLabel2.AutoSize = true;
            this.keyFrameLabel2.Location = new System.Drawing.Point(147, 424);
            this.keyFrameLabel2.Name = "keyFrameLabel2";
            this.keyFrameLabel2.Size = new System.Drawing.Size(38, 13);
            this.keyFrameLabel2.TabIndex = 10;
            this.keyFrameLabel2.Text = "frames";
            // 
            // keyFrameSaveButton
            // 
            this.keyFrameSaveButton.Location = new System.Drawing.Point(191, 419);
            this.keyFrameSaveButton.Name = "keyFrameSaveButton";
            this.keyFrameSaveButton.Size = new System.Drawing.Size(75, 23);
            this.keyFrameSaveButton.TabIndex = 11;
            this.keyFrameSaveButton.Text = "save";
            this.keyFrameSaveButton.UseVisualStyleBackColor = true;
            this.keyFrameSaveButton.Click += new System.EventHandler(this.keyFrameSaveButton_Click);
            // 
            // playButton
            // 
            this.playButton.Image = global::Codec.Properties.Resources.playButton1;
            this.playButton.Location = new System.Drawing.Point(261, 280);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(143, 50);
            this.playButton.TabIndex = 12;
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // outputPictureBox
            // 
            this.outputPictureBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.outputPictureBox.BackgroundImage = global::Codec.Properties.Resources.outputDefault;
            this.outputPictureBox.Location = new System.Drawing.Point(338, 12);
            this.outputPictureBox.Name = "outputPictureBox";
            this.outputPictureBox.Size = new System.Drawing.Size(320, 240);
            this.outputPictureBox.TabIndex = 2;
            this.outputPictureBox.TabStop = false;
            // 
            // inputPictureBox
            // 
            this.inputPictureBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.inputPictureBox.BackgroundImage = global::Codec.Properties.Resources.inputDefault;
            this.inputPictureBox.Location = new System.Drawing.Point(12, 12);
            this.inputPictureBox.Name = "inputPictureBox";
            this.inputPictureBox.Size = new System.Drawing.Size(320, 240);
            this.inputPictureBox.TabIndex = 0;
            this.inputPictureBox.TabStop = false;
            this.inputPictureBox.Click += new System.EventHandler(this.inputPictureBox_Click);
            // 
            // ColorSubSamplingLabel
            // 
            this.ColorSubSamplingLabel.AutoSize = true;
            this.ColorSubSamplingLabel.Location = new System.Drawing.Point(19, 454);
            this.ColorSubSamplingLabel.Name = "ColorSubSamplingLabel";
            this.ColorSubSamplingLabel.Size = new System.Drawing.Size(92, 13);
            this.ColorSubSamplingLabel.TabIndex = 13;
            this.ColorSubSamplingLabel.Text = "Color subsampling";
            // 
            // inputCheckBox
            // 
            this.inputCheckBox.AutoSize = true;
            this.inputCheckBox.Checked = true;
            this.inputCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.inputCheckBox.Location = new System.Drawing.Point(181, 284);
            this.inputCheckBox.Name = "inputCheckBox";
            this.inputCheckBox.Size = new System.Drawing.Size(72, 17);
            this.inputCheckBox.TabIndex = 17;
            this.inputCheckBox.Text = "Play input";
            this.inputCheckBox.UseVisualStyleBackColor = true;
            // 
            // outputCheckBox
            // 
            this.outputCheckBox.AutoSize = true;
            this.outputCheckBox.Location = new System.Drawing.Point(181, 308);
            this.outputCheckBox.Name = "outputCheckBox";
            this.outputCheckBox.Size = new System.Drawing.Size(79, 17);
            this.outputCheckBox.TabIndex = 18;
            this.outputCheckBox.Text = "Play output";
            this.outputCheckBox.UseVisualStyleBackColor = true;
            // 
            // multiThreadInput
            // 
            this.multiThreadInput.Location = new System.Drawing.Point(607, 401);
            this.multiThreadInput.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.multiThreadInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.multiThreadInput.Name = "multiThreadInput";
            this.multiThreadInput.Size = new System.Drawing.Size(51, 20);
            this.multiThreadInput.TabIndex = 19;
            this.multiThreadInput.Tag = "";
            this.multiThreadInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.multiThreadInput.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // multiThreadLabel
            // 
            this.multiThreadLabel.AutoSize = true;
            this.multiThreadLabel.Location = new System.Drawing.Point(555, 404);
            this.multiThreadLabel.Name = "multiThreadLabel";
            this.multiThreadLabel.Size = new System.Drawing.Size(46, 13);
            this.multiThreadLabel.TabIndex = 20;
            this.multiThreadLabel.Text = "Threads";
            // 
            // multiThreadSaveButton
            // 
            this.multiThreadSaveButton.Location = new System.Drawing.Point(583, 424);
            this.multiThreadSaveButton.Name = "multiThreadSaveButton";
            this.multiThreadSaveButton.Size = new System.Drawing.Size(75, 23);
            this.multiThreadSaveButton.TabIndex = 21;
            this.multiThreadSaveButton.Text = "save";
            this.multiThreadSaveButton.UseVisualStyleBackColor = true;
            this.multiThreadSaveButton.Click += new System.EventHandler(this.multiThreadSaveButton_Click);
            // 
            // qualityLabel
            // 
            this.qualityLabel.AutoSize = true;
            this.qualityLabel.Location = new System.Drawing.Point(19, 392);
            this.qualityLabel.Name = "qualityLabel";
            this.qualityLabel.Size = new System.Drawing.Size(39, 13);
            this.qualityLabel.TabIndex = 23;
            this.qualityLabel.Text = "Quality";
            // 
            // qualityInput
            // 
            this.qualityInput.Location = new System.Drawing.Point(60, 388);
            this.qualityInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.qualityInput.Name = "qualityInput";
            this.qualityInput.Size = new System.Drawing.Size(51, 20);
            this.qualityInput.TabIndex = 22;
            this.qualityInput.Tag = "";
            this.qualityInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.qualityInput.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // qualitySaveButton
            // 
            this.qualitySaveButton.Location = new System.Drawing.Point(117, 386);
            this.qualitySaveButton.Name = "qualitySaveButton";
            this.qualitySaveButton.Size = new System.Drawing.Size(75, 23);
            this.qualitySaveButton.TabIndex = 24;
            this.qualitySaveButton.Text = "save";
            this.qualitySaveButton.UseVisualStyleBackColor = true;
            this.qualitySaveButton.Click += new System.EventHandler(this.qualitySaveButton_Click);
            // 
            // frameInput
            // 
            this.frameInput.Location = new System.Drawing.Point(410, 306);
            this.frameInput.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.frameInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.frameInput.Name = "frameInput";
            this.frameInput.Size = new System.Drawing.Size(51, 20);
            this.frameInput.TabIndex = 25;
            this.frameInput.Tag = "";
            this.frameInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.frameInput.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // frameLimiter
            // 
            this.frameLimiter.AutoSize = true;
            this.frameLimiter.Checked = true;
            this.frameLimiter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.frameLimiter.Location = new System.Drawing.Point(410, 286);
            this.frameLimiter.Name = "frameLimiter";
            this.frameLimiter.Size = new System.Drawing.Size(131, 17);
            this.frameLimiter.TabIndex = 26;
            this.frameLimiter.Text = "Limit number of frames";
            this.frameLimiter.UseVisualStyleBackColor = true;
            // 
            // chromaBox
            // 
            this.chromaBox.CheckOnClick = true;
            this.chromaBox.FormattingEnabled = true;
            this.chromaBox.Items.AddRange(new object[] {
            "4:4:4",
            "4:2:2",
            "4:2:0"});
            this.chromaBox.Location = new System.Drawing.Point(117, 454);
            this.chromaBox.Name = "chromaBox";
            this.chromaBox.Size = new System.Drawing.Size(53, 49);
            this.chromaBox.TabIndex = 27;
            this.chromaBox.SelectedIndexChanged += new System.EventHandler(this.chromaBox_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 591);
            this.Controls.Add(this.chromaBox);
            this.Controls.Add(this.frameLimiter);
            this.Controls.Add(this.frameInput);
            this.Controls.Add(this.qualitySaveButton);
            this.Controls.Add(this.qualityLabel);
            this.Controls.Add(this.qualityInput);
            this.Controls.Add(this.multiThreadSaveButton);
            this.Controls.Add(this.multiThreadLabel);
            this.Controls.Add(this.multiThreadInput);
            this.Controls.Add(this.outputCheckBox);
            this.Controls.Add(this.inputCheckBox);
            this.Controls.Add(this.ColorSubSamplingLabel);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.keyFrameSaveButton);
            this.Controls.Add(this.keyFrameLabel2);
            this.Controls.Add(this.keyFrameLabel1);
            this.Controls.Add(this.keyFrameInput);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.convertButton);
            this.Controls.Add(this.timeBar);
            this.Controls.Add(this.outputSizeLabel);
            this.Controls.Add(this.outputPictureBox);
            this.Controls.Add(this.inputSizeLabel);
            this.Controls.Add(this.inputPictureBox);
            this.Name = "Form1";
            this.Text = "Codec Toolbox";
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.multiThreadInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.qualityInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameInput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox inputPictureBox;
        private System.Windows.Forms.Label inputSizeLabel;
        private System.Windows.Forms.PictureBox outputPictureBox;
        private System.Windows.Forms.Label outputSizeLabel;
        private System.Windows.Forms.TrackBar timeBar;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.TextBox keyFrameInput;
        private System.Windows.Forms.Label keyFrameLabel1;
        private System.Windows.Forms.Label keyFrameLabel2;
        private System.Windows.Forms.Button keyFrameSaveButton;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Label ColorSubSamplingLabel;
        private System.Windows.Forms.CheckBox inputCheckBox;
        private System.Windows.Forms.CheckBox outputCheckBox;
        private System.Windows.Forms.NumericUpDown multiThreadInput;
        private System.Windows.Forms.Label multiThreadLabel;
        private System.Windows.Forms.Button multiThreadSaveButton;
        private System.Windows.Forms.Label qualityLabel;
        private System.Windows.Forms.NumericUpDown qualityInput;
        private System.Windows.Forms.Button qualitySaveButton;
        private System.Windows.Forms.NumericUpDown frameInput;
        private System.Windows.Forms.CheckBox frameLimiter;
        private System.Windows.Forms.CheckedListBox chromaBox;
    }
}

