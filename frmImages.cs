using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace EpubCreator
{
    /// <summary>
    /// Simple form to grab an image location and set a public property for later use
    /// </summary>
	public partial class frmImages : Form
	{
		public String location = "";
		public String name = "";
		private Form _ParentForm = null;

		public frmImages(Form ParentForm)
		{
			InitializeComponent();
			_ParentForm = ParentForm;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			location = tbxImageFileName.Text;
			name = tbxImageName.Text;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnImage_Click(object sender, EventArgs e)
		{
			dlgFile.RestoreDirectory = true;
			dlgFile.Multiselect = false;
            dlgFile.Filter = "image files (*.jpg;*.jpeg;*.gif;*.png)|*.jpg;*.jpeg;*.gif;*.png";

			if (dlgFile.ShowDialog() == DialogResult.OK)
			{
				tbxImageFileName.Text = dlgFile.FileName;

				CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
				TextInfo textInfo = cultureInfo.TextInfo;

				if (_ParentForm.Controls.Find("tbxTitle", true) != null)
				{
					string Title = ((TextBox)_ParentForm.Controls.Find("tbxTitle", true)[0]).Text;
					string Author = ((TextBox)_ParentForm.Controls.Find("tbxAuthorSort", true)[0]).Text;

					Author = Author.Substring(0, Author.IndexOf(",") < 0 ? Author.Length : Author.IndexOf(",")).Replace(" ", "");
					Title = textInfo.ToTitleCase(Title);
					Title = Author + "_" + Title.Replace(" ", "").Replace(",", "");

					tbxImageName.Text = Title + Path.GetExtension(dlgFile.FileName);
				}
				else
					tbxImageName.Text = Path.GetFileName(dlgFile.FileName);
			}
		}
	}
}