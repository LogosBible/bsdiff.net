using System.Diagnostics;
using BsDiff;

namespace BsDiffTool
{
	class Program
	{
		public static void Main(string[] args)
		{
			// check for correct usage
			if (args.Length != 3)
			{
				Console.Error.WriteLine("bsdiff oldfile newfile patchfile");
				return;
			}

			// check for special command-line switch that performs a self-test
			if (args[0] == "--check")
				CheckImplementation(args[1], args[2]);
			else
			{
				var oldFile = args[0];
				var newFile = args[1];
				var patchFile = args[2];

				try
				{
					using (var output = new FileStream(patchFile, FileMode.Create))
						BinaryPatch.Create(File.ReadAllBytes(oldFile), File.ReadAllBytes(newFile), output);
				}
				catch (FileNotFoundException ex)
				{
					Console.Error.WriteLine("Could not open '{0}'.", ex.FileName);
				}
			}
		}

		// Creates a patch for oldFile and newFile with the reference C implementation and this C# code, then verifies that
		// the patch applies correctly.
		private static void CheckImplementation(string oldFile, string newFile)
		{
			// to test, download bsdiff and bspatch for Windows, e.g., from http://sites.inka.de/tesla/others.html#bsdiff
			const string bsdiffPath = @"C:\Util\bsdiff.exe";
			const string rbspatchPath = @"C:\Util\bspatch.exe";

			var tempPath = Path.GetTempPath();
			var referencePatchFileName = Path.Combine(tempPath, "reference.patch");
			var portPatchFileName = Path.Combine(tempPath, "port.patch");

			// run reference implementation
			var referenceTime = Stopwatch.StartNew();
			using (var process = Process.Start(bsdiffPath, QuotePaths(oldFile, newFile, referencePatchFileName)))
			{
				process.WaitForExit();
				referenceTime.Stop();
			}

			// run C# implementation
			var portTime = Stopwatch.StartNew();
			using (var output = new FileStream(portPatchFileName, FileMode.Create))
				BinaryPatch.Create(File.ReadAllBytes(oldFile), File.ReadAllBytes(newFile), output);
			portTime.Stop();

			Console.WriteLine("Patches created in {0} (reference) and {1} (C# port).", referenceTime.Elapsed, portTime.Elapsed);
			Console.WriteLine("File sizes (in bytes) are {0:n0} (reference) and {1:n0} (C# port).", new FileInfo(referencePatchFileName).Length, new FileInfo(portPatchFileName).Length);

			var outputFilePaths = new[] { "test-ref-ref.dat", "test-prt-ref.dat", "test-ref-prt.dat", "test-prt-prt.dat" }
				.Select(fn => Path.Combine(tempPath, fn))
				.ToArray();

			Console.Write("Applying reference patch with reference binary...");
			using (var process = Process.Start(rbspatchPath, QuotePaths(oldFile, outputFilePaths[0], referencePatchFileName)))
				process.WaitForExit();
			Console.WriteLine("done.");

			Console.Write("Applying port patch with reference binary...");
			using (var process = Process.Start(rbspatchPath, QuotePaths(oldFile, outputFilePaths[1], portPatchFileName)))
				process.WaitForExit();
			Console.WriteLine("done.");

			Console.Write("Applying reference patch with port binary...");
			using (var input = new FileStream(oldFile, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var output = new FileStream(outputFilePaths[2], FileMode.Create))
				BinaryPatch.Apply(input, () => new FileStream(referencePatchFileName, FileMode.Open, FileAccess.Read, FileShare.Read), output);
			Console.WriteLine("done.");

			Console.Write("Applying port patch with port binary...");
			using (var input = new FileStream(oldFile, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var output = new FileStream(outputFilePaths[3], FileMode.Create))
				BinaryPatch.Apply(input, () => new FileStream(portPatchFileName, FileMode.Open, FileAccess.Read, FileShare.Read), output);
			Console.WriteLine("done.");

			var errors = 0;
			var expectedOutput = File.ReadAllBytes(newFile);
			foreach (var filePath in outputFilePaths)
			{
				var actualOutput = File.ReadAllBytes(filePath);
				if (!expectedOutput.SequenceEqual(actualOutput))
				{
					Console.Error.WriteLine("Incorrect results in {0}.", Path.GetFileName(filePath));
					errors++;
				}
			}

			if (errors == 0)
				Console.WriteLine("All patches are correct.");
		}

		// Returns a single string that contains all paths quoted and joined with spaces.
		private static string QuotePaths(params string[] paths)
		{
			return "\"" + string.Join("\" \"", paths) + "\"";
		}
	}
}
