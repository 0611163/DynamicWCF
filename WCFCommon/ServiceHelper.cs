using Autofac;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFCommon
{
    public class ServiceHelper
    {
        private static IContainer _container;

        /// <summary>
        /// 服务接口集合
        /// </summary>
        private static List<Type> _serviceInterfaces = new List<Type>();

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
        /// <param name="serviceAssembly">服务程序集</param>
        public static void RegisterAssembly(Assembly serviceAssembly)
        {
            Type[] typeArr = serviceAssembly.GetTypes();
            ContainerBuilder builder = new ContainerBuilder();

            foreach (Type type in typeArr)
            {
                Type[] interfaceTypes = type.GetInterfaces();
                foreach (Type interfaceType in interfaceTypes)
                {
                    if (interfaceType.GetCustomAttribute<RegisterServiceAttribute>() != null)
                    {
                        builder.RegisterType(type).SingleInstance().AsImplementedInterfaces();
                        _serviceInterfaces.Add(interfaceType);
                        break;
                    }
                }
            }

            _container = builder.Build();
        }

        #region 启动所有服务
        /// <summary>
        /// 启动所有服务
        /// </summary>
        public static Task StartAllService()
        {
            return Task.Run(() =>
            {
                List<Task> taskList = new List<Task>();
                foreach (Type t in _serviceInterfaces)
                {
                    Task task = Task.Factory.StartNew(obj =>
                    {
                        Type _serviceInterfaceType = obj as Type;
                        IService service = _container.Resolve(_serviceInterfaceType) as IService;

                        if (service != null)
                        {
                            try
                            {
                                service.OnServiceStart();
                                LogUtil.Info("服务 " + _serviceInterfaceType.FullName + " 已启动");
                            }
                            catch (Exception ex)
                            {
                                LogUtil.Error(ex, "服务 " + _serviceInterfaceType.FullName + " 启动失败");
                            }
                        }
                    }, t);
                    taskList.Add(task);
                }
                Task.WaitAll(taskList.ToArray());
            });
        }
        #endregion

        #region 停止所有服务
        /// <summary>
        /// 停止所有服务
        /// </summary>
        public static Task StopAllService()
        {
            return Task.Run(() =>
            {
                List<Task> taskList = new List<Task>();
                Type iServiceInterfaceType = typeof(IService);
                foreach (Type t in _serviceInterfaces)
                {
                    Task task = Task.Factory.StartNew(obj =>
                    {
                        if (iServiceInterfaceType.IsAssignableFrom(obj.GetType()))
                        {
                            Type _serviceInterfaceType = obj as Type;
                            IService service = _container.Resolve(_serviceInterfaceType) as IService;

                            if (service != null)
                            {
                                try
                                {
                                    service.OnServiceStop();
                                    LogUtil.Info("服务 " + _serviceInterfaceType.FullName + " 已停止");
                                }
                                catch (Exception ex)
                                {
                                    LogUtil.Error(ex, "服务 " + _serviceInterfaceType.FullName + " 停止失败");
                                }
                            }
                        }
                    }, t);
                    taskList.Add(task);
                }
                Task.WaitAll(taskList.ToArray());
            });
        }
        #endregion

    }
}
