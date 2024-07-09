# Release Notes

## 1.1.0

* Strong-name the assembly: [#12](https://github.com/LogosBible/bsdiff.net/issues/12).
* Add `net6.0` and `net8.0` target frameworks.
* Enable nullable annotations.
* Add `BinaryPatch.Create(ReadOnlySpan<byte>, ReadOnlySpan<byte>, Stream)` overload.
* Reduce unnecessary allocations.

## 1.0.0

* Initial release.
* Port of bsdiff 4.3 to managed code.
