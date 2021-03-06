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
using WCFServiceProxy;
using WCFService;
using WCFCommon;

namespace WCFServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            int serverPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            Assembly serviceAssembly = Assembly.GetAssembly(typeof(TestService));
            Assembly contractAssembly = Assembly.GetAssembly(typeof(ITestService));
            string contractNamespace = "WCFContract";

            HostFactory.CreateHosts(serverPort, serviceAssembly, contractAssembly, contractNamespace);

            ServiceHelper.StartAllService();

            Console.WriteLine("服务成功启动");
            Console.Read();
        }

    }
}
