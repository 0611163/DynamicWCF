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

            //执行方法
            try
            {
                if (true) //token校验
                {
                    Type implType = _impl.GetType();
                    MethodInfo methodInfo = implType.GetMethod(invocation.Method.Name);
                    invocation.ReturnValue = methodInfo.Invoke(_impl, valArr);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex, "ProxyInterceptor " + invocation.Method.DeclaringType.FullName + " " + invocation.Method.Name + " 异常");
                throw ex; //把异常扔到客户端
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
