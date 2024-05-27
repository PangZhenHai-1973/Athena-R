using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Athena_R
{
    public partial class MainForm : Form
    {

        static string CDirectory;//当前目录
        static ArrayList alorgid = new ArrayList();
        static ArrayList alorgoff = new ArrayList();
        static ArrayList alorgsize = new ArrayList();
        static ArrayList altraid = new ArrayList();
        static ArrayList altraoff = new ArrayList();
        static ArrayList altrasize = new ArrayList();
        static ArrayList alnewid = new ArrayList();
        static ArrayList alnewoff = new ArrayList();
        static ArrayList alnewsize = new ArrayList();

        public MainForm()
        {
            InitializeComponent();
        }

        bool PE(string s)//判断一个文件是否是标准的 PE 文件
        {
            FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            bool bl = false;
            if (fs.Length > 63)
            {
                int i = br.ReadUInt16();
                if (i == 23117)
                {
                    fs.Seek(60, SeekOrigin.Begin);
                    i = (int)br.ReadUInt32();
                    if (i < fs.Length)
                    {
                        fs.Seek(i, SeekOrigin.Begin);
                        i = br.ReadInt32();
                        if (i == 17744)
                        {
                            bl = true;
                        }
                    }
                }
            }
            br.Close();
            fs.Close();
            if (bl == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        void SaveARSetup(string s1, string s2, string s3)
        {
            string s = CDirectory + "配置";
            if (Directory.Exists(s) == false)
            {
                Directory.CreateDirectory(s);
            }
            s = CDirectory + "配置\\ARSetup.ini";
            StreamWriter sw = new StreamWriter(s, false);
            sw.WriteLine("Org=" + s1);
            sw.WriteLine("Tra=" + s2);
            sw.WriteLine("New=" + s3);
            sw.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            About Ab = new About();
            if (checkBox9.Checked == true)
            {
                Ab.TopMost = true;
            }
            else
            {
                Ab.TopMost = false;
            }
            if (MyDpi > 96F)
            {
                Ab.Font = this.Font;
                if (MyDpi <= 240F)
                {
                    Ab.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else
                {
                    Ab.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                }
                Ab.pictureBox1.Image = Athena_R.Properties.Resources.Athena_R256;
            }
            Ab.ShowDialog();
        }

        public static float MyDpi = 96F;

        private void MainForm_Load(object sender, EventArgs e)
        {
            CDirectory = Directory.GetCurrentDirectory(); //获得当前程序所在目录
            if (CDirectory.Length > 3)
            {
                CDirectory = CDirectory + "\\";
            }
            using (Graphics gh = Graphics.FromHwnd(IntPtr.Zero))
            {
                MyDpi = gh.DpiX;
                if (MyDpi > 96F)
                {
                    this.Font = new Font("Microsoft YaHei UI", 9F);
                    label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)((double)textBox1.Height / (double)2 - (double)label1.Height / (double)2) - 1);
                    label2.Location = new Point(label2.Location.X, textBox2.Location.Y + (int)((double)textBox2.Height / (double)2 - (double)label2.Height / (double)2) - 1);
                    label3.Location = new Point(label3.Location.X, textBox3.Location.Y + (int)((double)textBox3.Height / (double)2 - (double)label3.Height / (double)2) - 1);
                }
                else
                {
                    this.Font = new Font("宋体", 9F);
                }
            }
        }

        private void OrgFile(string s)
        {
            string s1 = textBox1.Text;
            string s2 = textBox2.Text;
            string s3 = textBox3.Text;
            if (s1 != s)
            {
                if (checkBox9.Checked)
                {
                    MainForm.ActiveForm.TopMost = false;
                    if (s == s2)
                    {
                        MessageBox.Show("文件不能与译版文件相同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (s == s3)
                    {
                        MessageBox.Show("文件不能与新版文件相同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (PE(s) == false)
                    {
                        MessageBox.Show("文件不是有效的 PE 文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (File.Exists(s2) == true)
                    {
                        if (FileVersionInfo.GetVersionInfo(s).FileVersion != FileVersionInfo.GetVersionInfo(s2).FileVersion)
                        {
                            MessageBox.Show("文件的版本与译版文件的版本不同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
                            FileStream fs2 = new FileStream(s2, FileMode.Open, FileAccess.Read);
                            if (fs.Length != fs2.Length)
                            {
                                if (checkBox9.Checked)
                                {
                                    MainForm.ActiveForm.TopMost = false;
                                    DialogResult dr = MessageBox.Show("文件的大小与译版文件的大小不同，是否要指定此文件？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                    if (dr == DialogResult.OK)
                                    {
                                        textBox1.Text = s;
                                        SaveARSetup(s, s2, s3);
                                    }
                                    MainForm.ActiveForm.TopMost = true;
                                }
                                else
                                {
                                    DialogResult dr = MessageBox.Show("文件的大小与译版文件的大小不同，是否要指定此文件？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                    if (dr == DialogResult.OK)
                                    {
                                        textBox1.Text = s;
                                        SaveARSetup(s, s2, s3);
                                    }
                                }
                            }
                            else
                            {
                                textBox1.Text = s;
                                SaveARSetup(s, s2, s3);
                            }
                            fs.Close();
                            fs2.Close();
                        }
                    }
                    else
                    {
                        textBox1.Text = s;
                        SaveARSetup(s, s2, s3);
                    }
                    MainForm.ActiveForm.TopMost = true;
                }
                else
                {
                    if (s == s2)
                    {
                        MessageBox.Show("文件不能与译版文件相同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (s == s3)
                    {
                        MessageBox.Show("文件不能与新版文件相同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (PE(s) == false)
                    {
                        MessageBox.Show("文件不是有效的 PE 文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (File.Exists(s2) == true)
                    {
                        if (FileVersionInfo.GetVersionInfo(s).FileVersion != FileVersionInfo.GetVersionInfo(s2).FileVersion)
                        {
                            MessageBox.Show("文件的版本与译版文件的版本不同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
                            FileStream fs2 = new FileStream(s2, FileMode.Open, FileAccess.Read);
                            if (fs.Length != fs2.Length)
                            {
                                if (checkBox9.Checked)
                                {
                                    MainForm.ActiveForm.TopMost = false;
                                    DialogResult dr = MessageBox.Show("文件的大小与译版文件的大小不同，是否要指定此文件？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                    if (dr == DialogResult.OK)
                                    {
                                        textBox1.Text = s;
                                        SaveARSetup(s, s2, s3);
                                    }
                                    MainForm.ActiveForm.TopMost = true;
                                }
                                else
                                {
                                    DialogResult dr = MessageBox.Show("文件的大小与译版文件的大小不同，是否要指定此文件？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                    if (dr == DialogResult.OK)
                                    {
                                        textBox1.Text = s;
                                        SaveARSetup(s, s2, s3);
                                    }
                                }
                            }
                            else
                            {
                                textBox1.Text = s;
                                SaveARSetup(s, s2, s3);
                            }
                            fs.Close();
                            fs2.Close();
                        }
                    }
                    else
                    {
                        textBox1.Text = s;
                        SaveARSetup(s, s2, s3);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.Filter = "DLL 文件或 EXE 文件(*.DLL;*.EXE)|*.DLL;*.EXE|所有文件(*.*)|*.*";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    OrgFile(open.FileName);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)//载入配置
        {
            string s = CDirectory + "配置\\ARSetup.ini";
            if (File.Exists(s) == true)
            {
                StreamReader sr = File.OpenText(s);
                textBox1.Text = sr.ReadLine().Remove(0, 4);
                textBox2.Text = sr.ReadLine().Remove(0, 4);
                textBox3.Text = sr.ReadLine().Remove(0, 4);
                sr.Close();
            } 
        }

        void TraFile(string s)
        {
            string s1 = textBox1.Text;
            string s2 = textBox2.Text;
            string s3 = textBox3.Text;
            if (s2 != s)
            {
                if (checkBox9.Checked)
                {
                    MainForm.ActiveForm.TopMost = false;
                    if (s == s1)
                    {
                        MessageBox.Show("文件不能与原版文件相同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (s == s3)
                    {
                        MessageBox.Show("文件不能与新版文件相同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (PE(s) == false)
                    {
                        MessageBox.Show("文件不是有效的 PE 文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (File.Exists(s1) == true)
                    {
                        if (FileVersionInfo.GetVersionInfo(s).FileVersion != FileVersionInfo.GetVersionInfo(s1).FileVersion)
                        {
                            MessageBox.Show("文件的版本与原版文件的版本不同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
                            FileStream fs1 = new FileStream(s1, FileMode.Open, FileAccess.Read);
                            if (fs.Length != fs1.Length)
                            {
                                if (checkBox9.Checked)
                                {
                                    MainForm.ActiveForm.TopMost = false;
                                    DialogResult dr = MessageBox.Show("文件的大小与原版文件的大小不同，是否要指定此文件？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                    if (dr == DialogResult.OK)
                                    {
                                        textBox2.Text = s;
                                        SaveARSetup(s1, s, s3);
                                    }
                                    MainForm.ActiveForm.TopMost = true;
                                }
                                else
                                {
                                    DialogResult dr = MessageBox.Show("文件的大小与原版文件的大小不同，是否要指定此文件？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                    if (dr == DialogResult.OK)
                                    {
                                        textBox2.Text = s;
                                        SaveARSetup(s1, s, s3);
                                    }
                                }
                            }
                            else
                            {
                                textBox2.Text = s;
                                SaveARSetup(s1, s, s3);
                            }
                            fs.Close();
                            fs1.Close();
                        }
                    }
                    else
                    {
                        textBox2.Text = s;
                        SaveARSetup(s1, s, s3);
                    }
                    MainForm.ActiveForm.TopMost = true;
                }
                else
                {
                    if (s == s1)
                    {
                        MessageBox.Show("文件不能与原版文件相同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (s == s3)
                    {
                        MessageBox.Show("文件不能与新版文件相同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (PE(s) == false)
                    {
                        MessageBox.Show("文件不是有效的 PE 文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (File.Exists(s1) == true)
                    {
                        if (FileVersionInfo.GetVersionInfo(s).FileVersion != FileVersionInfo.GetVersionInfo(s1).FileVersion)
                        {
                            MessageBox.Show("文件的版本与原版文件的版本不同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
                            FileStream fs1 = new FileStream(s1, FileMode.Open, FileAccess.Read);
                            if (fs.Length != fs1.Length)
                            {
                                if (checkBox9.Checked)
                                {
                                    MainForm.ActiveForm.TopMost = false;
                                    DialogResult dr = MessageBox.Show("文件的大小与原版文件的大小不同，是否要指定此文件？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                    if (dr == DialogResult.OK)
                                    {
                                        textBox2.Text = s;
                                        SaveARSetup(s1, s, s3);
                                    }
                                    MainForm.ActiveForm.TopMost = true;
                                }
                                else
                                {
                                    DialogResult dr = MessageBox.Show("文件的大小与原版文件的大小不同，是否要指定此文件？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                    if (dr == DialogResult.OK)
                                    {
                                        textBox2.Text = s;
                                        SaveARSetup(s1, s, s3);
                                    }
                                }
                            }
                            else
                            {
                                textBox2.Text = s;
                                SaveARSetup(s1, s, s3);
                            }
                            fs.Close();
                            fs1.Close();
                        }
                    }
                    else
                    {
                        textBox2.Text = s;
                        SaveARSetup(s1, s, s3);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.Filter = "DLL 文件或 EXE 文件(*.DLL;*.EXE)|*.DLL;*.EXE|所有文件(*.*)|*.*";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    TraFile(open.FileName);
                }
            }
        }

        void NewFile(string s)
        {
            string s1 = textBox1.Text;
            string s2 = textBox2.Text;
            string s3 = textBox3.Text;
            if (s3 != s)
            {
                if (checkBox9.Checked)
                {
                    MainForm.ActiveForm.TopMost = false;
                    if (s == s1)
                    {
                        MessageBox.Show("文件不能与原版文件相同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (s == s2)
                    {
                        MessageBox.Show("文件不能与译版文件相同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (PE(s) == false)
                    {
                        MessageBox.Show("文件不是有效的 PE 文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        textBox3.Text = s;
                        SaveARSetup(s1, s2, s);
                    }
                    MainForm.ActiveForm.TopMost = true;
                }
                else
                {
                    if (s == s1)
                    {
                        MessageBox.Show("文件不能与原版文件相同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (s == s2)
                    {
                        MessageBox.Show("文件不能与译版文件相同。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (PE(s) == false)
                    {
                        MessageBox.Show("文件不是有效的 PE 文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        textBox3.Text = s;
                        SaveARSetup(s1, s2, s);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.Filter = "DLL 文件或 EXE 文件(*.DLL;*.EXE)|*.DLL;*.EXE|所有文件(*.*)|*.*";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    NewFile(open.FileName);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            string s2 = textBox2.Text;
            string s3 = textBox3.Text;
            string tem = s3 + ".org";
            if (File.Exists(s1) == false)
            {
                if (checkBox9.Checked)
                {
                    MainForm.ActiveForm.TopMost = false;
                    MessageBox.Show("原版文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MainForm.ActiveForm.TopMost = true;
                }
                else
                {
                    MessageBox.Show("原版文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (File.Exists(s2) == false)
            {
                if (checkBox9.Checked)
                {
                    MainForm.ActiveForm.TopMost = false;
                    MessageBox.Show("译版文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MainForm.ActiveForm.TopMost = true;
                }
                else
                {
                    MessageBox.Show("译版文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (File.Exists(s3) == false)
            {
                if (checkBox9.Checked)
                {
                    MainForm.ActiveForm.TopMost = false;
                    MessageBox.Show("新版文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MainForm.ActiveForm.TopMost = true;
                }
                else
                {
                    MessageBox.Show("新版文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                label1.Enabled = false;
                label2.Enabled = false;
                label3.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;
                button8.Enabled = false;
                button9.Enabled = false;
                checkBox1.Enabled = false;
                groupBox1.Enabled = false;
                if (checkBox1.Checked == true)
                {
                    if (File.Exists(tem) == false)
                    {
                        File.Copy(s3, tem);
                    }
                }
                alorgid.Clear();
                alorgoff.Clear();
                alorgsize.Clear();
                altraid.Clear();
                altraoff.Clear();
                altrasize.Clear();
                alnewid.Clear();
                alnewoff.Clear();
                alnewsize.Clear();
                backgroundWorker1.RunWorkerAsync();
            }
        }

        void Readorg(string s)
        {
            FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(60, SeekOrigin.Begin);//寻找PE标识
            uint u = br.ReadUInt32();//PE标识位置
            fs.Seek(u + 4, SeekOrigin.Begin);//CPU 类型
            int i1 = br.ReadUInt16();//读取 CPU 类型
            fs.Seek(u + 6, SeekOrigin.Begin);//读出文件段数
            int i2 = br.ReadUInt16();//文件段数
            if (i1 == 332)
            {
                fs.Seek(u + 136, SeekOrigin.Begin);//资源段
            }
            else
            {
                fs.Seek(u + 152, SeekOrigin.Begin);//资源段
            }
            int rsadress = br.ReadInt32();//资源段虚拟地址
            int VR = 0;
            if (rsadress > 0)
            {
                if (i1 == 332)
                {
                    fs.Seek(u + 248, SeekOrigin.Begin);//各个段
                }
                else
                {
                    fs.Seek(u + 264, SeekOrigin.Begin);//各个段
                }
                for (int i = 0; i < i2; i++)
                {
                    fs.Seek(12, SeekOrigin.Current);
                    if (rsadress == br.ReadInt32())
                    {
                        fs.Seek(4, SeekOrigin.Current);
                        VR = rsadress;
                        rsadress = br.ReadInt32();
                        VR = VR - rsadress;
                        break;
                    }
                    fs.Seek(24, SeekOrigin.Current);
                }
                fs.Seek(rsadress + 12, SeekOrigin.Begin);
                int r1, r2, r3;
                int id1, id2, id3, offset1, offset2;
                r1 = br.ReadUInt16() + br.ReadUInt16();
                bool bl = false;
                for (int i = 0; i < r1; i++)
                {
                    fs.Seek(8 * i + rsadress + 16, SeekOrigin.Begin);
                    id1 = br.ReadUInt16();
                    fs.Seek(2, SeekOrigin.Current);
                    offset1 = br.ReadUInt16();
                    bl = false;
                    if (checkBox2.Checked == true && id1 == 2)
                    {
                        bl = true;
                    }
                    else if (checkBox3.Checked == true && id1 == 4)
                    {
                        bl = true;
                    }
                    else if (checkBox4.Checked == true && id1 == 5)
                    {
                        bl = true;
                    }
                    else if (checkBox5.Checked == true && id1 == 6)
                    {
                        bl = true;
                    }
                    else if (checkBox6.Checked == true && id1 == 10)
                    {
                        bl = true;
                    }
                    else if (checkBox7.Checked == true && id1 == 11)
                    {
                        bl = true;
                    }
                    else if (checkBox8.Checked == true)
                    {
                        bl = true;
                    }
                    if (bl == true)
                    {
                        fs.Seek(rsadress + offset1 + 12, SeekOrigin.Begin);
                        r2 = br.ReadUInt16() + br.ReadUInt16();
                        for (int x = 0; x < r2; x++)
                        {
                            fs.Seek(8 * x + rsadress + offset1 + 16, SeekOrigin.Begin);
                            id2 = br.ReadUInt16();
                            fs.Seek(2, SeekOrigin.Current);
                            offset2 = br.ReadUInt16();
                            fs.Seek(rsadress + offset2 + 12, SeekOrigin.Begin);
                            r3 = br.ReadUInt16() + br.ReadUInt16();
                            if (r3 == 1)
                            {
                                fs.Seek(4, SeekOrigin.Current);
                                fs.Seek(rsadress + br.ReadInt32(), SeekOrigin.Begin);
                                if (id1 < 10)
                                {
                                    alorgid.Add("0" + id1.ToString() + id2.ToString());
                                }
                                else
                                {
                                    alorgid.Add(id1.ToString() + id2.ToString());
                                }
                                alorgoff.Add((br.ReadInt32() - VR).ToString());
                                alorgsize.Add(br.ReadInt32().ToString());
                            }
                            else
                            {
                                for (int y = 0; y < r3; y++)
                                {
                                    fs.Seek(8 * y + rsadress + offset2 + 16, SeekOrigin.Begin);
                                    id3 = br.ReadInt32();
                                    fs.Seek(rsadress + br.ReadInt32(), SeekOrigin.Begin);
                                    if (id1 < 10)
                                    {
                                        alorgid.Add("0" + id1.ToString() + id2.ToString() + id3.ToString());
                                    }
                                    else
                                    {
                                        alorgid.Add(id1.ToString() + id2.ToString() + id3.ToString());
                                    }
                                    alorgoff.Add((br.ReadInt32() - VR).ToString());
                                    alorgsize.Add(br.ReadInt32().ToString());
                                }
                            }
                        }
                    }
                }
            }
            br.Close();
            fs.Close();
        }

        void Readtra(string s)
        {
            FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(60, SeekOrigin.Begin);//寻找PE标识
            uint u = br.ReadUInt32();//PE标识位置
            fs.Seek(u + 4, SeekOrigin.Begin);//CPU 类型
            int i1 = br.ReadUInt16();//读取 CPU 类型
            fs.Seek(u + 6, SeekOrigin.Begin);//读出文件段数
            int i2 = br.ReadUInt16();//文件段数
            if (i1 == 332)
            {
                fs.Seek(u + 136, SeekOrigin.Begin);//资源段
            }
            else
            {
                fs.Seek(u + 152, SeekOrigin.Begin);//资源段
            }
            int rsadress = br.ReadInt32();//资源段虚拟地址
            int VR = 0;
            if (rsadress > 0)
            {
                if (i1 == 332)
                {
                    fs.Seek(u + 248, SeekOrigin.Begin);//各个段
                }
                else
                {
                    fs.Seek(u + 264, SeekOrigin.Begin);//各个段
                }
                for (int i = 0; i < i2; i++)
                {
                    fs.Seek(12, SeekOrigin.Current);
                    if (rsadress == br.ReadInt32())
                    {
                        fs.Seek(4, SeekOrigin.Current);
                        VR = rsadress;
                        rsadress = br.ReadInt32();
                        VR = VR - rsadress;
                        break;
                    }
                    fs.Seek(24, SeekOrigin.Current);
                }
                fs.Seek(rsadress + 12, SeekOrigin.Begin);
                int r1, r2, r3;
                int id1, id2, id3, offset1, offset2;
                r1 = br.ReadUInt16() + br.ReadUInt16();
                bool bl = false;
                for (int i = 0; i < r1; i++)
                {
                    fs.Seek(8 * i + rsadress + 16, SeekOrigin.Begin);
                    id1 = br.ReadUInt16();
                    fs.Seek(2, SeekOrigin.Current);
                    offset1 = br.ReadUInt16();
                    bl = false;
                    if (checkBox2.Checked == true && id1 == 2)
                    {
                        bl = true;
                    }
                    else if (checkBox3.Checked == true && id1 == 4)
                    {
                        bl = true;
                    }
                    else if (checkBox4.Checked == true && id1 == 5)
                    {
                        bl = true;
                    }
                    else if (checkBox5.Checked == true && id1 == 6)
                    {
                        bl = true;
                    }
                    else if (checkBox6.Checked == true && id1 == 10)
                    {
                        bl = true;
                    }
                    else if (checkBox7.Checked == true && id1 == 11)
                    {
                        bl = true;
                    }
                    else if (checkBox8.Checked == true)
                    {
                        bl = true;
                    }
                    if (bl == true)
                    {
                        fs.Seek(rsadress + offset1 + 12, SeekOrigin.Begin);
                        r2 = br.ReadUInt16() + br.ReadUInt16();
                        for (int x = 0; x < r2; x++)
                        {
                            fs.Seek(8 * x + rsadress + offset1 + 16, SeekOrigin.Begin);
                            id2 = br.ReadUInt16();
                            fs.Seek(2, SeekOrigin.Current);
                            offset2 = br.ReadUInt16();
                            fs.Seek(rsadress + offset2 + 12, SeekOrigin.Begin);
                            r3 = br.ReadUInt16() + br.ReadUInt16();
                            if (r3 == 1)
                            {
                                fs.Seek(4, SeekOrigin.Current);
                                fs.Seek(rsadress + br.ReadInt32(), SeekOrigin.Begin);
                                if (id1 < 10)
                                {
                                    altraid.Add("0" + id1.ToString() + id2.ToString());
                                }
                                else
                                {
                                    altraid.Add(id1.ToString() + id2.ToString());
                                }
                                altraoff.Add((br.ReadInt32() - VR).ToString());
                                altrasize.Add(br.ReadInt32().ToString());
                            }
                            else
                            {
                                for (int y = 0; y < r3; y++)
                                {
                                    fs.Seek(8 * y + rsadress + offset2 + 16, SeekOrigin.Begin);
                                    id3 = br.ReadInt32();
                                    fs.Seek(rsadress + br.ReadInt32(), SeekOrigin.Begin);
                                    if (id1 < 10)
                                    {
                                        altraid.Add("0" + id1.ToString() + id2.ToString() + id3.ToString());
                                    }
                                    else
                                    {
                                        altraid.Add(id1.ToString() + id2.ToString() + id3.ToString());
                                    }
                                    altraoff.Add((br.ReadInt32() - VR).ToString());
                                    altrasize.Add(br.ReadInt32().ToString());
                                }
                            }
                        }
                    }
                }
            }
            br.Close();
            fs.Close();
        }

        void Readnew(string s)
        {
            FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(60, SeekOrigin.Begin);//寻找PE标识
            uint u = br.ReadUInt32();//PE标识位置
            fs.Seek(u + 4, SeekOrigin.Begin);//CPU 类型
            int i1 = br.ReadUInt16();//读取 CPU 类型
            fs.Seek(u + 6, SeekOrigin.Begin);//读出文件段数
            int i2 = br.ReadUInt16();//文件段数
            if (i1 == 332)
            {
                fs.Seek(u + 136, SeekOrigin.Begin);//资源段
            }
            else
            {
                fs.Seek(u + 152, SeekOrigin.Begin);//资源段
            }
            int rsadress = br.ReadInt32();//资源段虚拟地址
            int VR = 0;
            if (rsadress > 0)
            {
                if (i1 == 332)
                {
                    fs.Seek(u + 248, SeekOrigin.Begin);//各个段
                }
                else
                {
                    fs.Seek(u + 264, SeekOrigin.Begin);//各个段
                }
                for (int i = 0; i < i2; i++)
                {
                    fs.Seek(12, SeekOrigin.Current);
                    if (rsadress == br.ReadInt32())
                    {
                        fs.Seek(4, SeekOrigin.Current);
                        VR = rsadress;
                        rsadress = br.ReadInt32();
                        VR = VR - rsadress;
                        break;
                    }
                    fs.Seek(24, SeekOrigin.Current);
                }
                fs.Seek(rsadress + 12, SeekOrigin.Begin);
                int r1, r2, r3;
                int id1, id2, id3, offset1, offset2;
                r1 = br.ReadUInt16() + br.ReadUInt16();
                bool bl = false;
                for (int i = 0; i < r1; i++)
                {
                    fs.Seek(8 * i + rsadress + 16, SeekOrigin.Begin);
                    id1 = br.ReadUInt16();
                    fs.Seek(2, SeekOrigin.Current);
                    offset1 = br.ReadUInt16();
                    bl = false;
                    if (checkBox2.Checked == true && id1 == 2)
                    {
                        bl = true;
                    }
                    else if (checkBox3.Checked == true && id1 == 4)
                    {
                        bl = true;
                    }
                    else if (checkBox4.Checked == true && id1 == 5)
                    {
                        bl = true;
                    }
                    else if (checkBox5.Checked == true && id1 == 6)
                    {
                        bl = true;
                    }
                    else if (checkBox6.Checked == true && id1 == 10)
                    {
                        bl = true;
                    }
                    else if (checkBox7.Checked == true && id1 == 11)
                    {
                        bl = true;
                    }
                    else if (checkBox8.Checked == true)
                    {
                        bl = true;
                    }
                    if (bl == true)
                    {
                        fs.Seek(rsadress + offset1 + 12, SeekOrigin.Begin);
                        r2 = br.ReadUInt16() + br.ReadUInt16();
                        for (int x = 0; x < r2; x++)
                        {
                            fs.Seek(8 * x + rsadress + offset1 + 16, SeekOrigin.Begin);
                            id2 = br.ReadUInt16();
                            fs.Seek(2, SeekOrigin.Current);
                            offset2 = br.ReadUInt16();
                            fs.Seek(rsadress + offset2 + 12, SeekOrigin.Begin);
                            r3 = br.ReadUInt16() + br.ReadUInt16();
                            if (r3 == 1)
                            {
                                fs.Seek(4, SeekOrigin.Current);
                                fs.Seek(rsadress + br.ReadInt32(), SeekOrigin.Begin);
                                if (id1 < 10)
                                {
                                    alnewid.Add("0" + id1.ToString() + id2.ToString());
                                }
                                else
                                {
                                    alnewid.Add(id1.ToString() + id2.ToString());
                                }
                                alnewoff.Add((br.ReadInt32() - VR).ToString());
                                alnewsize.Add(br.ReadInt32().ToString());
                            }
                            else
                            {
                                for (int y = 0; y < r3; y++)
                                {
                                    fs.Seek(8 * y + rsadress + offset2 + 16, SeekOrigin.Begin);
                                    id3 = br.ReadInt32();
                                    fs.Seek(rsadress + br.ReadInt32(), SeekOrigin.Begin);
                                    if (id1 < 10)
                                    {
                                        alnewid.Add("0" + id1.ToString() + id2.ToString() + id3.ToString());
                                    }
                                    else
                                    {
                                        alnewid.Add(id1.ToString() + id2.ToString() + id3.ToString());
                                    }
                                    alnewoff.Add((br.ReadInt32() - VR).ToString());
                                    alnewsize.Add(br.ReadInt32().ToString());
                                }
                            }
                        }
                    }
                }
            }
            br.Close();
            fs.Close();
        }
        
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string s1 = textBox1.Text;
            string s2 = textBox2.Text;
            string s3 = textBox3.Text;
            Readorg(s1);
            Readtra(s2);
            Readnew(s3);
            if (alorgid.Count == 0)
            {
                if (checkBox9.Checked)
                {
                    MainForm.ActiveForm.TopMost = false;
                    this.Invoke(new Action(delegate
                    {
                        MessageBox.Show("没有搜索到任何资源内容。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                    MainForm.ActiveForm.TopMost = true;
                }
                else
                {
                    this.Invoke(new Action(delegate
                    {
                        MessageBox.Show("没有搜索到任何资源内容。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                }
            }
            else
            {
                for (int i = alorgid.Count - 1; i >= 0; i--)
                {
                    for (int x = altraid.Count - 1; x >= 0; x--)
                    {
                        if (alorgid[i].ToString() == altraid[x].ToString() && int.Parse(alorgsize[i].ToString()) < int.Parse(altrasize[x].ToString()))
                        {
                            alorgid.RemoveAt(i);
                            alorgoff.RemoveAt(i);
                            alorgsize.RemoveAt(i);
                            altraid.RemoveAt(x);
                            altraoff.RemoveAt(x);
                            altrasize.RemoveAt(x);
                            break;
                        }
                    }
                }
                FileStream fs1 = new FileStream(s1, FileMode.Open, FileAccess.Read);
                FileStream fs2 = new FileStream(s2, FileMode.Open, FileAccess.Read);
                FileStream fs3 = new FileStream(s3, FileMode.Open, FileAccess.ReadWrite);
                bool bl1 = true;
                bool bl2 = true;
                int i1 = 0;
                progressBar1.Maximum = alorgid.Count;
                for (int i = 0; i < alorgid.Count; i++)
                {
                    progressBar1.Value = i;
                    for (int x = 0; x < alnewid.Count; x++)
                    {
                        if (alorgid[i].ToString().Substring(0, 2) == alnewid[x].ToString().Substring(0, 2) && alorgsize[i].ToString() == alnewsize[x].ToString())
                        {
                            bl1 = true;
                            fs1.Seek(int.Parse(alorgoff[i].ToString()), SeekOrigin.Begin);
                            fs3.Seek(int.Parse(alnewoff[x].ToString()), SeekOrigin.Begin);
                            for (int y = 0; y < int.Parse(alorgsize[i].ToString()); y++)
                            {
                                if (fs1.ReadByte() != fs3.ReadByte())
                                {
                                    bl1 = false;
                                    break;
                                }
                            }
                            if (bl1 == true)
                            {
                                for (int z = 0; z < altraid.Count; z++)
                                {
                                    if (alorgid[i].ToString() == altraid[z].ToString())
                                    {
                                        fs1.Seek(int.Parse(alorgoff[i].ToString()), SeekOrigin.Begin);
                                        fs2.Seek(int.Parse(altraoff[z].ToString()), SeekOrigin.Begin);
                                        for (int m = 0; m < int.Parse(alorgsize[i].ToString()); m++)
                                        {
                                            if (fs1.ReadByte() != fs2.ReadByte())
                                            {
                                                bl2 = false;
                                                i1 = z;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (bl2 == false)
                                {
                                    bl2 = true;
                                    fs3.Seek(int.Parse(alnewoff[x].ToString()), SeekOrigin.Begin);
                                    for (int m = 0; m < int.Parse(alnewsize[x].ToString()); m++)
                                    {
                                        fs3.WriteByte(0);
                                    }
                                    fs2.Seek(int.Parse(altraoff[i1].ToString()), SeekOrigin.Begin);
                                    fs3.Seek(int.Parse(alnewoff[x].ToString()), SeekOrigin.Begin);
                                    for (int m = 0; m < int.Parse(altrasize[i1].ToString()); m++)
                                    {
                                        fs3.WriteByte((byte)fs2.ReadByte());
                                    }
                                }
                            }
                        }
                    }
                }
                fs1.Close();
                fs2.Close();
                fs3.Close();
                progressBar1.Value = progressBar1.Maximum;
                if (checkBox9.Checked)
                {
                    MainForm.ActiveForm.TopMost = false;
                    this.Invoke(new Action(delegate
                    {
                        MessageBox.Show("处理完成。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                    MainForm.ActiveForm.TopMost = true;
                }
                else
                {
                    this.Invoke(new Action(delegate
                    {
                        MessageBox.Show("处理完成。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 0;
            label1.Enabled = true;
            label2.Enabled = true;
            label3.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;
            checkBox1.Enabled = true;
            groupBox1.Enabled = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            checkBox6.Checked = false;
            checkBox7.Checked = false;
            checkBox8.Checked = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = true;
            checkBox3.Checked = true;
            checkBox4.Checked = true;
            checkBox5.Checked = true;
            checkBox6.Checked = true;
            checkBox7.Checked = true;
            checkBox8.Checked = true;
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            OrgFile((string)Clipboard.GetData(DataFormats.Text));
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                Clipboard.Clear();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length == 1)
                {
                    string s1 = filenames[0];
                    Clipboard.SetDataObject(filenames[0]);
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            TraFile((string)Clipboard.GetData(DataFormats.Text));
        }

        private void textBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                Clipboard.Clear();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length == 1)
                {
                    string s1 = filenames[0];
                    Clipboard.SetDataObject(filenames[0]);
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void textBox3_DragDrop(object sender, DragEventArgs e)
        {
            NewFile((string)Clipboard.GetData(DataFormats.Text));
        }

        private void textBox3_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                Clipboard.Clear();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length == 1)
                {
                    string s1 = filenames[0];
                    Clipboard.SetDataObject(filenames[0]);
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                MainForm.ActiveForm.TopMost = true;
            }
            else
            {
                MainForm.ActiveForm.TopMost = false;
            }
        }
    }
}
