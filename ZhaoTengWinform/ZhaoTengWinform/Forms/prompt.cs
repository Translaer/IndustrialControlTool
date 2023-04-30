using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZhaoTengWinform.Forms
{
    public partial class prompt : Form
    {
        public prompt()
        {
            InitializeComponent();
        }
        public void promptAndShow(string prompttext, uint mode)
        {
            this.label1.Text = prompttext;
            if (mode == 1)
            {
                this.Show();
            }
            else
            {
                this.ShowDialog();
            }
        }
    }
}
