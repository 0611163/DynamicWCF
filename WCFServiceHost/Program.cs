﻿using System;
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

namespace WCFServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            int serverPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            Assembly serviceAssembly = Assembly.GetAssembly(typeof(TestService));
            Assembly contractAssembly = Assembly.GetAssembly(typeof(ITestService));
            Assembly implAssembly = Assembly.GetAssembly(typeof(ITestService));
            string contractNamespace = "WCFContract";
            string implNamespace = "WCFContract";

            HostFactory.CreateHosts(serverPort, serviceAssembly, contractAssembly, implAssembly, contractNamespace, implNamespace);

            Console.WriteLine("服务成功启动");
            Console.Read();
        }

    }
}