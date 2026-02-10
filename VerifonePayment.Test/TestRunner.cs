using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VerifonePayment.Test
{
    public static class TestRunner
    {
        public static void RunAllTests()
        {
            Console.WriteLine("=== Running Unit Tests ===");
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            
            int totalTests = 0;
            int passedTests = 0;
            int failedTests = 0;
            
            foreach (Type type in types)
            {
                if (type.GetCustomAttribute<TestClassAttribute>() != null)
                {
                    Console.WriteLine($"\nRunning tests in class: {type.Name}");
                    
                    MethodInfo[] methods = type.GetMethods();
                    foreach (MethodInfo method in methods)
                    {
                        if (method.GetCustomAttribute<TestMethodAttribute>() != null)
                        {
                            totalTests++;
                            Console.Write($"  Running {method.Name}... ");
                            
                            try
                            {
                                object instance = Activator.CreateInstance(type);
                                method.Invoke(instance, null);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("PASSED");
                                Console.ResetColor();
                                passedTests++;
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("FAILED");
                                Console.ResetColor();
                                Console.WriteLine($"    Error: {ex.InnerException?.Message ?? ex.Message}");
                                failedTests++;
                            }
                        }
                    }
                }
            }
            
            Console.WriteLine($"\n=== Test Results ===");
            Console.WriteLine($"Total tests: {totalTests}");
            Console.WriteLine($"Passed: {passedTests}");
            Console.WriteLine($"Failed: {failedTests}");
            
            if (failedTests == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("All tests passed!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{failedTests} test(s) failed!");
            }
            Console.ResetColor();
        }
    }
}