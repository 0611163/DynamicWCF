using WCFCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFServiceProxy
{
    public class MyServiceHostFactory
    {
        /// <summary>
        /// 创建ServiceHost
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="serviceAddress">服务地址</param>
        /// <param name="contractAssembly">WCF服务契约程序集</param>
        /// <param name="contractNamespace">WCF服务契约命名空间</param>
        /// <returns></returns>
        public ServiceHost CreateServiceHost(string serviceName, Uri[] serviceAddress, Assembly contractAssembly, string contractNamespace)
        {
            Type contractInterfaceType = contractAssembly.GetType(contractNamespace + ".I" + serviceName);
            if (contractInterfaceType != null)
            {
                var proxy = ProxyFactory.CreateProxy(contractInterfaceType);
                ServiceHost host = new ServiceHost(proxy, serviceAddress);
                return host;
            }
            else
            {
                return null;
            }
        }
    }
}
