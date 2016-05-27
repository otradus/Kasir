using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Kasir
{
	delegate void Function();	// a simple delegate for marshalling calls from event handlers to the GUI thread

	public partial class MainForm : Form
	{
        public static bool verified = false;

        public MainForm()
		{
            InitializeComponent();
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}


		private DPFP.Template Template;

        private void MainForm_Load(object sender, EventArgs e)
        {

            CaptureForm Verifier = new CaptureForm();
            Verifier.Verify(Template);

        }
    }
}