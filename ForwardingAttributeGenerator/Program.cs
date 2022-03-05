using AsmResolver.DotNet;
using System;
using System.IO;
using System.Text;

namespace ForwardingAttributeGenerator
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			if(args.Length != 1)
			{
				Console.WriteLine("This program takes exactly one argument");
			}
			else
			{
				try
				{
					string path = args[0];
					AssemblyDefinition assembly = AssemblyDefinition.FromFile(path);
					StringBuilder sb = new StringBuilder();
					sb.AppendLine("using System.Runtime.CompilerServices;");
					sb.AppendLine();
					foreach(ModuleDefinition module in assembly.Modules)
					{
						foreach(TypeDefinition type in module.TopLevelTypes)
						{
							//Only top level types need forwarding attributes
							sb.AppendForwardingAttributeLines(type);
						}
					}
					string outputPath = Path.Combine(AppContext.BaseDirectory, $"{Path.GetFileNameWithoutExtension(path)}.cs");
					File.WriteAllText(outputPath, sb.ToString());
					Console.WriteLine("Done!");
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}

			Console.ReadLine();
		}


		private static void AppendForwardingAttributeLines(this StringBuilder sb, TypeDefinition type)
		{
			if (!type.IsPublic)
				return;

			string fullName = type.FullName
				.Replace('+', '.')
				.Replace("`1","<>")
				.Replace("`2", "<,>")
				.Replace("`3", "<,,>")
				.Replace("`4", "<,,,>")
				.Replace("`5", "<,,,,>")
				.Replace("`6", "<,,,,,>")
				.Replace("`7", "<,,,,,,>")
				.Replace("`8", "<,,,,,,,>");
			
			string attributeLine = $"[assembly: TypeForwardedTo(typeof({fullName}))]";
			sb.AppendLine(attributeLine);
		}
	}
}
