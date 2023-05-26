using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace LR_31
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            processListView.ContextMenuStrip = contextMenuStrip;
            processListView.MouseClick += processListView_MouseClick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshProcessList();
        }
        private void processListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip.Show(processListView, e.Location);
            }
        }
        private void RefreshProcessList()
        {
            processListView.Items.Clear();

            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                ListViewItem item = new ListViewItem(process.ProcessName);
                item.SubItems.Add(process.Id.ToString());
                processListView.Items.Add(item);
            }
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (processListView.SelectedItems.Count == 0)
                e.Cancel = true;
        }

        private void viewDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (processListView.SelectedItems.Count > 0)
            {
                int processId = int.Parse(processListView.SelectedItems[0].SubItems[1].Text);
                Process process = Process.GetProcessById(processId);
                string details = $"Process Name: {process.ProcessName}\n" +
                                 $"ID: {process.Id}\n" +
                                 $"Start Time: {process.StartTime}\n" +
                                 $"Threads: {process.Threads.Count}\n" +
                                 $"Modules: {process.Modules.Count}\n";

                MessageBox.Show(details, "Process Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void stopProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (processListView.SelectedItems.Count > 0)
            {
                int processId = int.Parse(processListView.SelectedItems[0].SubItems[1].Text);
                Process process = Process.GetProcessById(processId);

                if (process != null && !process.HasExited)
                {
                    process.Kill();
                    MessageBox.Show("Процеси зупинено успішно.", "Процеси зупинено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshProcessList();
                }
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void exportToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            saveFileDialog.Title = "Export Process List";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    foreach (ListViewItem item in processListView.Items)
                    {
                        writer.WriteLine($"{item.Text}, {item.SubItems[1].Text}");
                    }
                }

                MessageBox.Show("Список процесів збережено успішно.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
