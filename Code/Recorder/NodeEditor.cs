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
    public partial class NodeEditor : Form
    {
        public NodeEditor()
        {
            InitializeComponent();
        }

        public object EditorObject
        {
            get { return taskGrid.SelectedObject; }
            set { taskGrid.SelectedObject = value; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
