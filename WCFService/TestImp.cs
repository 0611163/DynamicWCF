using WCFCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCFContract;
using WCFModel;
using WCFServiceInterface;

namespace WCFService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class TestImp : ITestImp, IService
    {
        public void OnServiceStart()
        {

        }

        public void OnServiceStop()
        {

        }

        public TestData GetData(string code, string name, ref int num, out string msg)
        {
            TestData data = new TestData();
            data.Code = code;
            data.Name = name;
            num = 999;
            msg = "成功获取数据";
            LogUtil.Info("成功获取数据");
            return data;
        }

        public TestData GetData2(string code, string name)
        {
            TestData data = new TestData();
            data.Code = code;
            data.Name = name;
            return data;
        }

        public string TestError()
        {
            throw new Exception("测试异常 " + DateTime.Now.ToString("HH:mm:ss"));

            return "测试返回";
        }
    }
}
