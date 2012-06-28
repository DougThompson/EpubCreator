using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EpubCreator
{
	public partial class frmBlindSave : Form
	{
		public int NumFiles;
		public string Info;

		public frmBlindSave()
		{
			InitializeComponent();
		}

		private void frmBlindSave_Load(object sender, EventArgs e)
		{
			pbarInfo.Minimum = 1;
			pbarInfo.Maximum = 100;
			pbarInfo.Step = 1;
		}

		public void UpdateInfo()
		{
			pbarInfo.Maximum = NumFiles;
			lblInfo.Text = Info;
			Application.DoEvents();
		}

		public void UpdateProgress()
		{
			pbarInfo.PerformStep();
			Application.DoEvents();
		}
	}
}
