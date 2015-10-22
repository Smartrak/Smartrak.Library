# Smartrak.Library

Smartrak library is a place to put cool helpers and non-proprietary tools used by smartrak. Projects in the library are automatically published to Smartrak's public nuget account.

### I have something cool, where should I put it?

Awesome we love cool stuff. 

If this is an extension or helper for a tool from nuget concider doing a pull request to that project. Otherwise put it in a project named [TheOtherPackage].[SomethingToDescribeWhatThisExtends]

If this is a new tool or helper give it a cool name

If this fits with an existing package put it there.

### How do I publish a new version of a package?

Super easy, go to the AssemblyInfo File of the package and change the version number of the package.

![Imgur](http://i.imgur.com/IIj7ZlW.png)

Make sure you follow SemVer eg. [BreakingChange].[NewFeature].[BugFix]

If this is not ready for public consumption add -alpha to the end of the AssemblyInformationalVersion eg: `[assembly: AssemblyInformationalVersion("1.0.0-alpha")]`. To increment an alpha version add a number after -alpha eg: `[assembly: AssemblyInformationalVersion("1.0.0-alpha2")]`

### How do I publish create a new package?

Create a new project in the library solution.

Add a nuspec file to the project with the same name as the project.

	<?xml version="1.0" encoding="utf-8"?>
	<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
		<metadata>
			<id>$id$</id>
			<version>$version$</version>
			<authors>Smartrak</authors>
			<description>Helpers for dealing with collections</description>
			<projectUrl>https://github.com/Smartrak/Smartrak.Library/$id$</projectUrl>
			<licenseUrl>https://raw.githubusercontent.com/Smartrak/Smartrak.Library/master/LICENSE</licenseUrl>
		</metadata>
	</package>

Add an AssemblyInformationalVersion to the assemblyinfo of the project with a 3 digit version number (starting at 1.0.0).

### Do and dont

 - Follow Semver eg [BreakingChange].[NewFeature].[BugFix]
 - Projects shouldn't directly refer to other packages in library. If they do nuget wont correctly determine the relationsip between packages
 - Packages should be small and concise. They should be about one thing only. 
 - Packages should reference a minimal amount of other nuget packages, too many and they are complicated to maintain
