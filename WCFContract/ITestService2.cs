using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCFModel;

namespace WCFContract
{
    [ServiceContract]
    public interface ITestService2
    {
        /// <summary>
        /// 大数据量测试
        /// </summary>
        [OperationContract]
        List<TestData> GetBigData(string code, string name);
    }
}
