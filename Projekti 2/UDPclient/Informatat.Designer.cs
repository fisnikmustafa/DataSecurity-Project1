
namespace UDPclient
{
    partial class Informatat
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
            this.btnVerify = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnVerify
            // 
            this.btnVerify.Location = new System.Drawing.Point(133, 111);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(102, 58);
            this.btnVerify.TabIndex = 0;
            this.btnVerify.Text = "Verify Signature";
            this.btnVerify.UseVisualStyleBackColor = true;
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // Informatat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 263);
            this.Controls.Add(this.btnVerify);
            this.Name = "Informatat";
            this.Text = "Informatat";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnVerify;
    }
}