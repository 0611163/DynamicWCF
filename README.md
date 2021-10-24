# DynamicWCF

#### 介绍
动态WCF：使用动态代理精简WCF代码架构


#### 软件架构
CS架构


#### 引用开源库

1.  Autofac：一个IOC框架
2.  Castle.core：一个AOP框架
3.  NLog：日志框架


#### 工程说明

1.  WCFClient：客户端
2.  WCFProxy：客户端WCF动态代理
3.  Common：通用工程，封装日志和IOC功能
4.  WCFContract：WCF接口
5.  WCFModel：实体类
6.  WCFHost：WCF服务端宿主
7.  WCF：WCFService服务端实现


#### 使用说明

1.  服务端：引用WCFServiceProxy.dll和WCFCommon.dll
服务契约添加RegisterServiceAttribute：  
'''  
[RegisterService]
[ServiceContract]
public interface ITestService
'''  
程序启动时添加如下代码：  
'''  
int serverPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);
Assembly serviceAssembly = Assembly.GetAssembly(typeof(TestService));
Assembly contractAssembly = Assembly.GetAssembly(typeof(ITestService));
Assembly implAssembly = Assembly.GetAssembly(typeof(ITestService));
string contractNamespace = "WCFContract";
string implNamespace = "WCFContract";

HostFactory.CreateHosts(serverPort, serviceAssembly, contractAssembly, implAssembly, contractNamespace, implNamespace);

ServiceHelper.StartAllService();
'''  

2.  客户端：引用WCFClientProxy和WCFCommon.dll
使用前初始化PF工厂类：  
'''  
PF.Init(ConfigurationManager.AppSettings["WCFServiceAddress"]); //初始化PF
'''  
使用：  
'''  
List<TestData> list = PF.Get<ITestService2>().GetBigData("001", "测试001");
'''





