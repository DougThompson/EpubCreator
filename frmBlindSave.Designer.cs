namespace EpubCreator
{
	partial class frmBlindSave
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
			this.pbarInfo = new System.Windows.Forms.ProgressBar();
			this.lblInfo = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// pbarInfo
			// 
			this.pbarInfo.Location = new System.Drawing.Point(12, 42);
			this.pbarInfo.Name = "pbarInfo";
			this.pbarInfo.Size = new System.Drawing.Size(552, 23);
			this.pbarInfo.TabIndex = 0;
			// 
			// lblInfo
			// 
			this.lblInfo.AutoSize = true;
			this.lblInfo.Location = new System.Drawing.Point(12, 13);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(35, 13);
			this.lblInfo.TabIndex = 1;
			this.lblInfo.Text = "label1";
			// 
			// frmBlindSave
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(576, 82);
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.pbarInfo);
			this.Name = "frmBlindSave";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Creating ePub files...";
			this.Load += new System.EventHandler(this.frmBlindSave_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar pbarInfo;
		private System.Windows.Forms.Label lblInfo;
	}
}