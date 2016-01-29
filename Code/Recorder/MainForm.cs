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
    public partial class MainForm : Form
    {

        private DesktopRecorder _recorder;

        public MainForm()
        {
            InitializeComponent();
            _recorder = new DesktopRecorder();
            _recorder.LogAction += Recorder_LogAction;
        }

        private void Recorder_LogAction(object sender, string e)
        {
            txtConsole.AppendText(string.Format("{0}\n",e));
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _recorder.Start();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _recorder.Stop();
            txtConsole.Clear();
            txtConsole.AppendText(_recorder.GetCommands());
        }
    }
}
