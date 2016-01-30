using Automator.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            txtConsole.AppendText(string.Format("{0}\n", e));
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
            tabResult.Show();
            LoadTree();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            var root = treeResult.Nodes[0];
            var taskNodes = root.Nodes.Cast<TreeNode>().ToList();
            TreeNode prev = null;
            foreach(var node in taskNodes)
            {
                Application.DoEvents();
                var task = ((ITask)node.Tag);
                node.ForeColor = Color.Blue;
                treeResult.SelectedNode = node;
                node.EnsureVisible();
                Application.DoEvents();
                task.Execute();
                if (prev != null)
                    prev.ForeColor = Color.Black;
                prev = node;
                Application.DoEvents();
            }
            MessageBox.Show("Robot Completed", "Robot");
        }

        private void LoadTree()
        {
            treeResult.Nodes.Clear();
            var root = new TreeNode()
            {
                Name = "root",
                Text = "Tasks"
            };
            var tasks = _recorder.TaskList;
            foreach(var task in _recorder.TaskList)
            {
                var token = JToken.FromObject(task);
                var node = LoadNode(root, token);
                node.Tag = task;
            }
            treeResult.Nodes.Add(root);
            treeResult.ExpandAll();
            tabControl.SelectedTab = tabResult;
        }

        private TreeNode LoadNode(TreeNode root, JToken token)
        {
            if ((token.Type == JTokenType.Property && ((JProperty)token).Name == "TaskCaption") || IsTokenScalarValue(token)) return null;
            var name = GetName(token);
            var newNode = new TreeNode()
            {
                Name = string.Format("{0}.{1}", root.Name, name),
                Text = name
            };
            root.Nodes.Add(newNode);
            if (token.HasValues)
            {
                foreach (JToken childToken in token)
                {
                    LoadNode(newNode, childToken);
                }
            }
            return newNode;
        }

        private string GetName(JToken token)
        {
            if (token.Type == JTokenType.Object)
                return token.Values<JProperty>().FirstOrDefault(i => i.Name == "TaskCaption").Value.ToString();
            if (token.Type == JTokenType.Property)
            {
                var prop = (JProperty)token;
                if (IsTokenScalarValue(prop.Value))
                    return string.Format("{0}: {1}", prop.Name, prop.Value.ToString());
                else
                    return prop.Name;
            }
            return token.ToString();
        }

        private bool IsTokenScalarValue(JToken token)
        {
            return !(token.Type == JTokenType.Array || token.Type == JTokenType.Object || token.Type == JTokenType.Property);
        }
    }
}
