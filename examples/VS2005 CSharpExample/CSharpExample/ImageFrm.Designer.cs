namespace CSharpExample
{
    partial class ImageFrm
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
            this.lumiPictureBox = new System.Windows.Forms.PictureBox();
            this.Match = new System.Windows.Forms.Button();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.CaptureWithPD = new System.Windows.Forms.Button();
            this.txtTriggerTimeout = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ChangeTimeout = new System.Windows.Forms.Button();
            this.btnTestAPI = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.lumiPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lumiPictureBox
            // 
            this.lumiPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lumiPictureBox.Location = new System.Drawing.Point(19, 18);
            this.lumiPictureBox.Name = "lumiPictureBox";
            this.lumiPictureBox.Size = new System.Drawing.Size(352, 544);
            this.lumiPictureBox.TabIndex = 0;
            this.lumiPictureBox.TabStop = false;
            // 
            // Match
            // 
            this.Match.Location = new System.Drawing.Point(482, 21);
            this.Match.Name = "Match";
            this.Match.Size = new System.Drawing.Size(86, 50);
            this.Match.TabIndex = 3;
            this.Match.Text = "Match w/ Presence Detection Feedback";
            this.Match.UseVisualStyleBackColor = true;
            this.Match.Click += new System.EventHandler(this.Match_Click);
            // 
            // statusTextBox
            // 
            this.statusTextBox.AcceptsReturn = true;
            this.statusTextBox.Location = new System.Drawing.Point(390, 133);
            this.statusTextBox.Multiline = true;
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.statusTextBox.Size = new System.Drawing.Size(369, 429);
            this.statusTextBox.TabIndex = 4;
            // 
            // CaptureWithPD
            // 
            this.CaptureWithPD.Location = new System.Drawing.Point(390, 21);
            this.CaptureWithPD.Name = "CaptureWithPD";
            this.CaptureWithPD.Size = new System.Drawing.Size(86, 50);
            this.CaptureWithPD.TabIndex = 5;
            this.CaptureWithPD.Text = "Lumi Capture w/ Presence Detection Feedback";
            this.CaptureWithPD.UseVisualStyleBackColor = true;
            this.CaptureWithPD.Click += new System.EventHandler(this.CaptureWithPD_Click);
            // 
            // txtTriggerTimeout
            // 
            this.txtTriggerTimeout.Location = new System.Drawing.Point(666, 37);
            this.txtTriggerTimeout.Name = "txtTriggerTimeout";
            this.txtTriggerTimeout.Size = new System.Drawing.Size(81, 20);
            this.txtTriggerTimeout.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(663, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Trigger Timeout";
            // 
            // ChangeTimeout
            // 
            this.ChangeTimeout.Location = new System.Drawing.Point(574, 21);
            this.ChangeTimeout.Name = "ChangeTimeout";
            this.ChangeTimeout.Size = new System.Drawing.Size(86, 50);
            this.ChangeTimeout.TabIndex = 8;
            this.ChangeTimeout.Text = "Change Timeout";
            this.ChangeTimeout.UseVisualStyleBackColor = true;
            this.ChangeTimeout.Click += new System.EventHandler(this.ChangeTimeout_Click);
            // 
            // btnTestAPI
            // 
            this.btnTestAPI.Location = new System.Drawing.Point(390, 77);
            this.btnTestAPI.Name = "btnTestAPI";
            this.btnTestAPI.Size = new System.Drawing.Size(86, 50);
            this.btnTestAPI.TabIndex = 9;
            this.btnTestAPI.Text = "Test API";
            this.btnTestAPI.UseVisualStyleBackColor = true;
            this.btnTestAPI.Click += new System.EventHandler(this.btnTestAPI_Click);
            // 
            // ImageFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 583);
            this.Controls.Add(this.btnTestAPI);
            this.Controls.Add(this.ChangeTimeout);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTriggerTimeout);
            this.Controls.Add(this.CaptureWithPD);
            this.Controls.Add(this.statusTextBox);
            this.Controls.Add(this.Match);
            this.Controls.Add(this.lumiPictureBox);
            this.Name = "ImageFrm";
            this.Text = "CSharp Example";
            this.Load += new System.EventHandler(this.Image_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageFrm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.lumiPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox lumiPictureBox;
        private System.Windows.Forms.Button Match;
        private System.Windows.Forms.TextBox statusTextBox;
        private System.Windows.Forms.Button CaptureWithPD;
        private System.Windows.Forms.TextBox txtTriggerTimeout;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ChangeTimeout;
        private System.Windows.Forms.Button btnTestAPI;
    }
}

