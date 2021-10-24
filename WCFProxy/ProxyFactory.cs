using Castle.DynamicProxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace WCFProxy
{
    /// <summary>
    /// WCF服务工厂
    /// PF是ProxyFactory的简写
    /// </summary>
    public class PF
    {
        /// <summary>
        /// 拦截器缓存
        /// </summary>
        private static ConcurrentDictionary<Type, IInterceptor> _interceptors = new ConcurrentDictionary<Type, IInterceptor>();

        /// <summary>
        /// 代理对象缓存
        /// </summary>
        private static ConcurrentDictionary<Type, object> _objs = new ConcurrentDictionary<Type, object>();

        private static ProxyGenerator _proxyGenerator = new ProxyGenerator();

        static PF()
        {
            string wcfServiceAddress = ConfigurationManager.AppSettings["WCFServiceAddress"];

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationSectionGroup sectionGroup = config.SectionGroups["system.serviceModel"];
            ServiceModelSectionGroup serviceModelSectionGroup = sectionGroup as ServiceModelSectionGroup;
            ClientSection clientSection = serviceModelSectionGroup.Client;

            foreach (ChannelEndpointElement endpoint in clientSection.Endpoints)
            {
                endpoint.Address = new Uri(Path.Combine(wcfServiceAddress, "Service", endpoint.Name).Replace("\\", "/"));
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("system.serviceModel");
        }

        /// <summary>
        /// 获取WCF服务
        /// </summary>
        /// <typeparam name="T">WCF接口</typeparam>
        public static T Get<T>()
        {
            Type interfaceType = typeof(T);

            IInterceptor interceptor = _interceptors.GetOrAdd(interfaceType, type =>
            {
                string serviceName = interfaceType.Name.Substring(1); //服务名称
                ChannelFactory<T> channelFactory = CreateChannel<T>(serviceName);
                return new ProxyInterceptor<T>(channelFactory);
            });

            return (T)_objs.GetOrAdd(interfaceType, type => _proxyGenerator.CreateInterfaceProxyWithoutTarget(interfaceType, interceptor)); //根据接口类型动态创建代理对象，接口没有实现类
        }

        private static ChannelFactory<T> CreateChannel<T>(string serviceName)
        {
            string wcfServiceAddress = ConfigurationManager.AppSettings["WCFServiceAddress"];
            string url = Path.Combine(wcfServiceAddress, "Service", serviceName).Replace("\\", "/");

            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.ReaderQuotas.MaxDepth = int.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 1, 0);
            binding.SendTimeout = new TimeSpan(0, 1, 0);
            binding.Security.Mode = BasicHttpSecurityMode.None;

            ChannelFactory<T> channelFactory = new ChannelFactory<T>(binding, url);
            foreach (OperationDescription op in channelFactory.Endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dataContractBehavior = op.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;

                if (dataContractBehavior != null)
                {
                    dataContractBehavior.MaxItemsInObjectGraph = int.MaxValue;
                }
            }

            return channelFactory;
        }
    }
}
