using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace WCFService
{
    /// <summary>
    /// WCF服务端异常拦截器
    /// </summary>
    public class MyErrorHandler : IErrorHandler
    {
        public bool HandleError(Exception error)
        {
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            LogUtil.Error(error, "调用WCF接口出错");
        }
    }
}
