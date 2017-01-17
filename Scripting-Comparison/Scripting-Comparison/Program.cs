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


            swTotal.Reset();
            swTotal.Start();
            TestCSScript();
            swTotal.Stop();
            Console.WriteLine("Total CSScript Elapsed(MS) = {0} - FPS: {1}", swTotal.ElapsedMilliseconds, 1000.0 / swTotal.ElapsedMilliseconds);
            Console.WriteLine("------");


            swTotal.Reset();
            swTotal.Start();
            TestRoslyn();
            swTotal.Stop();
            Console.WriteLine("Total Roslyn Elapsed(MS) = {0} - FPS: {1}", swTotal.ElapsedMilliseconds, 1000.0 / swTotal.ElapsedMilliseconds);

            Console.ReadKey();
        }

        static async void TestRoslyn()
        {
            double[] val = new double[1000];
            object[] x = new object[1000];
            bool[] a = new bool[1000];

            Console.WriteLine("Separate instances of a Roslyn context do not exist");

            StartTiming();

            for (var i = 0; i < 1000; i++)
            {
                val[i] = await CSharpScript.EvaluateAsync<double>("Math.Sin(10*10+7);",
     ScriptOptions.Default.WithImports("System"));
            }

            EndTiming("Retrieving a value from a math function parsed at runtime 1000 times");

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

        }

        static void TestCSScript()
        {
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
    }
}
