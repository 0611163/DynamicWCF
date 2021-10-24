using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFCommon;
using WCFModel;

namespace WCFServiceInterface
{
    [RegisterService]
    public interface ITestImp
    {
        /// <summary>
        /// 基本测试，以及ref和out参数测试
        /// </summary>
        TestData GetData(string code, string name, ref int num, out string msg);

        /// <summary>
        /// 基本测试
        /// </summary>
        TestData GetData2(string code, string name);

        /// <summary>
        /// 测试服务端异常
        /// </summary>
        string TestError();
    }

}
