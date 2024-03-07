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
            this.txtAccNum.Location = new System.Drawing.Point(313, 140);
            this.txtAccNum.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtAccNum.Name = "txtAccNum";
            this.txtAccNum.Size = new System.Drawing.Size(284, 26);
            this.txtAccNum.TabIndex = 0;
            //this.txtAccNum.TextChanged += new System.EventHandler(this.txtAccNum_TextChanged);
            // 
            // txtPin
            // 
            this.txtPin.Location = new System.Drawing.Point(313, 206);
            this.txtPin.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtPin.Name = "txtPin";
            this.txtPin.Size = new System.Drawing.Size(284, 26);
            this.txtPin.TabIndex = 1;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(313, 290);
            this.btnConfirm.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(283, 59);
            this.btnConfirm.TabIndex = 2;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // ATMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 599);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.txtPin);
            this.Controls.Add(this.txtAccNum);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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

