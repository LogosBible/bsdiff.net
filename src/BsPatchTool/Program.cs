using BsDiff;

namespace BsPatchTool;

class Program
{
	static void Main(string[] args)
	{
		// check for correct usage
		if (args.Length != 3)
		{
			Console.Error.WriteLine("bsdiff oldfile newfile patchfile");
			return;
		}

		var oldFile = args[0];
		var newFile = args[1];
		var patchFile = args[2];

		try
		{
			using (var input = new FileStream(oldFile, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var output = new FileStream(newFile, FileMode.Create))
				BinaryPatch.Apply(input, () => new FileStream(patchFile, FileMode.Open, FileAccess.Read, FileShare.Read), output);
		}
		catch (FileNotFoundException ex)
		{
			Console.Error.WriteLine("Could not open '{0}'.", ex.FileName);
		}
	}
}
