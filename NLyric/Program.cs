using System;
using System.Cli;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLyric.Settings;

namespace NLyric {
	public static class Program {
		private static async Task Main(string[] args) {
			if (args != null && args.Contains("-h")) {
				CommandLine.ShowUsage<Arguments>();
				return;
			}
			bool noargs = false;
			if (args == null || args.Length == 0) {
				noargs = true;
			}

			try {
				Console.Title = GetTitle();
			}
			catch {
			}

			Arguments arguments = null;
			if (!noargs) {
				if (!CommandLine.TryParse(args, out arguments)) {
					CommandLine.ShowUsage<Arguments>();
					return;
				}
			}
			else {
				arguments = new Arguments();
				arguments.noargs = true;
			}
			AllSettings.Default = JsonConvert.DeserializeObject<AllSettings>(File.ReadAllText("Settings.json"));
			await NLyricImpl.ExecuteAsync(arguments);
			FastConsole.WriteLine("完成", ConsoleColor.Green);
			FastConsole.Synchronize();
			if (Debugger.IsAttached) {
				Console.WriteLine("按任意键继续...");
				try {
					Console.ReadKey(true);
				}
				catch {
				}
			}
		}

		private static string GetTitle() {
			string productName = GetAssemblyAttribute<AssemblyProductAttribute>().Product;
			string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			string copyright = GetAssemblyAttribute<AssemblyCopyrightAttribute>().Copyright.Substring(12);
			int firstBlankIndex = copyright.IndexOf(' ');
			string copyrightOwnerName = copyright.Substring(firstBlankIndex + 1);
			string copyrightYear = copyright.Substring(0, firstBlankIndex);
			return $"{productName} v{version} by {copyrightOwnerName} {copyrightYear}";
		}

		private static T GetAssemblyAttribute<T>() {
			return (T)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false)[0];
		}
	}
}
