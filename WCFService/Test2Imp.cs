using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCFCommon;
using WCFContract;
using WCFModel;
using WCFServiceInterface;

namespace WCFService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Test2Imp : ITest2Imp, IService
    {
        public void OnServiceStart()
        {

        }

        public void OnServiceStop()
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
