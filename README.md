# PythonLoader

PythonLoader is a simple example of using the fantastic [Python.NET]() library on Windows, Mac, and Linux.

## Usage

Two `.cs` files are present in this code: `ProgramNoVenv.cs` and `ProgramVenv.cs`.  The first does not use virtual environments, whereas the second one does.

Set either file's entrypoint to `Main`, build, and then run.  For the virtual environment version, a virtual environment will be created in `bin/Debug/net8.0/venv`, and will be reused on subsequent invocations.

## Helpful Links

The following links were helpful in writing this example and are here for your reference:

- https://stackoverflow.com/questions/78531348/python-net-issue-working-with-modules-on-ubuntu-22-04-but-works-on-windows-11/78550509#78550509
- https://packaging.python.org/en/latest/guides/installing-using-pip-and-virtual-environments/
- https://stackoverflow.com/questions/72719556/how-to-add-virtualenv-to-pythonnet
- https://stackoverflow.com/questions/21240653/how-to-install-a-package-inside-virtualenv
- https://stackoverflow.com/questions/8055371/how-do-i-run-two-commands-in-one-line-in-windows-cmd
