using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;
using System.Diagnostics;
using CSScriptLibrary;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;

namespace Scripting_Comparison
{
    class Program
    {

        static Stopwatch sw = new Stopwatch();
        static Stopwatch swTotal = new Stopwatch();

        static void Main(string[] args)
        {
            
            swTotal.Reset();
            swTotal.Start();
            TestNLua();
            swTotal.Stop();
            Console.WriteLine("Total NLUA Elapsed(MS) = {0} - FPS: {1}", swTotal.ElapsedMilliseconds, 1000.0 / swTotal.ElapsedMilliseconds);
            Console.WriteLine("------");

            /*
            swTotal.Reset();
            swTotal.Start();
            TestCSScript();
            swTotal.Stop();
            Console.WriteLine("Total CSScript Elapsed(MS) = {0} - FPS: {1}", swTotal.ElapsedMilliseconds, 1000.0 / swTotal.ElapsedMilliseconds);
            Console.WriteLine("------");
            */

            swTotal.Reset();
            swTotal.Start();
            TestRoslyn();
            swTotal.Stop();
            Console.WriteLine("Total Roslyn Elapsed(MS) = {0} - FPS: {1}", swTotal.ElapsedMilliseconds, 1000.0 / swTotal.ElapsedMilliseconds);

            StartTiming();
            bool r = sieve(100000);
            EndTiming("Calling a single heavy C# function in normal C# returned value: " + r + ",");

            Console.ReadKey();
        }

