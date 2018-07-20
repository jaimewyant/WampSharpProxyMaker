# WampSharpProxyMaker
Code generator for WampSharp Callee Proxies

If you plan on using RPC via WAMP on iOS, then you need to generate proxies.
Normally, WampSharp can generate them for you by calling GetCalleeProxy:

```c#

IArgumentsService proxy =
                channel.RealmProxy.Services.GetCalleeProxy<IArgumentsService>();

```

Currently on iOS, this call generates an exception.  Elad has a better 
understanding of this issue which he explains [here](https://github.com/Code-Sharp/WampSharp/issues/257)

To get around this issue, create a dotnet core 2.0 console application within your solution.
Copy Program.cs from this repository, into your project and modify it appropriately.

I have marked the parts of the program that need to be modified with comments beginning with
the string MOD:.  Below is the first one in the source code.

```c#
// MOD: Import appropriate namespaces
```

Run the program each time the proxy interface files change to generate new files.

## How to use the proxy classes

```c#
// This code uses the generated interceptor.
var proxy = new UserProxy(wampChannel.RealmProxy, new MyInterceptor());
proxy.CallSomeMethod();
```