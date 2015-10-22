using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using NuGet;

namespace NugetPackageTrimmer
{
	class Program
	{
		static int Main(string[] args)
		{
			var repoUrl = args.Length > 4 ? args[3] : "https://packages.nuget.org/api/v2";

			Console.WriteLine($"Checking if packages in [{args[0]}] exist in {repoUrl} and pushing if they dont. [nuget path: {args[1]}]");

			var repo = PackageRepositoryFactory.Default.CreateRepository(repoUrl);

			var packageServer = new PackageServer(repoUrl, "NuGet Command Line");
			packageServer.SendingRequest += (sender, e) =>
			{
				Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "{0} {1}", e.Request.Method, e.Request.RequestUri));
			};

			foreach (var nupkg in Directory.EnumerateFiles(args[0], "*.nupkg"))
			{
				Console.WriteLine($"Checking nuget package : {nupkg}");
				var package = new OptimizedZipPackage(nupkg);

				if (nupkg.EndsWith(".symbols.nupkg"))
				{
					Console.WriteLine("That package is symbols, skipping...");
				}
				//else if (repo.Exists(package))
				//{
				//	Console.WriteLine("That package already exists, skipping...");
				//}
				else
				{
					Console.WriteLine("That package isnt in nuget yet, pushing...");


					ProcessStartInfo start = new ProcessStartInfo();
					// Enter in the command line arguments, everything you would enter after the executable name itself
					start.Arguments = $"push \"{nupkg}\" -ApiKey {args[2]} -NonInteractive";
					// Enter the executable to run, including the complete path
					start.FileName = args[1];
					// Do you want to show a console window?
					start.WindowStyle = ProcessWindowStyle.Hidden;
					start.CreateNoWindow = true;
					start.UseShellExecute = false;
					start.RedirectStandardOutput = true;
					start.RedirectStandardError = true;
					//start.
					int exitCode;

					using (Process proc = Process.Start(start))
					{
						proc.WaitForExit();

						// Retrieve the app's exit code
						exitCode = proc.ExitCode;
						Console.Out.Write(proc.StandardOutput.ReadToEnd());
					}

					if (exitCode != 0)
					{
						return exitCode;
					}
					Console.WriteLine("Pushed");
				}
			}
			return 0;
		}
	}
}
