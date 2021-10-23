using Castle.DynamicProxy;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace WCFProxy
{
    /// <summary>
    /// 拦截器
    /// </summary>
    /// <typeparam name="T">接口</typeparam>
    public class ProxyInterceptor<T> : IInterceptor
    {
        private ChannelFactory<T> _channelFactory;

        public ProxyInterceptor(ChannelFactory<T> channelFactory)
        {
            _channelFactory = channelFactory;
        }

        /// <summary>
        /// 拦截方法
        /// </summary>
        public void Intercept(IInvocation invocation)
        {
            //准备参数
            ParameterInfo[] parameterInfoArr = invocation.Method.GetParameters();
            object[] valArr = new object[parameterInfoArr.Length];
            for (int i = 0; i < parameterInfoArr.Length; i++)
            {
                valArr[i] = invocation.GetArgumentValue(i);
            }

            //执行方法
            T server = _channelFactory.CreateChannel();
            using (OperationContextScope scope = new OperationContextScope(server as IContextChannel))
            {
                try
                {
                    //此处添加token

                    invocation.ReturnValue = invocation.Method.Invoke(server, valArr);

                    ((IChannel)server).Close();
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex, "ProxyInterceptor " + typeof(T).Name + " " + invocation.Method.Name + " 异常");
                    ((IChannel)server).Abort();
                }
            }

            //out和ref参数处理
            for (int i = 0; i < parameterInfoArr.Length; i++)
            {
                ParameterInfo paramInfo = parameterInfoArr[i];
                if (paramInfo.IsOut || paramInfo.ParameterType.IsByRef)
                {
                    invocation.SetArgumentValue(i, valArr[i]);
                }
            }
        }
    }
}