        static async void TestRoslyn()
        {
            double[] val = new double[1000];
            object[] x = new object[1000];
            bool[] a = new bool[1000];

            Console.WriteLine("Separate instances of a Roslyn context do not exist");

            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(@"
    using System;

    namespace RoslynCompileSample
    {
        public class Writer
        {
            public void Write(string message)
            {
                Console.WriteLine(message);
            }
        }
    }");
                string assemblyName = Path.GetRandomFileName();
                MetadataReference[] references = new MetadataReference[]
                {
    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
                };

                CSharpCompilation compilation = CSharpCompilation.Create(
                    assemblyName,
                    syntaxTrees: new[] { syntaxTree },
                    references: references,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                using (var ms = new MemoryStream())
                {

                    StartTiming();
                    EmitResult result = compilation.Emit(ms);
                    EndTiming("Compiled class");

                    if (!result.Success)
                    {
                        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error);

                        foreach (Diagnostic diagnostic in failures)
                        {
                            Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                        }
                    }
                    else
                    {
                        ms.Seek(0, SeekOrigin.Begin);

                        StartTiming();
                        Assembly assembly = Assembly.Load(ms.ToArray());


                        EndTiming("Loaded assembly");

                        Type type = assembly.GetType("RoslynCompileSample.Writer");
                        object obj = Activator.CreateInstance(type);
                        type.InvokeMember("Write",
                            BindingFlags.Default | BindingFlags.InvokeMethod,
                            null,
                            obj,
                            new object[] { "Hello World" });
                    }
                }`````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````
            }

            StartTiming();
            Parallel.For(0, 1000, i =>
            //for (var i = 0; i < 1000; i++)
            {

                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(@"
    using System;

    namespace RoslynCompileSample" + i + @"
    {
        public class Writer
        {
            public void Write(string message)
            {
                Console.WriteLine(message);
            }
        }
    }");
                string assemblyName = Path.GetRandomFileName();
                MetadataReference[] references = new MetadataReference[]
                {
    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
                };

                CSharpCompilation compilation = CSharpCompilation.Create(
                    assemblyName,
                    syntaxTrees: new[] { syntaxTree },
                    references: references,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithOptimizationLevel(OptimizationLevel.Release));

                using (var ms = new MemoryStream())
                {

                    EmitResult result = compilation.Emit(ms);

                    if (!result.Success)
                    {
                        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error);

                        foreach (Diagnostic diagnostic in failures)
                        {
                            Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                        }
                    }
                    else
                    {
                        ms.Seek(0, SeekOrigin.Begin);

                        Assembly assembly = Assembly.Load(ms.ToArray());


                        //Type type = assembly.GetType("RoslynCompileSample"+i+".Writer");
                        //object obj = Activator.CreateInstance(type);
                        /*
                        type.InvokeMember("Write",
                            BindingFlags.Default | BindingFlags.InvokeMethod,
                            null,
                            obj,
                            new object[] { "Hello World: " + i });
                            */
                        /*
                        Type type = assembly.GetType("RoslynCompileSample2.Writer");
                        object obj = Activator.CreateInstance(type);
                        type.InvokeMember("Write",
                            BindingFlags.Default | BindingFlags.InvokeMethod,
                            null,
                            obj,
                            new object[] { "Hello World" });
                            */
                    }
                }

            });

            EndTiming("Full class");
            /*
            for (var i = 0; i < 1000; i++)
            {
                val[i] = await CSharpScript.EvaluateAsync<double>("Math.Sin(10*10+7+"+i+");",
     ScriptOptions.Default.WithImports("System"));
            }
            */

            // EndTiming("Retrieving a value from a math function parsed at runtime 1000 times");
            /*
            StartTiming();
            MethodDelegate<bool> script = null;
            for (var i = 0; i < 1000; i++)
            {
                script = CSScript.RoslynEvaluator.CreateDelegate<bool>(@"using System.Collections.Generic;
bool sieve(int n)
        {
            var x = new Dictionary<int, int>();
            int iter = 0;
            int i = 0;
            int p = 0;
            int j = 0;
            while (iter != 101)
            {
                x[1] = 0;
                i = 2;
                while (i <= n)
                {
                    x[i] = 1;
                    i = i + 1;
                }
                p = 2;
                while (p * p <= n)
                {
                    j = p;
                    while (j <= n)
                    {
                        x[j] = 0;
                        j = j + p;
                    }
                    while (x[p] != 1)
                    {
                        p = p + 1;
                    }
                }
                iter = iter + 1;
            }
            return true;
        }");
            }
            EndTiming("Setting a function 1000 times");


            StartTiming();
            for (var i = 0; i < 1000; i++)
            {
                a[i] = script(10);
            }

            EndTiming("Calling a function 1000 times");


            StartTiming();

            bool r = script(100000);

            EndTiming("Calling a single heavy C# function returned value: " + r + ",");
            */
        }

        static void TestCSScript()
        {


            String[] val2 = new String[1000];
            String test = "";
            for (var i = 0; i < 1000; i++)
            {
                var SayHello = @"using System;
                         public static class aTest" + i + @" {
                         public static void SayHello(string greeting)
                         {
                             ConsoleSayHello(greeting+" + i + @");
                         }
                         static void MessageBoxSayHello(string greeting)
                         {
                             SayHello(greeting+" + i + @");
                         }
                         static void ConsoleSayHello(string greeting)
                         {
                             Console.WriteLine(greeting+" + i + @");
                         }
                        }
                        public class aTestb" + i + @" {
                         public void SayHello(string greeting)
                         {
                             ConsoleSayHello(greeting+" + i + @");
                         }
                         void MessageBoxSayHello(string greeting)
                         {
                             SayHello(greeting+" + i + @");
                         }
                          void ConsoleSayHello(string greeting)
                         {
                             Console.WriteLine(greeting+" + i + @");
                         }
                        }";

                test += SayHello;
                val2[i] = SayHello;
            }
            String[] files = new String[1000];
            for (var i = 0; i < 1000; i++)
            {
                StreamWriter textFile = File.CreateText("Test" + i + ".txt");
                textFile.WriteLine(val2[i]);
                textFile.Flush();
                textFile.Close();
                files[i] = "Test" + i + ".txt";
            }


            StartTiming();
            System.Reflection.Assembly output;
            for (var i = 0; i < 10; i++)
            {

                output = CSScript.Load(files[i]);
            }
            //output = CSScript.LoadFiles(files);
            EndTiming("Test");
            //Console.WriteLine("Type: " + output.GetType("aTestb1"));
            //Console.WriteLine("Object: " + output.CreateObject("aTestb1"));
            //Console.WriteLine("output: " + output);

            //for (var i = 0; i < 100; i++)
            //{
            //    var SayHello = val[i].GetStaticMethod("SayHello", typeof(string));
            //    SayHello("Hello again!");
            //}

            double[] val = new double[1000];
            object[] x = new object[1000];
            bool[] a = new bool[1000];

            Console.WriteLine("Separate instances of a CSScript context do not exist");
            
            StartTiming();

            for (var i = 0; i < 1000; i++)
            {
                var doMath = CSScript.CreateFunc<double>(@"double doMath(int a)
                                             {
                                                 return Math.Sin(10*10+7);
                                             }");
                val[i] = doMath(0);
            }

            EndTiming("Retrieving a value from a math function parsed at runtime 1000 times");

            StartTiming();
            MethodDelegate<bool> script = null;
            for (var i = 0; i < 1000; i++)
            {
                script = CSScript.CreateFunc<bool>(
                           @"using System.Collections.Generic;
bool sieve(int n)
        {
            var x = new Dictionary<int, int>();
            int iter = 0;
            int i = 0;
            int p = 0;
            int j = 0;
            while (iter != 101)
            {
                x[1] = 0;
                i = 2;
                while (i <= n)
                {
                    x[i] = 1;
                    i = i + 1;
                }
                p = 2;
                while (p * p <= n)
                {
                    j = p;
                    while (j <= n)
                    {
                        x[j] = 0;
                        j = j + p;
                    }
                    while (x[p] != 1)
                    {
                        p = p + 1;
                    }
                }
                iter = iter + 1;
            }
            return true;
        }");
            }
            EndTiming("Setting a function 1000 times");


            StartTiming();
            for (var i = 0; i < 1000; i++)
            {
                a[i] = script(10);
            }

            EndTiming("Calling a function 1000 times");


            StartTiming();

            bool r = script(100000);

            EndTiming("Calling a single heavy C# function returned value: " + r + ",");

        }

        static void TestNLua()
        {
            StartTiming();

            Lua[] contexts = new Lua[1000];
            double[] val = new double[1000];
            object[] x = new object[1000];
            bool[] a = new bool[1000];

            for (var i = 0; i < 1000; i++)
            {
                contexts[i] = new Lua();
                contexts[i].LoadCLRPackage(); // Enable call methods using reflection
            }
            
            EndTiming("Preparing 1000 Lua Instances");
            
            StartTiming();
            
            for (var i = 0; i < 1000; i++)
            {
                val[i] = (double)contexts[i].DoString("return math.sin (10)*10 + 7")[0];
            }

            EndTiming("Retrieving a value from a math function parsed at runtime 1000 times");



            // Send value to Lua
            StartTiming();
            for (var i = 0; i < 1000; i++)
            {
                contexts[i]["val_x"] = 10;
            }
            EndTiming("Setting a global value 1000 times");


            StartTiming();
            
            for (var i = 0; i < 1000; i++)
            {
                x[i] = contexts[i]["val_x"];
            }

            EndTiming("Retrieving a global value 1000 times");


            StartTiming();
            
            for (var i = 0; i < 1000; i++)
            {
                contexts[i].DoString(@"
                function sieve(n)
                  local x = {}
                  local iter = 0
                  local i = 0;
                  local j = 0;
                  local p = 0;
                  while(iter ~= 101) do
                    x[1] = 0
                    i = 2
                    while(i <= n) do
                      x[i] = 1
                      i = i + 1
                    end
                    p = 2
                    while(p * p <= n) do
                      j = p
                      while(j <= n) do
                        x[j] = 0
                        j = j + p
                      end
                      while(x[p] ~= 1) do
                        p = p + 1
                      end
                    end
                    iter = iter + 1
                  end
                  return true
                end
                ", "");
            }

            EndTiming("Setting a function 1000 times");


            StartTiming();
            for (var i = 0; i < 1000; i++)
            {
                a[i] = (bool)contexts[i].GetFunction("sieve").Call(10)[0];
            }

            EndTiming("Calling a function 1000 times");
            

            StartTiming();

            bool r = (bool)contexts[0].GetFunction("sieve").Call(100000)[0];

            EndTiming("Calling a single heavy lua function returned value: " + r + ",");
        }
        static void StartTiming()
        {
            sw.Reset();
            sw.Start();
        }
        static void EndTiming(String label)
        {
            sw.Stop();
            Console.WriteLine("{0} Elapsed(MS) = {1} - FPS: {2}", label, sw.ElapsedMilliseconds, 1000.0 / sw.ElapsedMilliseconds);
        }

        static bool sieve(int n)
        {
            var x = new Dictionary<int, int>();
            int iter = 0;
            int i = 0;
            int p = 0;
            int j = 0;
            while (iter != 101)
            {
                x[1] = 0;
                i = 2;
                while (i <= n)
                {
                    x[i] = 1;
                    i = i + 1;
                }
                p = 2;
                while (p * p <= n)
                {
                    j = p;
                    while (j <= n)
                    {
                        x[j] = 0;
                        j = j + p;
                    }
                    while (x[p] != 1)
                    {
                        p = p + 1;
                    }
                }
                iter = iter + 1;
            }
            return true;
        }
    }
}
