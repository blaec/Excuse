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

namespace Excuse
{

    public partial class Form1 : Form
    {
        private Excuse currentExcuse = new Excuse();
        private string selectedFolder = "";
        private bool formChanged = false;
        Random random = new Random();

        private readonly static string FILE_FILTER = "Excuse files (*.excuse)|*.excuse|All files (*.*)|*.*";
        private readonly static string FILE_EXTENTION = ".excuse";

        public Form1()
        {
            InitializeComponent();
            currentExcuse.LastUsed = lastUsed.Value;
            save.Enabled = false;
            open.Enabled = false;
            randomExcuse.Enabled = false;
        }

        private void folder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = selectedFolder;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                selectedFolder = folderBrowserDialog1.SelectedPath;
                save.Enabled = true;
                open.Enabled = true;
                randomExcuse.Enabled = true;
            }
        }

        private void UpdateForm(bool changed)
        {
            if (changed)
            {
                this.Text = "Excuse Manager*";
            }
            else
            {
                this.description.Text = currentExcuse.Description;
                this.results.Text = currentExcuse.Results;
                this.lastUsed.Value = currentExcuse.LastUsed;
                if (!String.IsNullOrEmpty(currentExcuse.ExcusePath))
                {
                    fileDate.Text = File.GetLastWriteTime(currentExcuse.ExcusePath).ToString();
                }
                this.Text = "Excuse Manager";
            }
            this.formChanged = changed;
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(description.Text) || String.IsNullOrEmpty(results.Text))
            {
                MessageBox.Show("Please specify an excuse and a result", "Unable to save", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                saveFileDialog1.InitialDirectory = selectedFolder;
                saveFileDialog1.Filter = FILE_FILTER;
                saveFileDialog1.FileName = description.Text + FILE_EXTENTION;
                DialogResult result = saveFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    currentExcuse.Save(saveFileDialog1.FileName);
                    UpdateForm(false);
                    MessageBox.Show("Escuse written");
                }
            }
        }

        private void open_Click(object sender, EventArgs e)
        {
            if (CheckChanged())
            {
                openFileDialog1.InitialDirectory = selectedFolder;
                openFileDialog1.Filter = FILE_FILTER;
                openFileDialog1.FileName = description.Text + FILE_EXTENTION;
                DialogResult result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    currentExcuse = new Excuse(openFileDialog1.FileName);
                    UpdateForm(false);
                }
            }
        }

        private void randomExcuse_Click(object sender, EventArgs e)
        {
            if (CheckChanged())
            {
                currentExcuse = new Excuse(random, selectedFolder);
                UpdateForm(false);
            }
        }

        private bool CheckChanged()
        {
            bool checkResult = true;
            if (formChanged)
            {
                DialogResult result = MessageBox.Show
                    (
                        "The current excuse has not been saved. Continue?",
                        "Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );
                checkResult = result == DialogResult.Yes;
            }
            return checkResult;
        }

        private void description_TextChanged(object sender, EventArgs e)
        {
            currentExcuse.Description = description.Text;
            UpdateForm(true);
        }

        private void results_TextChanged(object sender, EventArgs e)
        {
            currentExcuse.Results = results.Text;
            UpdateForm(true);
        }

        private void lastUsed_ValueChanged(object sender, EventArgs e)
        {
            currentExcuse.LastUsed = lastUsed.Value;
            UpdateForm(true);
        }
    }
}
