using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Windows.Forms.DataVisualization.Charting;

namespace ZhaoTengWinform.Forms
{
   
    public partial class PIDTest : Form
    {
        //控制参数及控制对象
        double Kp = 0.5;
        double Ki = 0.5;
        double Kd = 0.5;
        noLagError co0 = new noLagError();
        noLagisError co1 = new noLagisError();
        isLagError co2 = new isLagError();

        // 定义一个泛型队列，存储数据点
        int timei = 0; //时间点采样
        Queue<double> Xvalue = new Queue<double>();
        Queue<double> Y0value = new Queue<double>();
        Queue<double> Y1value = new Queue<double>();
        //图表参数
        int LastError = 0;
        int SumError = 0;
        int TargetValue = 20000;
        int NowValue = 10000;

        public PIDTest()
        {
            InitializeComponent();
            this.PIDmode.Items.Add("P");
            this.PIDmode.Items.Add("PI");
            this.PIDmode.Items.Add("PD");
            this.PIDmode.Items.Add("PID");
            this.PIDmode.Text = "P";
            this.ControlObject.Items.Add("无滞后无静态误差");
            this.ControlObject.Items.Add("无滞后有静态误差");
            this.ControlObject.Items.Add("有滞后有静态误差");
            this.ControlObject.Text = "无滞后无静态误差";
            this.textBox1.KeyPress += PIDtextBox_KeyPress;
            this.textBox2.KeyPress += PIDtextBox_KeyPress;
            this.textBox3.KeyPress += PIDtextBox_KeyPress;
            this.textBox1.Text = Kp.ToString();
            this.textBox2.Text = Ki.ToString();
            this.textBox3.Text = Kd.ToString();

        }

        private void PIDtextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是退格键并且不是数字键
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                //如果输入的不是小数点
                if (e.KeyChar != '.')
                {
                    //不处理该事件
                    e.Handled = true;
                }
                else
                {
                    //如果已经存在小数点
                    if ((sender as TextBox).Text.IndexOf('.') >= 1)
                    {
                        //不处理该事件
                        e.Handled = true;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 设置图表的样式和功能
            chart1.Series[0].ChartType = SeriesChartType.Line; // 图表类型为折线图
            chart1.Series[0].Color = System.Drawing.Color.Red; // 线条颜色为红色
            chart1.Series[1].ChartType = SeriesChartType.Line; // 图表类型为折线图
            chart1.Series[1].Color = System.Drawing.Color.Green; // 线条颜色为红色
            chart1.ChartAreas[0].AxisX.Title = "Time"; // X 轴标题为 Time
            chart1.ChartAreas[0].AxisY.Title = "Value"; // Y 轴标题为 Value
            //chart1.ChartAreas[0].AxisY.Minimum = 5000;

            this.Kp = double.Parse(textBox1.Text);
            this.Ki = double.Parse(textBox1.Text);
            this.Kd = double.Parse(textBox1.Text);
            this.timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            this.timer1.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.timer1.Stop();
            this.LastError = 0;
            this.NowValue = 10000;
            this.SumError = 0;
            this.co0.Clear();
            this.co1.Clear();
            this.co2.Clear();
            this.Xvalue.Clear();
            this.Y0value.Clear();
            this.Y1value.Clear();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int controlvalue = 0;
            int NowError = 0;

            timei++;
            Xvalue.Enqueue(timei);
            Y0value.Enqueue(this.NowValue);
            Y1value.Enqueue(this.TargetValue);
            // 移除队列中过旧的数据，保持队列的长度不超过 100
            while (Xvalue.Count > 100)
            {
                Xvalue.Dequeue();
                Y0value.Dequeue();
                Y1value.Dequeue();
            }
            // 使用队列中的数据来更新图表的数据序列
            chart1.Series[0].Points.DataBindXY(Xvalue, Y0value);
            chart1.Series[1].Points.DataBindXY(Xvalue, Y1value);
            // 刷新图表
            chart1.Invalidate();

            switch (this.ControlObject.Text)
            {
                case "无滞后无静态误差":
                    NowError = this.TargetValue - this.co0.RealValue;//计算当前误差
                    break;
                case "无滞后有静态误差":
                    NowError = this.TargetValue - this.co1.RealValue;
                    break;
                case "有滞后有静态误差":
                    NowError = this.TargetValue - this.co2.RealValue;
                    break;
            }
            switch (this.PIDmode.Text)
            {
                case "P":
                    controlvalue = (int)(this.Kp * NowError);
                    break;
                case "PI":
                    this.SumError += NowError;
                    controlvalue = (int)(this.Kp * NowError + this.Ki* this.SumError);
                    break;
                case "PD":
                    controlvalue = (int)(this.Kp * NowError + this.Kd * (NowError - this.LastError));
                    this.LastError = NowError;//保存上一次误差
                    break;
                case "PID":
                    this.SumError += NowError;
                    controlvalue = (int)(this.Kp * NowError + this.Ki * this.SumError + this.Kd * (NowError - this.LastError));
                    this.LastError = NowError;
                    break;
            }
            switch (this.ControlObject.Text)
            {
                case "无滞后无静态误差":
                    this.NowValue = this.co0.Control(controlvalue);
                    break;
                case "无滞后有静态误差":
                    this.NowValue = this.co1.Control(controlvalue);
                    break;
                case "有滞后有静态误差":
                    this.NowValue = this.co2.Control(controlvalue);
                    break;
            }
            
        }

    }

    public class noLagError
    {
        //无滞后无静态误差
        private int _RealValue;
        public noLagError(int initialValue = 10000)
        {
            this._RealValue = initialValue;
        }

        public int RealValue { get => _RealValue; }
        public int Control(int ControlValue)
        {
            this._RealValue += ControlValue;
            return this._RealValue;
        }
        public void Clear()
        {
            this._RealValue = 10000;
        }

    }
    public class noLagisError
    {
        //无滞后有静态误差
        private int _RealValue;
        private int _staticError;
        public noLagisError(int initialValue = 10000, int staticError = -500)
        {
            this._RealValue = initialValue;
            this._staticError = staticError;
        }

        public int RealValue { get => _RealValue; }
        public int Control(int ControlValue)
        {
            this._RealValue += ControlValue + this._staticError;
            return this._RealValue;
        }
        public void Clear()
        {
            this._RealValue = 10000;
        }
    }
    public class isLagError
    {
        //有滞后有静态误差
        private int _RealValue;
        private int _staticError;
        private int _realContraoValue = 0;
        public isLagError(int initialValue = 10000, int staticError = -500)
        {
            this._RealValue = initialValue;
            this._staticError = staticError;
        }

        public int RealValue { get => _RealValue; }
        public int Control(int ControlValue)
        {
            if (Math.Abs(ControlValue - this._realContraoValue) > 1000)
            {
                this._realContraoValue += (ControlValue - this._realContraoValue)/2;
            }
            else
            {
                this._realContraoValue = ControlValue;
            }

            this._RealValue += _realContraoValue + this._staticError;

            return this._RealValue;

        }
        public void Clear()
        {
            this._RealValue = 10000;
            this._realContraoValue = 0;
        }

    }
}
