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
[RegisterService]
[ServiceContract]
public interface ITestService
```

说明：为什么要使用RegisterServiceAttribute？是为了兼容旧的WCF服务端和客户端架构，以便改造现有项目，原来的架构不变，为了精简增删改查代码，额外引入该框架。

#### 3.  服务实现类继承IService：

```C#
[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
public class TestService : ITestService, IService
```

#### 4.  程序启动时添加如下代码：

```C#
int serverPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);
Assembly serviceAssembly = Assembly.GetAssembly(typeof(TestService));
Assembly contractAssembly = Assembly.GetAssembly(typeof(ITestService));
string contractNamespace = "WCFContract";

HostFactory.CreateHosts(serverPort, serviceAssembly, contractAssembly, contractNamespace);

ServiceHelper.StartAllService();
```

注意：约定WCF契约接口名称为服务名称前加字母I，例如服务名称为TestService，则WCF契约接口名称为ITestService

### 二、  客户端

客户端使用起来非常简单

#### 1.  引用WCFClientProxy和WCFCommon.dll

#### 2.  使用前初始化PF工厂类：

```C#
PF.Init(ConfigurationManager.AppSettings["WCFServiceAddress"]); //初始化PF
```

#### 3.  使用：

```C#
List<TestData> list = PF.Get<ITestService2>().GetBigData("001", "测试001");
```





