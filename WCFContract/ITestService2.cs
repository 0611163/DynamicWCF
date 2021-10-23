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
        /// 测试返回大数据量
        /// </summary>
        [OperationContract]
        List<TestData> GetBigData(string code, string name);

        /// <summary>
        /// 测试客户端向服务端传输大数据量
        /// </summary>
        [OperationContract]
        string PutBigData(string bigData);

        /// <summary>
        /// 测试客户端向服务端传输大集合数据
        /// </summary>
        [OperationContract]
        List<TestData> PutBigDataList(List<TestData> list);
    }
}
