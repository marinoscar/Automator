using Automator.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{
    public partial class MainForm : Form
    {

        private DesktopRecorder _recorder;
        private FileInfo _recorderFile;
        private bool _isDirty;

        public MainForm()
        {
            InitializeComponent();
            _recorder = new DesktopRecorder();
            _recorder.LogAction += Recorder_LogAction;
        }

        private void SetDirty()
        {
            if (!_isDirty)
            {
                Text = Text + " *";
            }
            _isDirty = true;
        }

        private void Recorder_LogAction(object sender, string e)
        {
            txtConsole.AppendText(string.Format("{0}\n", e));
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
        }



        private void LoadTree()
        {
            LoadTree(_recorder.TaskList);
        }

        private void LoadTree(List<ITask> tasks)
        {
            treeResult.Nodes.Clear();
            var root = new TreeNode()
            {
                Name = "root",
                Text = "Tasks"
            };
            treeResult.Nodes.Add(root);
            treeResult.ExpandAll();
            tabControl.SelectedTab = tabResult;
            pbExecution.Visible = true;
            pbExecution.Minimum = 1;
            pbExecution.Maximum = tasks.Count;
            foreach (var task in tasks)
            {
                var token = JToken.FromObject(task);
                var node = LoadNode(root, token);
                pbExecution.Value = tasks.IndexOf(task) + 1;
                node.Tag = task;
                node.ContextMenuStrip = mnuNodeCtx;
                node.EnsureVisible();
                Application.DoEvents();
            }
        }

        private TreeNode LoadNode(TreeNode root, JToken token)
        {
            if ((token.Type == JTokenType.Property && ((((JProperty)token).Name == "TaskCaption") || (((JProperty)token).Name == "TaskName")))
                || IsTokenScalarValue(token)) return null;
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

        private void btnRecord_Click(object sender, EventArgs e)
        {
            btnRecord.Enabled = false;
            _recorder.Start();
        }

        private void btnStopRecording_Click(object sender, EventArgs e)
        {
            btnStopRecording.Enabled = false;
            _recorder.Stop();
            tabResult.Show();
            LoadTree();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            var root = treeResult.Nodes[0];
            var taskNodes = root.Nodes.Cast<TreeNode>().ToList();
            pbExecution.Visible = true;
            pbExecution.Minimum = 1;
            pbExecution.Maximum = taskNodes.Count;
            TreeNode prev = null;
            foreach (var node in taskNodes)
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
                pbExecution.Value = taskNodes.IndexOf(node) + 1;
                Application.DoEvents();
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog()
            {
                Filter = "Json file (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = ".json",
                Title = "Open recording",
                RestoreDirectory = true,
            };
            if (openDialog.ShowDialog() != DialogResult.OK) return;
            if (string.IsNullOrWhiteSpace(openDialog.FileName)) return;
            DoOpen(openDialog.FileName);
        }

        private void UpdateTitle()
        {
            this.Text = string.Format("Automator Session {0}", _recorderFile.Name);
        }

        private void DoSaveAs()
        {
            var saveDialog = new SaveFileDialog()
            {
                Title = "Save the recorded tasks",
                Filter = "Json file (*.json)|*.json|All Files (*.*)|*.*",
                RestoreDirectory = true,
                DefaultExt = ".json"
            };
            if (saveDialog.ShowDialog() != DialogResult.OK) return;
            _recorderFile = new FileInfo(saveDialog.FileName);
            UpdateTitle();
            DoSave(_recorderFile.FullName);
        }

        private void DoSave(string fileName)
        {
            var tasks = treeResult.Nodes[0].Nodes.Cast<TreeNode>().Select(i => (ITask)i.Tag).ToList();
            File.WriteAllText(fileName, JsonConvert.SerializeObject(tasks, Formatting.Indented));
            MessageBox.Show("File saved", "The file was saved");
        }

        private void DoOpen(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            if (!File.Exists(fileName)) return;
            _recorderFile = new FileInfo(fileName);
            UpdateTitle();
            var loader = new TaskLoader();
            LoadTree(loader.LoadTasks(fileName));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_recorderFile == null) DoSaveAs();
            else DoSave(_recorderFile.FullName);
        }

        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            DoSaveAs();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Close Application", "Are you sure you want to exit", MessageBoxButtons.YesNo) == DialogResult.Yes) Application.Exit();
        }

        private void mnuNodeDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete Task", "Are you sure you want to delete the selected task", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
            if (treeResult.Nodes[0] == treeResult.SelectedNode) return;
            treeResult.SelectedNode.Remove();

        }

        private void treeResult_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeResult.SelectedNode = e.Node;
        }

        private void mnuNodeEdit_Click(object sender, EventArgs e)
        {
            var task = (ITask)treeResult.SelectedNode.Tag;
            var editor = new NodeEditor()
            {
                EditorObject = task
            };
            if (editor.ShowDialog() != DialogResult.OK) return;
            treeResult.SelectedNode.Tag = EditNode(treeResult.SelectedNode, task);
        }

        private TreeNode EditNode(TreeNode node, ITask task)
        {
            node.Text = task.TaskCaption;
            node.Nodes.Clear();
            var obj = JObject.FromObject(task);
            foreach (var token in obj.Values<JToken>())
            {
                LoadNode(node, token);
            }
            treeResult.Refresh();
            Application.DoEvents();
            node.EnsureVisible();
            SetDirty();
            return node;
        }

        private void mnuUpdateWait_Click(object sender, EventArgs e)
        {
            var editor = new WaitToolEditor();
            if (editor.ShowDialog() != DialogResult.OK) return;
            var nodes = treeResult.Nodes[0].Nodes.Cast<TreeNode>()
                .Where(i => i.Tag.GetType().Name == typeof(WaitTask).Name).ToList();
            foreach(var node in nodes)
            {
                var task = (WaitTask)node.Tag;
                task.DurationInMs = editor.Value;
                EditNode(node, task);
            }


        }
    }
}
