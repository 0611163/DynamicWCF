# DynamicWCF

## 介绍

动态WCF：使用动态代理精简WCF代码架构


## 软件架构

CS架构


## 引用开源库

1.  Autofac：一个IOC框架
2.  Castle.core：一个AOP框架
3.  NLog：日志框架


## 工程说明

1.  WCFClient：客户端
2.  WCFClientProxy：客户端WCF动态代理
3.  WCFCommon：通用工程，封装日志和IOC功能
4.  WCFContract：WCF契约
5.  WCFModel：实体类
6.  WCFService：WCF服务端实现
7.  WCFServiceProxy：服务端WCF动态代理
8.  WCFServiceHost：WCF服务端宿主


## 使用说明

现有程序框架中使用的话，只需要服务端引入WCFServiceProxy.dll和WCFCommon.dll，客户端引入WCFClientProxy.dll和WCFCommon.dll

### 一、  服务端

#### 1.  引用WCFServiceProxy.dll和WCFCommon.dll

#### 2.  服务契约添加RegisterServiceAttribute：

```C#
[WCFCommon.RegisterService]
[ServiceContract(Namespace = "http://www.suncreate.net/2014/combatplatform", SessionMode = SessionMode.Allowed)]
[ServiceKnownType("GetKnownTypes", typeof(KnownTypeHelper))]
public interface IMyTestService
```

```C#
[WCFCommon.RegisterService]
[ServiceContract(Namespace = "http://www.suncreate.net/2014/combatplatform", SessionMode = SessionMode.Allowed)]
[ServiceKnownType("GetKnownTypes", typeof(KnownTypeHelper))]
public interface IBaseDataService
```

说明：为什么要使用RegisterServiceAttribute？是为了兼容旧的WCF服务端和客户端架构，以便改造现有项目，原来的架构不变，为了精简增删改查代码，额外引入该框架。

#### 3.  服务接口添加RegisterServiceAttribute：

```C#
[WCFCommon.RegisterService]
public interface IMyTestImp
```

```C#
[WCFCommon.RegisterService]
public interface IBaseDataImp
```

#### 4.  服务实现类继承IService：

```C#
public class BaseDataImp : AbstractService, IBaseDataImp, WCFCommon.IService
{
    public void OnServiceStart()
    {
        m_dbSession = HI.Get<IDBSessionImp>();
    }

    public void OnServiceStop()
    {
        m_dbSession = null;
    }
```

```C#
public class MyTestImp : IMyTestImp, WCFCommon.IService
{
    private IDBSessionImp m_dbSession;
    private static ILog m_Log = log4net.LogManager.GetLogger(typeof(BaseDataImp));

    public void OnServiceStart()
    {
        m_dbSession = HI.Get<IDBSessionImp>();
    }

    public void OnServiceStop()
    {
        m_dbSession = null;
    }
```

#### 5.  程序启动时添加如下代码：

```C#
ThreadUtil.Run(() =>
{
    int serverPort = int.Parse(ConfigurationManager.AppSettings["WCFServerPort"]);
    Assembly serviceAssembly = Assembly.GetAssembly(typeof(BaseDataImp));
    Assembly contractAssembly = Assembly.GetAssembly(typeof(IBaseDataService));
    Assembly implAssembly = Assembly.GetAssembly(typeof(IBaseDataImp));
    string contractNamespace = "SunCreate.Vipf.Contract";
    string implNamespace = "SunCreate.Vipf.Server.Bussiness";

    try
    {
        HostFactory.CreateHosts(serverPort, serviceAssembly, contractAssembly, implAssembly, contractNamespace, implNamespace);

        WCFCommon.ServiceHelper.StartAllService();
    }
    catch (Exception ex)
    {
        string str = ex.Message;
    }
});
```

说明：这段代码要在StartRepository();之后，即在SP.CreateProvider().Start();之后，也就是要先启动ORM相关的服务

说明：业务接口程序集implAssembly和WCF服务契约程序集contractAssembly可以是同一个程序集，也可以不是同一个程序集

### 二、  客户端

客户端使用起来非常简单

#### 1.  引用WCFClientProxy和WCFCommon.dll

#### 2.  使用前初始化PF工厂类：

```C#
PF.Init(ConfigurationManager.AppSettings["WCFServiceAddress"]); //初始化PF
```

#### 3.  使用：

```C#
List<VIPF_VIDEO_DEVICE> list = PF.Get<IMyTestService>().GetAllDevice().ToList();
List<VIPF_VIDEO_DEVICE> list2 = PF.Get<IBaseDataService>().GetAllDevice().ToList();
```





