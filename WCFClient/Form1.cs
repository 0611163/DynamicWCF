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

        // 基本测试，以及ref和out参数测试
        private void button1_Click(object sender, EventArgs e)
        {
            int num = 0;
            string msg;
            TestData data = PF.Get<ITestService>().GetData("001", "测试001", ref num, out msg);
            if (data != null)
            {
                Log("返回内容：" + data.Code + ", " + data.Name + ", " + num.ToString() + ", " + msg);
            }
            else
            {
                Log("返回值data为null");
            }
        }

        // 测试返回大数据量
        private void button2_Click(object sender, EventArgs e)
        {
            List<TestData> list = PF.Get<ITestService2>().GetBigData("001", "测试001");
            if (list != null)
            {
                Log("count=" + list.Count().ToString());
            }
            else
            {
                Log("返回值list为null");
            }
        }

        // 测试服务端异常
        private void button3_Click(object sender, EventArgs e)
        {
            string result = PF.Get<ITestService>().TestError();
            if (result == null)
            {
                Log("返回值result为空");
            }
            else
            {
                Log("返回内容：" + result);
            }
        }

        //测试客户端向服务端传输大数据量
        private void button4_Click(object sender, EventArgs e)
        {
            StringBuilder data = new StringBuilder();
            for (int i = 0; i < 200000; i++)
            {
                data.Append("测试" + i);
            }

            string result = PF.Get<ITestService2>().PutBigData(data.ToString());
            if (result == null)
            {
                Log("返回值result为空");
            }
            else
            {
                Log("返回内容：" + result.Substring(0, 10) + "...");
            }
        }

        //测试客户端向服务端传输大集合数据
        private void button5_Click(object sender, EventArgs e)
        {
            List<TestData> list = new List<TestData>();

            for (int i = 0; i < 200000; i++)
            {
                TestData data = new TestData();
                data.Code = i.ToString();
                data.Name = "测试" + i.ToString();
                list.Add(data);
            }

            List<TestData> result = PF.Get<ITestService2>().PutBigDataList(list);

            if (result != null)
            {
                Log("count=" + result.Count().ToString());
            }
            else
            {
                Log("返回值result为null");
            }
        }
    }
}
