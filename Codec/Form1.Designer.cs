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
            this.colorAinput = new System.Windows.Forms.TextBox();
            this.colorBinput = new System.Windows.Forms.TextBox();
            this.colorCinput = new System.Windows.Forms.TextBox();
            this.inputCheckBox = new System.Windows.Forms.CheckBox();
            this.outputCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputPictureBox)).BeginInit();
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
            this.keyFrameInput.Location = new System.Drawing.Point(101, 426);
            this.keyFrameInput.Name = "keyFrameInput";
            this.keyFrameInput.Size = new System.Drawing.Size(33, 20);
            this.keyFrameInput.TabIndex = 8;
            this.keyFrameInput.Text = "30";
            this.keyFrameInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // keyFrameLabel1
            // 
            this.keyFrameLabel1.AutoSize = true;
            this.keyFrameLabel1.Location = new System.Drawing.Point(12, 429);
            this.keyFrameLabel1.Name = "keyFrameLabel1";
            this.keyFrameLabel1.Size = new System.Drawing.Size(83, 13);
            this.keyFrameLabel1.TabIndex = 9;
            this.keyFrameLabel1.Text = "Key frame every";
            // 
            // keyFrameLabel2
            // 
            this.keyFrameLabel2.AutoSize = true;
            this.keyFrameLabel2.Location = new System.Drawing.Point(140, 429);
            this.keyFrameLabel2.Name = "keyFrameLabel2";
            this.keyFrameLabel2.Size = new System.Drawing.Size(38, 13);
            this.keyFrameLabel2.TabIndex = 10;
            this.keyFrameLabel2.Text = "frames";
            // 
            // keyFrameSaveButton
            // 
            this.keyFrameSaveButton.Location = new System.Drawing.Point(184, 424);
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
            this.playButton.Location = new System.Drawing.Point(302, 280);
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
            this.ColorSubSamplingLabel.Location = new System.Drawing.Point(12, 404);
            this.ColorSubSamplingLabel.Name = "ColorSubSamplingLabel";
            this.ColorSubSamplingLabel.Size = new System.Drawing.Size(92, 13);
            this.ColorSubSamplingLabel.TabIndex = 13;
            this.ColorSubSamplingLabel.Text = "Color subsampling";
            // 
            // colorAinput
            // 
            this.colorAinput.Location = new System.Drawing.Point(110, 401);
            this.colorAinput.Name = "colorAinput";
            this.colorAinput.Size = new System.Drawing.Size(24, 20);
            this.colorAinput.TabIndex = 14;
            this.colorAinput.Text = "4";
            this.colorAinput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // colorBinput
            // 
            this.colorBinput.Location = new System.Drawing.Point(140, 401);
            this.colorBinput.Name = "colorBinput";
            this.colorBinput.Size = new System.Drawing.Size(24, 20);
            this.colorBinput.TabIndex = 15;
            this.colorBinput.Text = "2";
            this.colorBinput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // colorCinput
            // 
            this.colorCinput.Location = new System.Drawing.Point(170, 401);
            this.colorCinput.Name = "colorCinput";
            this.colorCinput.Size = new System.Drawing.Size(24, 20);
            this.colorCinput.TabIndex = 16;
            this.colorCinput.Text = "2";
            this.colorCinput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // inputCheckBox
            // 
            this.inputCheckBox.AutoSize = true;
            this.inputCheckBox.Checked = true;
            this.inputCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.inputCheckBox.Location = new System.Drawing.Point(222, 284);
            this.inputCheckBox.Name = "inputCheckBox";
            this.inputCheckBox.Size = new System.Drawing.Size(72, 17);
            this.inputCheckBox.TabIndex = 17;
            this.inputCheckBox.Text = "Play input";
            this.inputCheckBox.UseVisualStyleBackColor = true;
            // 
            // outputCheckBox
            // 
            this.outputCheckBox.AutoSize = true;
            this.outputCheckBox.Location = new System.Drawing.Point(222, 308);
            this.outputCheckBox.Name = "outputCheckBox";
            this.outputCheckBox.Size = new System.Drawing.Size(79, 17);
            this.outputCheckBox.TabIndex = 18;
            this.outputCheckBox.Text = "Play output";
            this.outputCheckBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 591);
            this.Controls.Add(this.outputCheckBox);
            this.Controls.Add(this.inputCheckBox);
            this.Controls.Add(this.colorCinput);
            this.Controls.Add(this.colorBinput);
            this.Controls.Add(this.colorAinput);
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
        private System.Windows.Forms.TextBox colorAinput;
        private System.Windows.Forms.TextBox colorBinput;
        private System.Windows.Forms.TextBox colorCinput;
        private System.Windows.Forms.CheckBox inputCheckBox;
        private System.Windows.Forms.CheckBox outputCheckBox;
    }
}

