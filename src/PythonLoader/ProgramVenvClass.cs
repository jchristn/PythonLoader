namespace PythonLoader
{
    using System;
    using System.Runtime.InteropServices;
    using GetSomeInput;
    using Python.Runtime;
    using HeyShelli;

    public class ProgramVenvClass
    {
        public static void Main(string[] args)
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
                 * https://packaging.python.org/en/latest/guides/installing-using-pip-and-virtual-environments/
                 * https://csguide.cs.princeton.edu/software/virtualenv#:~:text=A%20Python%20virtual%20environment%20(venv,installed%20in%20the%20specific%20venv.
                 * https://stackoverflow.com/questions/72719556/how-to-add-virtualenv-to-pythonnet
                 * 
                 */

                string script = Inputty.GetString("Module name: ", "scriptPerson", false);

                #region Set-Runtime-and-Initialize

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

                #endregion

                #region Setup-Virtual-Environment

                string pathSeparator = ";";
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) pathSeparator = ":";

                string directorySeparator = "\\";
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) directorySeparator = "/";

                string pathVirtualEnvironment = "." + directorySeparator + "venv" + directorySeparator;
                if (!Directory.Exists(pathVirtualEnvironment))
                {
                    Console.WriteLine("Creating virtual environment: " + pathVirtualEnvironment);

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        Shelli.Go("py -m venv " + pathVirtualEnvironment);
                    }
                    else
                    {
                        Shelli.Go("python3 -m venv " + pathVirtualEnvironment);
                    }

                    if (File.Exists("requirements.txt"))
                    {
                        Console.WriteLine("Installing requirements from requirements.txt");

                        string cmd = 
                            "py -m venv venv " + 
                            "&& venv\\scripts\\activate " +
                            "&& pip install -r requirements.txt";

                        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            cmd = "python3 -m venv venv " +
                                "; source venv/bin/activate " +
                                "; pip install -r requirements.txt";
                        }

                        Shelli.Go(cmd);
                    }
                }
                else
                {
                    Console.WriteLine("Reusing virtual environment: " + pathVirtualEnvironment);
                }

                string pathVariable = Environment.GetEnvironmentVariable("PATH");
                if (String.IsNullOrEmpty(pathVariable)) pathVariable = "";
                pathVariable = pathVariable.TrimEnd(';').TrimEnd(':');

                pathVariable = String.IsNullOrEmpty(pathVariable) ? pathVirtualEnvironment : pathVariable + pathSeparator + pathVirtualEnvironment;
                Environment.SetEnvironmentVariable("PATH", pathVariable, EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable("PATH", pathVirtualEnvironment, EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable(
                    "PYTHONHOME", 
                    pathVirtualEnvironment, 
                    EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable(
                    "PYTHONPATH", 
                    pathVirtualEnvironment + "Lib" + directorySeparator + "site-packages;" + pathVirtualEnvironment + "Lib", 
                    EnvironmentVariableTarget.Process);

                PythonEngine.PythonHome = pathVirtualEnvironment;
                PythonEngine.PythonPath = PythonEngine.PythonPath + ";" + Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);

                #endregion

                #region Enumerate-Configuration

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

                #endregion

                #region Execute

                using (Py.GIL())
                {
                    using (PyModule scope = Py.CreateScope())
                    {
                        // see https://stackoverflow.com/questions/78531348/python-net-issue-working-with-modules-on-ubuntu-22-04-but-works-on-windows-11/78531399
                        scope.Import("sys");
                        scope.Exec(@"if '' not in sys.path: sys.path.insert(0, '')");

                        // execute
                        dynamic app = Py.Import(script);

                        Person joel = new Person
                        {
                            First = "Joel",
                            Last = "Christner",
                            Age = 47
                        };

                        PyObject resp = app.hello(joel.ToDictionary());
                        string json = resp.ToString();

                        Console.WriteLine("--- In caller ---");
                        Console.WriteLine(json);
                    }
                }

                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public class Person
        {
            public string First { get; set; }
            public string Last { get; set; }
            public int Age { get; set; }
            public Dictionary<string, object> ToDictionary()
            {
                Dictionary<string, object> ret = new Dictionary<string, object>();
                ret.Add("First", First);
                ret.Add("Last", Last);
                ret.Add("Age", Age);
                return ret;
            }

            public Person()
            {

            }
        }
    }
}