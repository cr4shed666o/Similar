using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;

namespace Similar
{
    public partial class Form1 : Form
    {
        private Thread thread;
        public string _Url = null;
        public string _Alexa = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Work()
        {

            using (HttpRequest httpRequest = new HttpRequest())
            {
                httpRequest.UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.79 Safari/537.36 Maxthon/5.2.5.4000";
                httpRequest.AddHeader("Origin", "chrome-extension://necpbmbhhdiplmfhmjicabdeighkndkn");
                string che = httpRequest.Get("https://hiddmondbot.info/check.php", null).ToString();
                string input = httpRequest.Post("https://serving-api.similarsites.com/data",
                    "url=" + this.Request.Text, "application/x-www-form-urlencoded").ToString();
                int num = 0;

                foreach (object obj in new Regex("Site\":\"(.*?)\".*?GlobalRank\":([0-9]{1,10}).*?Title\":\"(.*?)\"").Matches(input))
                {
                    Match match = (Match) obj;
                    bunifuCustomDataGrid1.Invoke(new Action(() =>
                    {
                        var b = bunifuCustomDataGrid1.Rows.Add();
                        bunifuCustomDataGrid1.Rows[b].Cells[0].Value = "https://" + match.Groups[1].Value;
                        bunifuCustomDataGrid1.Rows[b].Cells[1].Value = match.Groups[2].Value;
                        bunifuCustomDataGrid1.Rows[b].Cells[2].Value = match.Groups[3].Value;
                    }));

                }

            }

        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            this.bunifuCustomDataGrid1.Rows.Clear();
            this.thread = new Thread(new ThreadStart(this.Work));
            this.thread.IsBackground = true;
            this.thread.Priority = ThreadPriority.Normal;
            this.thread.Start();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстовый документ (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.Unicode);
                try
                {
                    List<int> col_n = new List<int>();
                    foreach (DataGridViewColumn col in bunifuCustomDataGrid1.Columns)
                        if (col.Visible)
                        {
                            //sw.Write(col.HeaderText + "\t");
                            col_n.Add(col.Index);
                        }
                    //sw.WriteLine();
                    int x = bunifuCustomDataGrid1.RowCount;
                    if (bunifuCustomDataGrid1.AllowUserToAddRows) x--;

                    for (int i = 0; i < x; i++)
                    {
                        for (int y = 0; y < col_n.Count; y++)
                            sw.Write(bunifuCustomDataGrid1[col_n[y], i].Value + " | ");
                        sw.Write(" \r\n");
                    }
                    sw.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }
    }
}


