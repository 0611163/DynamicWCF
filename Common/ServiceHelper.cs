using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ServiceHelper
    {
        private static IContainer _container;

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        public static T Get<T>()
        {
            return _container.Resolve<T>();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="type">接口类型</param>
        public static object Get(Type type)
        {
            return _container.Resolve(type);
        }

        /// <summary>
        /// 注册程序集
        /// </summary>
        /// <param name="type">程序集中的一个类型</param>
        public static void RegisterAssembly(Type type)
        {
            RegisterAssembly(Assembly.GetAssembly(type).FullName);
        }

        /// <summary>
        /// 注册程序集
        /// </summary>
        /// <param name="assemblyString">程序集名称的长格式</param>
        public static void RegisterAssembly(string assemblyString)
        {
            Assembly assembly = Assembly.Load(assemblyString);
            Type[] typeArr = assembly.GetTypes();
            ContainerBuilder builder = new ContainerBuilder();

            foreach (Type type in typeArr)
            {
                Type[] interfaceTypes = type.GetInterfaces();
                foreach (Type interfaceType in interfaceTypes)
                {
                    if (interfaceType.GetCustomAttribute<ServiceContractAttribute>() != null)
                    {
                        builder.RegisterType(type).AsImplementedInterfaces();
                        break;
                    }
                }
            }

            _container = builder.Build();
        }

    }
}
