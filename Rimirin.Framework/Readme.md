# Rimirin.Framework
一个基于 .NET 的简易的 QQ 机器人框架
## 特性
- 依赖注入（基于 Microsoft.Extensions.DependencyInjection）
- 消息路由
- 自动重连
## 数据流
QQ Server → Mirai → Mirai-Wrapper → MessageRouter → Handler
## 引用
- [Executor-Cheng/Mirai-CSharp](https://github.com/Executor-Cheng/Mirai-CSharp)
- [dotnet/runtime](https://github.com/dotnet/runtime)