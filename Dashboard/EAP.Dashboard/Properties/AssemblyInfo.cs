﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的常规信息是通过以下项进行控制的
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("EAP.Dashboard")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("usish.com.cn")]
[assembly: AssemblyProduct("EAP.Dashboard")]
[assembly: AssemblyCopyright("版权所有(C) usish.com.cn 2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 将使此程序集中的类型
// 对 COM 组件不可见。如果需要
// COM，在该类型上将 ComVisible 属性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于 typelib 的 ID
[assembly: Guid("970e0e4c-794f-4e8a-91db-48f68f9321eb")]

// 程序集的版本信息由下列四个值组成:
//
//      主版本
//      次版本
//      内部版本号
//      修订号
//
// 你可以指定所有值，也可以让修订版本和内部版本号采用默认值，
// 方法是按如下所示使用 "*":
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", ConfigFileExtension = "config", Watch = true)]
