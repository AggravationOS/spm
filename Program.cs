//Uncomment for testing certain features without root access.
//Yes, I am aware that this is not a good way to do this.
//If you wanted a well-written package manager, you would not be using spm.
//#define ROOT_BYPASS

using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Linq;

public unsafe class Program
{
    public static Dictionary<string, string> BuildVars = new Dictionary<string, string>();
    public static Dictionary<string, Package> Packages = new Dictionary<string, Package>();

    public static void Main(string[] args)
    {
        #if !ROOT_BYPASS
        if(Environment.UserName != "root")
        {
            Console.WriteLine("spm must be run as root!");
            return;
        }
        #endif
        if(args.Length == 0)
        {
            Console.WriteLine("spm v1.0.0: The World's Worst LinuxⓇ  Package Manager™\nFlags: \n -E | --eject <package>: Uninstalls the specified package. \n -R | --refresh: Updates the system. \n -s | --sync <package>: Installs or updates the specified package. \n -S | --segfault: Triggers a segmentation fault.");
        }
        else
        {
            if(args[0] == "-s" || args[0] == "--sync")
            {
                if(args.Length == 1)
                {
                    Console.WriteLine($"Usage: spm {args[0]} <package>");
                }
                else
                {
                    for(int i = 1; i < args.Length; i++)
                    {
                        InstallPackage(args[i]);
                    }
                }
            }
            if(args[0] == "-S" || args[0] == "--segfault")
            {
                int* x = (int*)-1;
                *x += 3;
            }
            if(args[0] == "-E" || args[0] == "--eject")
            {
                if(args.Length == 1)
                {
                    Console.WriteLine($"Usage: spm {args[0]} <package>");
                }
                else
                {
                    //TODO: Implement uninstall
                }
            }
            if(args[0] == "-R" || args[0] == "--refresh")
            {
                //TODO: Implement update
            }
        }
    }

    /// <summary>
    /// Executes the specified PowerShell Core command.
    /// </summary>
    /// <param name="command"></param>
    public static void ExecuteShellCommand(string command)
    {
        var process = Process.Start("pwsh", $"-Command \"{command}\"");
        process.WaitForExit();
    }

    public static void InstallPackage(string package)
    {
        Directory.CreateDirectory(@"/var/cache/spm/pkg");
        if(Packages.ContainsKey(package))
        {
            Package pkg = Packages[package];
            if(Directory.Exists($@"/var/cache/spm/pkg/{pkg.Name}"))
                ExecuteShellCommand($"cd /var/cache/spm/pkg/{pkg.Name}; git reset --hard; git pull");
            else
                ExecuteShellCommand($"git clone {pkg.CloneURL} /var/cache/spm/pkg/{pkg.Name}");
            string buildCommandString = "";
            foreach(string command in pkg.InstallCommands)
            {
                buildCommandString += $"{command}; ";
            }
            ExecuteShellCommand($"cd /var/cache/spm/pkg/{pkg.Name}; {buildCommandString}");
        }
        else
        {
            Console.WriteLine($"{package}: package not found!");
        }
    }
}

public class Package
{
    public string Name { get; }
    public string Description { get; }
    public string CloneURL { get; }
    public List<string> InstallCommands { get; }
    public List<Package> Dependencies { get; }
    public List<Package> Dependents { get; }

    public Package(string name, string description, string cloneURL)
    {
        Name = name;
        Description = description;
        CloneURL = cloneURL;
        InstallCommands = new List<string>();
        Dependencies = new List<Package>();
        Dependents = new List<Package>();
    }
    public Package(string name, string description, string cloneURL, IEnumerable<string> installCommands, IEnumerable<Package> dependencies) : this(name, description, cloneURL)
    {
        InstallCommands = installCommands.ToList();
        Dependencies = dependencies.ToList();
    }
}