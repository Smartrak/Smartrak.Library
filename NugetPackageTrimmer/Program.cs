using System;
using System.Globalization;
using System.IO;
using NuGet;

namespace NugetPackageTrimmer
{
	class Program
	{
		static void Main(string[] args)
		{
			var repoUrl = args.Length > 3 ? args[2] : "https://packages.nuget.org/api/v2";

			Console.WriteLine($"Checking if packages in [{args[0]}] exist in {repoUrl} and deleting any that dont need deploying...");

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

				if (repo.Exists(package))
				{
					Console.WriteLine("That package already exists, skipping...");
					File.Delete(nupkg);
				}
				else
				{
					Console.WriteLine("That package isnt in nuget yet, pushing...");
					// Push the package to the server
					var sourceUri = new Uri(repoUrl);

					packageServer.PushPackage(
						args[1],
						package,
						new FileInfo(nupkg).Length,
						Convert.ToInt32(new TimeSpan(0, 0, 1, 0, 0).TotalMilliseconds),
						false);
					Console.WriteLine("Pushed");
				}
			}
		}
	}
}
