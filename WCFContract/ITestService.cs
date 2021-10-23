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
    public interface ITestService
    {
        /// <summary>
        /// 基本测试，以及ref和out参数测试
        /// </summary>
        [OperationContract]
        TestData GetData(string code, string name, ref int num, out string msg);

        /// <summary>
        /// 测试服务端异常
        /// </summary>
        [OperationContract]
        void TestError();
    }

}
