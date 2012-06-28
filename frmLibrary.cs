using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Configuration;
using System.Xml;
using Ionic.Utils.Zip;

namespace EpubCreator
{
    /// <summary>
    /// Add an EPUB file to a library, either private or public (obfuscated)
    /// </summary>
	public partial class frmLibrary : Form
	{
        public Boolean AddToPublicLibrary = false;
		public Boolean WasCancelled = false;
		private Form _ParentForm = null;

		public frmLibrary(Form ParentForm)
		{
			InitializeComponent();
			_ParentForm = ParentForm;
		}

		private void frmLibrary_Load(object sender, EventArgs e)
		{

		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			WasCancelled = true;
			this.Close();
		}

		private void btnOpenFile_Click(object sender, EventArgs e)
		{
            // Open the file and parse the data
			dlgFile.RestoreDirectory = true;
			dlgFile.Multiselect = false;
			dlgFile.Filter = "ePub files (*.epub;*.zip)|*.epub;*.zip";

			if (dlgFile.ShowDialog() == DialogResult.OK)
			{
				tbxFileName.Text = dlgFile.FileName;
				Program.ReadEpub(tbxFileName.Text, lstImages);

				String subFolder = Path.GetDirectoryName(dlgFile.FileName);
				subFolder = subFolder.Substring(subFolder.LastIndexOf('\\') + 1);
				tbxSubfolder.Text = subFolder;
			}
		}

		private void btnGetImages_Click(object sender, EventArgs e)
		{
            // Parse the EPUB for images
			if (!String.IsNullOrEmpty(tbxFileName.Text))
			{
				Program.ReadEpub(tbxFileName.Text, lstImages);
			}
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
            // Add the details to the library file
            Program.AddToLibrary(tbxFileName.Text, lstImages, tbxSubfolder.Text, AddToPublicLibrary);
            MessageBox.Show("ePub added to Library.");
		}

	}
}