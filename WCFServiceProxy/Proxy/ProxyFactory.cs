using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace WCFServiceProxy
{
    /// <summary>
    /// 动态代理工厂
    /// </summary>
    public class ProxyFactory
    {
        /// <summary>
        /// 拦截器缓存
        /// </summary>
        private static ConcurrentDictionary<Type, IInterceptor> _interceptors = new ConcurrentDictionary<Type, IInterceptor>();

        /// <summary>
        /// 代理对象缓存
        /// </summary>
        private static ConcurrentDictionary<Type, object> _objs = new ConcurrentDictionary<Type, object>();

        private static ProxyGenerator _proxyGenerator;

        private static ModuleScope _scope;

        private static ProxyGenerationOptions _options;

        static ProxyFactory()
        {
            AttributesToAvoidReplicating.Add(typeof(ServiceContractAttribute)); //动态代理类不继承接口的ServiceContractAttribute

            String path = AppDomain.CurrentDomain.BaseDirectory;

            _scope = new ModuleScope(true, false,
                ModuleScope.DEFAULT_ASSEMBLY_NAME,
                Path.Combine(path, ModuleScope.DEFAULT_FILE_NAME),
                "MyDynamicProxy.Proxies",
                Path.Combine(path, "MyDymamicProxy.Proxies.dll"));
            var builder = new DefaultProxyBuilder(_scope);

            _options = new ProxyGenerationOptions();

            //给动态代理类添加AspNetCompatibilityRequirementsAttribute属性
            PropertyInfo proInfoAspNet = typeof(AspNetCompatibilityRequirementsAttribute).GetProperty("RequirementsMode");
            CustomAttributeInfo customAttributeInfo = new CustomAttributeInfo(typeof(AspNetCompatibilityRequirementsAttribute).GetConstructor(new Type[0]), new object[0], new PropertyInfo[] { proInfoAspNet }, new object[] { AspNetCompatibilityRequirementsMode.Allowed });
            _options.AdditionalAttributes.Add(customAttributeInfo);

            //给动态代理类添加ServiceBehaviorAttribute属性
            PropertyInfo proInfoInstanceContextMode = typeof(ServiceBehaviorAttribute).GetProperty("InstanceContextMode");
            PropertyInfo proInfoConcurrencyMode = typeof(ServiceBehaviorAttribute).GetProperty("ConcurrencyMode");
            customAttributeInfo = new CustomAttributeInfo(typeof(ServiceBehaviorAttribute).GetConstructor(new Type[0]), new object[0], new PropertyInfo[] { proInfoInstanceContextMode, proInfoConcurrencyMode }, new object[] { InstanceContextMode.Single, ConcurrencyMode.Multiple });
            _options.AdditionalAttributes.Add(customAttributeInfo);

            _proxyGenerator = new ProxyGenerator(builder);
        }

        /// <summary>
        /// 动态创建代理
        /// </summary>
        public static object CreateProxy(Type contractInterfaceType, Type impInterfaceType)
        {
            IInterceptor interceptor = _interceptors.GetOrAdd(impInterfaceType, type =>
            {
                object _impl = ServiceHelper.Get(impInterfaceType);
                return new ProxyInterceptor(_impl);
            });

            return _objs.GetOrAdd(contractInterfaceType, type => _proxyGenerator.CreateInterfaceProxyWithoutTarget(contractInterfaceType, _options, interceptor)); //根据接口类型动态创建代理对象，接口没有实现类
        }

        /// <summary>
        /// 保存动态代理dll
        /// </summary>
        public static void Save()
        {
            string filePath = Path.Combine(_scope.WeakNamedModuleDirectory, _scope.WeakNamedModuleName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            _scope.SaveAssembly(false);
        }
    }
}
