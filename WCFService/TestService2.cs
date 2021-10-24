using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCFCommon;
using WCFContract;
using WCFModel;

namespace WCFService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class TestService2 : ITestService2, IService
    {
        public void OnStart()
        {

        }

        public void OnStop()
        {

        }

        public List<TestData> GetBigData(string code, string name)
        {
            List<TestData> list = new List<TestData>();

            for (int i = 0; i < 200000; i++)
            {
                TestData data = new TestData();
                data.Code = code;
                data.Name = name;
                list.Add(data);
            }

            return list;
        }

        public string PutBigData(string bigData)
        {
            return bigData;
        }

        public List<TestData> PutBigDataList(List<TestData> list)
        {
            return list;
        }
    }
}
