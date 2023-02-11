## About

BsDiff is a 100% managed implementation of Colin Percival's [bsdiff algorithm](https://www.daemonology.net/bsdiff/).
It provides functions to create a patch between two binary files and to apply that patch (to the first file, producing the second file).
The patch is usually much smaller than the size of the second file, so this can be used to optimize download size.

## Usage

Given two existing files, you can create a patch as follows:

```csharp
var oldFileBytes = File.ReadAllBytes("oldFile");
var newFileBytes = File.ReadAllBytes("newFile");
using var outputStream = File.Create("patchFile");
BinaryPatch.Create(oldFileBytes, newFileBytes, outputStream);
```

You can then apply the patch to the old file to produce the new file:

```csharp
using var oldFile = File.OpenRead("oldFile");
using var newFile = File.Create("newFile");
BinaryPatch.Apply(oldFile, () => File.OpenRead("patchFile"), newFile);
```
