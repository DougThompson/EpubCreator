namespace EpubCreator
{
	partial class frmLibrary
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
			this.dlgFile = new System.Windows.Forms.OpenFileDialog();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOpenFile = new System.Windows.Forms.Button();
			this.tbxFileName = new System.Windows.Forms.TextBox();
			this.lstImages = new System.Windows.Forms.ListBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnGetImages = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.tbxSubfolder = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// dlgFile
			// 
			this.dlgFile.FileName = "openFileDialog1";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(401, 224);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(100, 23);
			this.btnCancel.TabIndex = 13;
			this.btnCancel.Text = "Close";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOpenFile
			// 
			this.btnOpenFile.Location = new System.Drawing.Point(477, 10);
			this.btnOpenFile.Name = "btnOpenFile";
			this.btnOpenFile.Size = new System.Drawing.Size(24, 23);
			this.btnOpenFile.TabIndex = 12;
			this.btnOpenFile.Text = "...";
			this.btnOpenFile.UseVisualStyleBackColor = true;
			this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
			// 
			// tbxFileName
			// 
			this.tbxFileName.Location = new System.Drawing.Point(85, 12);
			this.tbxFileName.Name = "tbxFileName";
			this.tbxFileName.Size = new System.Drawing.Size(386, 20);
			this.tbxFileName.TabIndex = 14;
			// 
			// lstImages
			// 
			this.lstImages.FormattingEnabled = true;
			this.lstImages.Location = new System.Drawing.Point(85, 74);
			this.lstImages.Name = "lstImages";
			this.lstImages.Size = new System.Drawing.Size(386, 134);
			this.lstImages.TabIndex = 15;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(11, 15);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(23, 13);
			this.label5.TabIndex = 29;
			this.label5.Text = "File";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 74);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 30;
			this.label1.Text = "Cover Image";
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(286, 224);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(100, 23);
			this.btnAdd.TabIndex = 31;
			this.btnAdd.Text = "Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnGetImages
			// 
			this.btnGetImages.Location = new System.Drawing.Point(171, 224);
			this.btnGetImages.Name = "btnGetImages";
			this.btnGetImages.Size = new System.Drawing.Size(100, 23);
			this.btnGetImages.TabIndex = 32;
			this.btnGetImages.Text = "Get Images";
			this.btnGetImages.UseVisualStyleBackColor = true;
			this.btnGetImages.Click += new System.EventHandler(this.btnGetImages_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(11, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(52, 13);
			this.label2.TabIndex = 34;
			this.label2.Text = "Subfolder";
			// 
			// tbxSubfolder
			// 
			this.tbxSubfolder.Location = new System.Drawing.Point(85, 38);
			this.tbxSubfolder.Name = "tbxSubfolder";
			this.tbxSubfolder.Size = new System.Drawing.Size(386, 20);
			this.tbxSubfolder.TabIndex = 33;
			// 
			// frmLibrary
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(514, 258);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbxSubfolder);
			this.Controls.Add(this.btnGetImages);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.lstImages);
			this.Controls.Add(this.tbxFileName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOpenFile);
			this.Name = "frmLibrary";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add ePub Book to Library";
			this.Load += new System.EventHandler(this.frmLibrary_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.OpenFileDialog dlgFile;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOpenFile;
		private System.Windows.Forms.TextBox tbxFileName;
		private System.Windows.Forms.ListBox lstImages;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnGetImages;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbxSubfolder;
	}
}