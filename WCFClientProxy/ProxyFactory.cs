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

namespace WCFClientProxy
{
    /// <summary>
    /// WCF服务工厂
    /// PF是ProxyFactory的简写
    /// 使用前请调用PF.Init()初始化
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

        private static string _wcfServiceAddress;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="wcfServiceAddress">WCF服务端地址，例：http://127.0.0.1:8068</param>
        public static void Init(string wcfServiceAddress)
        {
            _wcfServiceAddress = wcfServiceAddress;
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
            string url = Path.Combine(_wcfServiceAddress, "Service", serviceName).Replace("\\", "/");

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
