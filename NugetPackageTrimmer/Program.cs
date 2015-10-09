using System;
using System.IO;
using NuGet;

namespace NugetPackageTrimmer
{
	class Program
	{
		static void Main(string[] args)
		{
			var repoUrl = args.Length > 2 ? args[1] : "https://packages.nuget.org/api/v2";
			var repo = PackageRepositoryFactory.Default.CreateRepository(repoUrl);

			Console.WriteLine($"Checking if packages in [{args[0]}] exist in {repoUrl} and deleting any that dont need deploying...");


			foreach (var nupkg in Directory.EnumerateFiles(args[0], "*.nupkg"))
			{
				Console.WriteLine($"Checking nuget package : {nupkg}");
				var package = new ZipPackage(nupkg);

				if (repo.Exists(package))
				{
					Console.WriteLine("That package already exists, deleting...");
					File.Delete(nupkg);
				}
				else
				{
					Console.WriteLine("That package isnt in nuget yet, skipping...");
				}
			}
		}
	}
}
