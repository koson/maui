#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Maui.ApplicationModel;
using Package = Windows.ApplicationModel.Package;

namespace Microsoft.Maui.Storage
{
	static partial class FileSystemUtils
	{
		public static bool AppPackageFileExists(string filename)
		{
			var file = PlatformGetFullAppPackageFilePath(filename);
			return File.Exists(file);
		}

		public static string PlatformGetFullAppPackageFilePath(string filename)
		{
			if (filename is null)
			{
				throw new ArgumentNullException(nameof(filename));
			}

			filename = NormalizePath(filename);

			string root;
			if (AppInfoUtils.IsPackagedApp)
			{
				root = Package.Current.InstalledLocation.Path;
			}
			else
			{
				root = AppContext.BaseDirectory;
			}

			return Path.Combine(root, filename);
		}

		public static bool TryGetAppPackageFileUri(string filename, [NotNullWhen(true)] out string? uri)
		{
			var path = PlatformGetFullAppPackageFilePath(filename);

			if (File.Exists(path))
			{
				if (AppInfoUtils.IsPackagedApp)

/* Unmerged change from project 'Essentials(net7.0-windows10.0.19041)'
Before:
					uri = $"ms-appx:///{filename.Replace('\\', '/')}";
				else
					uri = $"file:///{path.Replace('\\', '/')}";
After:
				{
					uri = $"ms-appx:///{filename.Replace('\\', '/')}";
				}
				else
				{
					uri = $"file:///{path.Replace('\\', '/')}";
				}
*/

/* Unmerged change from project 'Essentials(net7.0-windows10.0.20348)'
Before:
					uri = $"ms-appx:///{filename.Replace('\\', '/')}";
				else
					uri = $"file:///{path.Replace('\\', '/')}";
After:
				{
					uri = $"ms-appx:///{filename.Replace('\\', '/')}";
				}
				else
				{
					uri = $"file:///{path.Replace('\\', '/')}";
				}
*/
				{
					uri = $"ms-appx:///{filename.Replace('\\', '/')}";
				}
				else
				{
					uri = $"file:///{path.Replace('\\', '/')}";
				}

				return true;
			}

			uri = null;
			return false;
		}
	}
}
