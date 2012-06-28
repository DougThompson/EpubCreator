namespace EpubCreator
{
	partial class frmImages
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
			this.tbxImageName = new System.Windows.Forms.TextBox();
			this.label28 = new System.Windows.Forms.Label();
			this.btnImage = new System.Windows.Forms.Button();
			this.tbxImageFileName = new System.Windows.Forms.TextBox();
			this.label27 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.dlgFile = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// tbxImageName
			// 
			this.tbxImageName.Location = new System.Drawing.Point(81, 38);
			this.tbxImageName.Name = "tbxImageName";
			this.tbxImageName.Size = new System.Drawing.Size(483, 20);
			this.tbxImageName.TabIndex = 58;
			// 
			// label28
			// 
			this.label28.AutoSize = true;
			this.label28.Location = new System.Drawing.Point(11, 41);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(67, 13);
			this.label28.TabIndex = 60;
			this.label28.Text = "Image Name";
			// 
			// btnImage
			// 
			this.btnImage.Location = new System.Drawing.Point(570, 10);
			this.btnImage.Name = "btnImage";
			this.btnImage.Size = new System.Drawing.Size(24, 23);
			this.btnImage.TabIndex = 57;
			this.btnImage.Text = "...";
			this.btnImage.UseVisualStyleBackColor = true;
			this.btnImage.Click += new System.EventHandler(this.btnImage_Click);
			// 
			// tbxImageFileName
			// 
			this.tbxImageFileName.Location = new System.Drawing.Point(81, 12);
			this.tbxImageFileName.Name = "tbxImageFileName";
			this.tbxImageFileName.Size = new System.Drawing.Size(483, 20);
			this.tbxImageFileName.TabIndex = 56;
			// 
			// label27
			// 
			this.label27.AutoSize = true;
			this.label27.Location = new System.Drawing.Point(11, 15);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(54, 13);
			this.label27.TabIndex = 59;
			this.label27.Text = "File Name";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(438, 76);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 61;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(519, 76);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 62;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// dlgFile
			// 
			this.dlgFile.FileName = "openFileDialog1";
			// 
			// Form2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(604, 112);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.tbxImageName);
			this.Controls.Add(this.label28);
			this.Controls.Add(this.btnImage);
			this.Controls.Add(this.tbxImageFileName);
			this.Controls.Add(this.label27);
			this.Name = "Form2";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Cover Image";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbxImageName;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.Button btnImage;
		private System.Windows.Forms.TextBox tbxImageFileName;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.OpenFileDialog dlgFile;
	}
}