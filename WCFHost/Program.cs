using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using WCFContract;
using WCFService;

namespace WCFHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string port = ConfigurationManager.AppSettings["ServerPort"];

            Assembly assembly = Assembly.Load(Assembly.GetAssembly(typeof(TestService)).FullName);
            Type[] typeArr = assembly.GetTypes();

            foreach (Type type in typeArr)
            {
                Type[] interfaceTypes = type.GetInterfaces();
                foreach (Type interfaceType in interfaceTypes)
                {
                    if (interfaceType.GetCustomAttribute<ServiceContractAttribute>() != null)
                    {
                        CreateHost(port, type.Name, interfaceType);
                        break;
                    }
                }
            }



            Console.WriteLine("服务成功启动");
            Console.Read();
        }

        private static void CreateHost(string port, string serviceName, Type interfaceType)
        {
            string url = string.Format("http://localhost:{0}/Service/{1}", port, serviceName);
            Uri[] uri = new Uri[] { new Uri(url) };

            MyServiceHostFactory factory = new MyServiceHostFactory();
            ServiceHost host = factory.CreateServiceHost(serviceName, uri);

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
