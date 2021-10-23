using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WCFContract;
using WCFModel;
using WCFProxy;

namespace WCFClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Log
        private void Log(string log)
        {
            if (!this.IsDisposed)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        textBox1.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + " " + log + "\r\n\r\n");
                    }));
                }
                else
                {
                    textBox1.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + " " + log + "\r\n\r\n");
                }
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            int num = 0;
            string msg;
            TestData data = PF.Get<ITestService>().GetData("001", "测试001", ref num, out msg);
            Log(data.Code + ", " + data.Name + ", " + num.ToString() + ", " + msg);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<TestData> list = PF.Get<ITestService2>().GetBigData("001", "测试001");
            if (list != null)
            {
                Log("count=" + list.Count().ToString());
            }
            else
            {
                Log("list为null");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string result = PF.Get<ITestService>().TestError();
            if (result == null)
            {
                Log("result为空");
            }
            else
            {
                Log(result);
            }
        }
    }
}
