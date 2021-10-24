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
using WCFServiceInterface;

namespace WCFServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            int serverPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            Assembly serviceAssembly = Assembly.GetAssembly(typeof(TestImp));
            Assembly contractAssembly = Assembly.GetAssembly(typeof(ITestService));
            Assembly implAssembly = Assembly.GetAssembly(typeof(ITestImp));
            string contractNamespace = "WCFContract";
            string implNamespace = "WCFServiceInterface";

            HostFactory.CreateHosts(serverPort, serviceAssembly, contractAssembly, implAssembly, contractNamespace, implNamespace);

            ServiceHelper.StartAllService();

            Console.WriteLine("服务成功启动");
            Console.Read();
        }

    }
}
