# Calls


## Installing Calls

With NuGet:

    Install-Package Calls -Version 1.0.0-beta.2

With .NET Cli

    dotnet add package Calls --version 1.0.0-beta.2

Calls采用源生成提供方法调用的中介器，类似Mediator，并提供更加灵活的功能。

## Using Calls

首先需要定义一个分布类，并继承Calls.Call类

```
[IncludeAssembly]
public partial class MethodCall : Call { }
```

并在可调用的方法上标记[Callable]特性，例如：
```
public class Handler
{
    [Callable("add")]
    public int Add(int a, int b) => a + b;

    [Callable("sub")]
    public int Sub(int a, int b) => a - b;
}
```

Calls依赖Microsoft.Extensions.DependencyInjection,需要将可调用方法的类添加进依赖注入

```
services.AddSingleton<ICall, MethodCall>();
services.AddScoped<Handler>()
```

通过依赖注入获取ICall接口并调用你方法
```
var call = serviceProvider.GetRequiredService<ICall>();

//这里result会等于3
var result = call.Invoke<int>("add",1, 2);
```

## Calls的方法路由

### 通过类型查找
这种模式非常类似 MeidatR 的方式，不同的是命令（请求）无需继承IRequest，处理程序也无需继承IRequrestHandler接口

定义：
```
using Calls;

public class Handler
{
    [Callable]
    public int Handle1(AddRequest request) => a + b;

    [Callable]
    public int Handle2(SubRequest request) => a - b;
}

public class AddRequest
{
    public int A { get; set; }
    public int B { get; set; }
} 

public class SubRequest
{
    public int A { get; set; }
    public int B { get; set; }
} 
```
调用：
```
var call = services.GetRequiredService<ICall>();

//result1 equal 3
var result1 = call.Invoke<int>(new AddRequest
{
    A = 1,
    B = 2,
});

//result2 equal 1
var result2 = call.Invoke<int>(new SubRequest
{
    A = 2,
    B = 1,
});

```
### 通过方法名
在该模式下，我们为每一个方法定义一个Key，在调用时通过传递Key定位方法，该模式非常类似AspNetCore中的Controller/Action路由。

定义：
```
using Calls;

public class Handler
{
    [Callable("add")]
    public int Handle1(int a, int b) => a + b;

    [Callable("sub")]
    public int Handle1(int a, int b) => a - b;
}
```
调用：
```
var call = services.GetRequiredService<ICall>();

//result1 equal 3
var result1 = call.Invoke<int>("add", 2, 1);

//result2 equal 1
var result2 = call.Invoke<int>("sub", 2, 1);
```
也可以直接通过方法名作为Key：
```
using Calls;

public class Handler
{
    [Callable(true)]
    public int Add(int a, int b) => a + b;

    [Callable(true)]
    public int Sub(int a, int b) => a - b;
}
```
```
var call = services.GetRequiredService<ICall>();

//result1 equal 3
var result1 = call.Invoke<int>("Add", 2, 1);

//result2 equal 1
var result2 = call.Invoke<int>("Sub", 2, 1);
```

## 同步与异步

Calls支持同步调用与异步调用

定义：
```
using Calls;

public class Handler
{
    [Callable("a")]
    public void Handle() { }

    [Callable("b")]
    public Task Handle() { }
}
```
调用：
```
var call = services.GetRequiredService<ICall>();

//invoke method a
call.Invoke("a");

//async invoke method b
await call.InvokeAsync("b");
```

## 参数与返回值

### 多参数支持

定义：
```
using Calls;

public class Handler
{
    [Callable]
    public int Handle() => 0;

    [Callable]
    public int Handle(int a) => 1;

    [Callable]
    public int Handle(int a,int b) => 2;

    [Callable]
    public int Handle(int a,int b,int c) => 2;
}
```
调用：
```
var call = services.GetRequiredService<ICall>();

//返回值为 0
var result1 = call.Invoke<int>();

//返回值为 1
var result2 = call.Invoke<int>(1);

//返回值为 2
var result3 = call.Invoke<int>(1, 2);

//返回值为 3
var result4 = call.Invoke<int>(1, 2, 3);

```
### 支持Void与返回值
定义：
```
using Calls;

public class Handler
{
    [Callable]
    public Task<PongResponse> Ping(PingRequest) => Task.FromResult(new PongResponse());

    [Callable]
    public Task Handle(TestCommand command) => Task.CompletedTask;
}

public class PingRequest { }
public class PongResponse { }
public class TestCommand { }
```
调用：
```
var call = services.GetRequiredService<ICall>();

//返回值为 PongResponse 类型
var response = await call.InvokeAsync<PongResponse>(new PingRequest());

//无返回值
await call.InvokeAsync(new TestCommand());

```
## 方法优先级
Call始终优先调用参数类型完全匹配的方法

定义：
```
using Calls;

public class Handler
{
    [Callable]
    public int Handle(BaseType args) => 0;

    [Callable]
    public int Handle(DerivedType args) => 1;
}

public class BaseType { }
public class DerivedType : BaseType { }
public class DerivedType2 : BaseType { }

```

调用：
```
var call = services.GetRequiredService<ICall>();

//返回值为 0
var result1 = await call.InvokeAsync<int>(new BaseType());

//返回值为 1
var result2 = await call.InvokeAsync<int>(new DerivedType());

//返回值为 0
var result3 = await call.InvokeAsync<int>(new DerivedType2());
```

## 依赖注入
Calls可将[Callable]定义在接口或抽象类中
定义：
```
using Calls;

[IncludeAssembly]
public partial class MethodCall : Call { }

public interface IHandler
{
    [Callable] 
    void Handle(int a);
}

public abstract class HandlerBase
{
    [Callable] 
    public abstract void Handle(bool a);
}

```
依赖注入：
```
services.AddSingleton<ICall, MethodCall>();
services.AddScoped<IHandler, HandlerImpl>();
services.AddScoped<HandlerBase, HandlerBaseImpl>();
```
调用：
```
var call = services.GetRequiredService<ICall>();
call.Invoke(1);
call.Invoke(true);
```

## Include Type与 Include Assembly
Calls提供3种方法类型引用模式：

1. [IncludeAssembly], 标记此特性，源生成将默认搜索当前Assembly的所有[Callable]方法
2. [IncludeType(typeof(PingHandler))]，标记此特性，源生成将PingHandler类型下的y的所有[Callable]方法
3. [IncludeAssembly(typeof(PingHandler))], 标记此特性，源生成将搜索类型PingHandler所在Assebmly种的的所有[Callable]方法