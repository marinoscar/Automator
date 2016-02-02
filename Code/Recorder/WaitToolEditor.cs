using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{
    public partial class WaitToolEditor : Form
    {
        public WaitToolEditor()
        {
            InitializeComponent();
        }

        public int Value
        {
            get { return (int)numValue.Value; }
            set { numValue.Value = value; }
        }


        private void WaitToolEditor_Load(object sender, EventArgs e)
        {
            numValue.Maximum = int.MaxValue;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
