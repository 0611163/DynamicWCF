using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFContract;
using WCFModel;

namespace WCFService
{
    public class TestService2 : ITestService2
    {
        public List<TestData> GetBigData(string code, string name)
        {
            List<TestData> list = new List<TestData>();

            for (int i = 0; i < 1000000; i++)
            {
                TestData data = new TestData();
                data.Code = code;
                data.Name = name;
                list.Add(data);
            }

            return list;
        }
    }
}
