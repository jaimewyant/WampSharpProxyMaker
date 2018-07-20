using System;
using System.IO;
using WampSharp.CodeGeneration;

// MOD: Import appropriate namespaces
// Import the namespace containing the interfaces to be used for implementation
// generation.  
// using Your.Namespace.With.Proxies;

namespace MkCalleeProxyClasses
{
    class Program
    {
        static int Main(string[] args)
        {            
            // MOD: args = new string[] { "output-directory" };
            // Set this to the full path of your output directory, or pass 
            // the argument in via the command line.  I find it easier to just
            // hardcode it here, and run the program whenever the proxy 
            // interfaces change.
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Pass output directory on comd line");
                return 1;
            }

            if (!Directory.Exists(args[0]))
            {
                Console.Error.WriteLine($"Directory {args[0]} does not exist.");
                return 1;
            }

            // MOD: Generate an implementation file for each interface.
            // One call to GenerateImplementation per interface.  Below is an example, change
            // it.
            GenerateImplentation(args[0], "SeasourcesApp.Wamp.Services", typeof(IAuthenticator));
            
            // MOD: Optionally generate a default interceptor.  If you don't generate one, you will
            // need to create one on your own.
            // GenerateInterceptor(args[0], "SeasourcesApp.Wamp.Services");
            return 0;
        }

        static void GenerateImplentation(string outdir, string namespace_, Type type)
        {
            var codeGen = new CalleeProxyCodeGenerator(namespace_);
            var code = codeGen.GenerateCode(type);
            var interfaceName = GetInterfaceName(type);
            var fileName = interfaceName + "Proxy.cs";
            File.WriteAllText(Path.Combine(outdir, fileName), code);
        }

        static void GenerateInterceptor(string outdir, string namespace_)
        {
            var code = @"
using System;
using System.Reflection;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Rpc;

namespace XXX {
    public class MyInterceptor : ICalleeProxyInterceptor
        {
            public CallOptions GetCallOptions(MethodInfo method)
            {
                return new CallOptions();
            }

            public string GetProcedureUri(MethodInfo method)
            {
                foreach (var customAttribute in method.CustomAttributes)
                {
                    if (customAttribute.AttributeType == typeof(WampProcedureAttribute)) 
                    {
                        return (string)customAttribute.ConstructorArguments[0].Value;       
                    }

                }
                throw new Exception(""Could not find WampProcedureAttribute"");
            }
        }
}
";
            File.WriteAllText(Path.Combine(outdir, "MyInterceptor.cs"), code.Replace("XXX", namespace_));
        }

        static string GetInterfaceName(Type interfaceType)
        {
            string result = interfaceType.Name;

            if (result.StartsWith("I"))
            {
                return result.Substring(1);
            }
            else
            {
                return result;
            }
        }
    }
}
