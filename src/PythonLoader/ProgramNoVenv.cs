namespace PythonLoader
{
    using System;
    using System.Runtime.InteropServices;
    using GetSomeInput;
    using Python.Runtime;

    public class ProgramNoVenv
    {
        public static void MainNoVenv(string[] args)
        {
            try
            {
                /*
                 * See
                 * https://pythonnet.github.io/
                 * https://stackoverflow.com/questions/76156700/pythonnet-no-module-named-exception-for-my-module-but-not-numpy
                 * https://stackoverflow.com/questions/78070739/cant-import-my-module-with-python-net-from-the-same-directory-as-the-project
                 * https://somegenericdev.medium.com/calling-python-from-c-an-introduction-to-pythonnet-c3d45f7d5232
                 * https://www.freecodecamp.org/news/build-your-first-python-package/
                 * https://betterprogramming.pub/running-python-script-from-c-and-working-with-the-results-843e68d230e5
                 * https://www.educative.io/answers/how-to-call-a-python-function-from-c-sharp
                 * https://github.com/pythonnet/pythonnet
                 * https://stackoverflow.com/questions/20582270/distribution-independent-libpython-path
                 * 
                 * https://github.com/pythonnet/pythonnet/wiki/Using-Python.NET-with-Virtual-Environments
                 * 
                 */

                string script = Inputty.GetString("Module name: ", "script", false);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Runtime.PythonDLL = @"C:\Program Files\Python312\python312.dll";
                }
                else
                {
                    // see https://stackoverflow.com/questions/20582270/distribution-independent-libpython-path
                    // and https://pypi.org/project/find-libpython/
                    Runtime.PythonDLL = @"/usr/lib/x86_64-linux-gnu/libpython3.10.so.1.0";
                }

                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();

                Console.WriteLine("Python DLL     : " + Runtime.PythonDLL);
                Console.WriteLine("Build info     : " + PythonEngine.BuildInfo);
                Console.WriteLine("Compiler       : " + PythonEngine.Compiler);
                Console.WriteLine("Python version : " + PythonEngine.Version);
                Console.WriteLine("Max version    : " + PythonEngine.MaxSupportedVersion);
                Console.WriteLine("Min version    : " + PythonEngine.MinSupportedVersion);
                Console.WriteLine("Platform       : " + PythonEngine.Platform);
                Console.WriteLine("Program name   : " + PythonEngine.ProgramName);
                Console.WriteLine("Python home    : " + PythonEngine.PythonHome);
                Console.WriteLine("Python path    : " + PythonEngine.PythonPath);

                using (Py.GIL())
                {
                    // see https://stackoverflow.com/questions/78531348/python-net-issue-working-with-modules-on-ubuntu-22-04-but-works-on-windows-11/78531399
                    using (PyModule scope = Py.CreateScope())
                    {
                        scope.Import("sys");
                        scope.Exec(@"if '' not in sys.path: sys.path.insert(0, '')");

                        dynamic app = Py.Import(script);
                        Console.WriteLine(app.multiply(2, 4));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}