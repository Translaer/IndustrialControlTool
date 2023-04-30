using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZhaoTengWinform.Forms;

namespace ZhaoTengWinform
{
    
    public partial class UserLoad : Form
    {
        public UserLoad()
        {
            InitializeComponent();
        }
        private void UserLoad_Load(object sender, EventArgs e)
        {
            foreach (var i in GlobalInformation.Instance.UsersList)
            {
                this.comboBox1.Items.Add(i.Username);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool Fail = true;
            foreach (User user in GlobalInformation.Instance.UsersList)
            {
                if (this.textBox1.Text != null && user.Username == this.comboBox1.Text && user.Password == this.textBox1.Text)
                {
                    this.Hide();
                    // 登录成功
                    var mainForm = new MainForm();
                    mainForm.Show();
                    Fail = false;
                    break;
                }
            }
            //提示密码错误
            if (Fail)
            {
                var prompt0 = new prompt();
                prompt0.promptAndShow("密码错误！", 1);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Clear();
        }
    }
}
