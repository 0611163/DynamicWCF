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
    public interface ITest2Imp
    {
        /// <summary>
        /// 测试返回大数据量
        /// </summary>
        List<TestData> GetBigData(string code, string name);

        /// <summary>
        /// 测试客户端向服务端传输大数据量
        /// </summary>
        string PutBigData(string bigData);

        /// <summary>
        /// 测试客户端向服务端传输大集合数据
        /// </summary>
        List<TestData> PutBigDataList(List<TestData> list);
    }
}
