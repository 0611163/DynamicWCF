using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFServerProxy
{
    public class MyServiceHostFactory
    {
        /// <summary>
        /// 创建ServiceHost
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="baseAddresses">服务地址</param>
        /// <param name="contractAssembly">WCF服务契约程序集</param>
        /// <param name="implAssembly">业务接口程序集，可以和WCF服务契约程序集是同一个程序集，也可以不是同一个程序集</param>
        /// <param name="contractNamespace">WCF服务契约命名空间</param>
        /// <param name="implNamespace">业务接口命名空间，可以和WCF服务契约命名空间是同一个命名空间，也可以不是同一个命名空间</param>
        /// <returns></returns>
        public ServiceHost CreateServiceHost(string serviceName, Uri[] baseAddresses, Assembly contractAssembly, Assembly implAssembly, string contractNamespace, string implNamespace)
        {
            Type contractInterfaceType = contractAssembly.GetType(contractNamespace + ".I" + serviceName);
            Type implInterfaceType = implAssembly.GetType(implNamespace + ".I" + serviceName);
            if (contractInterfaceType != null && implInterfaceType != null)
            {
                var proxy = ProxyFactory.CreateProxy(contractInterfaceType, implInterfaceType);
                ServiceHost host = new ServiceHost(proxy, baseAddresses);
                return host;
            }
            else
            {
                return null;
            }
        }
    }
}
