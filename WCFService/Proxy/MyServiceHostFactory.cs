using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCFContract;

namespace WCFService
{
    public class MyServiceHostFactory
    {
        public MyServiceHostFactory()
        {
            ServiceHelper.RegisterAssembly(this.GetType());
        }

        public ServiceHost CreateServiceHost(string reference, Uri[] baseAddresses)
        {
            Assembly contractAssembly = Assembly.GetAssembly(typeof(ITestService));
            Assembly impAssembly = Assembly.GetAssembly(typeof(ITestService));
            Type contractInterfaceType = contractAssembly.GetType("WCFContract.I" + reference);
            Type impInterfaceType = impAssembly.GetType("WCFContract.I" + reference);
            if (contractInterfaceType != null && impInterfaceType != null)
            {
                var proxy = ProxyFactory.CreateProxy(contractInterfaceType, impInterfaceType);
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
