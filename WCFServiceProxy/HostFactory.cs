using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using WCFCommon;

namespace WCFServiceProxy
{
    public class HostFactory
    {
        /// <summary>
        /// 创建并启动 WCF Host
        /// </summary>
        /// <param name="serverPort">WCF服务端口</param>
        /// <param name="serviceAssembly">WCF服务实现程序集</param>
        /// <param name="contractAssembly">WCF服务契约程序集</param>
        /// <param name="contractNamespace">WCF服务契约命名空间</param>
        public static void CreateHosts(int serverPort, Assembly serviceAssembly, Assembly contractAssembly, string contractNamespace)
        {
            ServiceHelper.RegisterAssembly(serviceAssembly); //注册WCF服务程序集

            Type[] typeArr = serviceAssembly.GetTypes();

            foreach (Type type in typeArr)
            {
                Type[] interfaceTypes = type.GetInterfaces();
                foreach (Type interfaceType in interfaceTypes)
                {
                    if (interfaceType.GetCustomAttribute<RegisterServiceAttribute>() != null)
                    {
                        CreateHost(serverPort, type.Name, interfaceType, contractAssembly, contractNamespace);
                        break;
                    }
                }
            }

            ProxyFactory.Save();
        }

        private static void CreateHost(int port, string serviceName, Type interfaceType, Assembly contractAssembly, string contractNamespace)
        {
            string url = string.Format("http://localhost:{0}/Service/{1}", port, serviceName);
            Uri[] uri = new Uri[] { new Uri(url) };

            MyServiceHostFactory factory = new MyServiceHostFactory();
            ServiceHost host = factory.CreateServiceHost(serviceName, uri, contractAssembly, contractNamespace);

            ServiceMetadataBehavior serviceMetadataBehavior = new ServiceMetadataBehavior();
            serviceMetadataBehavior.HttpGetEnabled = true;
            host.Description.Behaviors.Add(serviceMetadataBehavior);

            ServiceBehaviorAttribute serviceBehaviorAttribute = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            serviceBehaviorAttribute.MaxItemsInObjectGraph = int.MaxValue;

            ServiceThrottlingBehavior serviceThrottlingBehavior = new ServiceThrottlingBehavior();
            serviceThrottlingBehavior.MaxConcurrentCalls = int.MaxValue;
            serviceThrottlingBehavior.MaxConcurrentSessions = int.MaxValue;
            serviceThrottlingBehavior.MaxConcurrentInstances = int.MaxValue;
            host.Description.Behaviors.Add(serviceThrottlingBehavior);

            DataContractSerializerOperationBehavior dataContractBehavior = host.Description.Behaviors.Find<DataContractSerializerOperationBehavior>();
            if (dataContractBehavior != null)
            {
                dataContractBehavior.MaxItemsInObjectGraph = int.MaxValue;
            }

            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.ReaderQuotas.MaxDepth = int.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
            binding.CloseTimeout = new TimeSpan(0, 1, 0);
            binding.OpenTimeout = new TimeSpan(0, 1, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 1, 0);
            binding.SendTimeout = new TimeSpan(0, 1, 0);
            binding.Security.Mode = BasicHttpSecurityMode.None;

            host.AddServiceEndpoint(interfaceType, binding, "");

            // 把自定义的IEndPointBehavior插入到终结点中
            foreach (var endpont in host.Description.Endpoints)
            {
                endpont.EndpointBehaviors.Add(new MyEndPointBehavior());
            }

            host.Open();
        }
    }
}
