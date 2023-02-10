
using System;
using System.IO;
using BsDiff;

namespace BsPatch
{
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

			string oldFile = args[0];
			string newFile = args[1];
			string patchFile = args[2];

			try
			{
				using (FileStream input = new FileStream(oldFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				using (FileStream output = new FileStream(newFile, FileMode.Create))
					BinaryPatchUtility.Apply(input, () => new FileStream(patchFile, FileMode.Open, FileAccess.Read, FileShare.Read), output);
			}
			catch (FileNotFoundException ex)
			{
				Console.Error.WriteLine("Could not open '{0}'.", ex.FileName);
			}
		}
	}
}
