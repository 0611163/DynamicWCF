using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
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
                        CreateHost(port, type.Name);
                        break;
                    }
                }
            }



            Console.WriteLine("服务成功启动");
            Console.Read();
        }

        private static void CreateHost(string port, string serviceName)
        {
            string url = string.Format("http://localhost:{0}/Service/{1}", port, serviceName);
            Uri[] uri = new Uri[] { new Uri(url) };

            MyServiceHostFactory factory = new MyServiceHostFactory();
            ServiceHost host = factory.CreateServiceHost(serviceName, uri);

            // 把自定义的IEndPointBehavior插入到终结点中
            foreach (var endpont in host.Description.Endpoints)
            {
                endpont.EndpointBehaviors.Add(new MyEndPointBehavior());
            }

            host.Open();
        }

    }
}
