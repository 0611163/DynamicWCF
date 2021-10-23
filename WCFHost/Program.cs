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

            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);

            ServiceBehaviorAttribute sba = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            sba.MaxItemsInObjectGraph = 2147483647;

            BasicHttpBinding wsHttp = new BasicHttpBinding();
            wsHttp.MaxBufferPoolSize = 524288;
            wsHttp.MaxReceivedMessageSize = 2147483647;
            wsHttp.ReaderQuotas.MaxArrayLength = 6553600;
            wsHttp.ReaderQuotas.MaxStringContentLength = 2147483647;
            wsHttp.ReaderQuotas.MaxBytesPerRead = 6553600;
            wsHttp.ReaderQuotas.MaxDepth = 6553600;
            wsHttp.ReaderQuotas.MaxNameTableCharCount = 6553600;
            wsHttp.CloseTimeout = new TimeSpan(0, 1, 0);
            wsHttp.OpenTimeout = new TimeSpan(0, 1, 0);
            wsHttp.ReceiveTimeout = new TimeSpan(0, 10, 0);
            wsHttp.SendTimeout = new TimeSpan(0, 10, 0);
            wsHttp.Security.Mode = BasicHttpSecurityMode.None;

            host.AddServiceEndpoint(interfaceType, wsHttp, "");

            // 把自定义的IEndPointBehavior插入到终结点中
            foreach (var endpont in host.Description.Endpoints)
            {
                endpont.EndpointBehaviors.Add(new MyEndPointBehavior());
            }

            host.Open();
        }

    }
}
