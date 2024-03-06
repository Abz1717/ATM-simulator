using System;

namespace ATM_simulator
{
    partial class ATMForm
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
            this.txtAccNum = new System.Windows.Forms.TextBox();
            this.txtPin = new System.Windows.Forms.TextBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtAccNum
            // 
            this.txtAccNum.Location = new System.Drawing.Point(417, 175);
            this.txtAccNum.Name = "txtAccNum";
            this.txtAccNum.Size = new System.Drawing.Size(378, 31);
            this.txtAccNum.TabIndex = 0;
            // 
            // txtPin
            // 
            this.txtPin.Location = new System.Drawing.Point(417, 258);
            this.txtPin.Name = "txtPin";
            this.txtPin.Size = new System.Drawing.Size(378, 31);
            this.txtPin.TabIndex = 1;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(417, 363);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(377, 74);
            this.btnConfirm.TabIndex = 2;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // ATMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1186, 749);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.txtPin);
            this.Controls.Add(this.txtAccNum);
            this.Name = "ATMForm";
            this.Text = "ATM";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        #endregion

        private System.Windows.Forms.TextBox txtAccNum;
        private System.Windows.Forms.TextBox txtPin;
        private System.Windows.Forms.Button btnConfirm;
    }
}

