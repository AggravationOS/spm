using System;
using System.IO;
using System.Net;
using System.Diagnostics;

public class Program
{
    public static Dictionary<string, string> BuildVars = new Dictionary<string, string>();
    public static Dictionary<string, Package> Packages = new Dictionary<string, Package>();
    public static void Main(string[] args)
    {
        if(Environment.UserName != "root")
        {
            Console.WriteLine("spm must be run as root!");
            return;
        }
        if(args.Length == 0)
        {
            Console.WriteLine("spm v1.0.0: The World's Worst LinuxⓇ Package Manager™\nCommands: spm install <package> \nspm uninstall <package> \nspm update \nspm update-manifest");
        }
        else
        {
            if(args[0] == "install")
            {
                if(args.Length == 1)
                {
                    Console.WriteLine("Usage: spm install <package>");
                }
                else
                {
                    for(int i = 1; i < args.Length; i++)
                    {
                        InstallPackage(args[i]);
                    }
                }
            }
            if(args[0] == "uninstall")
            {
                if(args.Length == 1)
                {
                    Console.WriteLine("Usage: spm uninstall <package>");
                }
            }
            if(args[0] == "update")
            {
                //Implement update
            }
            if(args[0] == "update-manifest")
            {
                //Implement manifest update
            }
        }
    }

    /// <summary>
    /// Executes the specified PowerShell Core command.
    /// </summary>
    /// <param name="command"></param>
    public static void ExecuteShellCommand(string command)
    {
        Process.Start(@"/usr/bin/pwsh", $"-Command \"{command}\"");
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

    public Package(string name, string description, string cloneURL)
    {
        Name = name;
        Description = description;
        CloneURL = cloneURL;
        InstallCommands = new List<string>();
    }
}