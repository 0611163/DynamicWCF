using Castle.DynamicProxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
                ChannelFactory<T> channelFactory = new ChannelFactory<T>(serviceName);
                return new ProxyInterceptor<T>(channelFactory);
            });

            return (T)_objs.GetOrAdd(interfaceType, type => _proxyGenerator.CreateInterfaceProxyWithoutTarget(interfaceType, interceptor)); //根据接口类型动态创建代理对象，接口没有实现类
        }
    }
}
