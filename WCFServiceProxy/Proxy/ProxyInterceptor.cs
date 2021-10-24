using Castle.DynamicProxy;
using WCFCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace WCFServiceProxy
{
    /// <summary>
    /// 拦截器
    /// </summary>
    public class ProxyInterceptor : IInterceptor
    {
        private object _impl;

        public ProxyInterceptor(object impl)
        {
            _impl = impl;
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

            if (true) //token校验
            {
                Type implType = _impl.GetType();
                MethodInfo methodInfo = implType.GetMethod(invocation.Method.Name);
                invocation.ReturnValue = methodInfo.Invoke(_impl, valArr);
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
