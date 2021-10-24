using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace WCFServerProxy
{
    /// <summary>
    /// WCF服务端消息拦截器
    /// </summary>
    public class MyDispatchMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            if (false) //token验证
            {
                //LogUtil.Info("ticket验证失败");
                //throw new Exception("ticket验证失败");
            }

            //LogUtil.Info("token验证通过");
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {

        }
    }
}
