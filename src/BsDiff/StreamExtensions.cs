#if !NET7_0_OR_GREATER
using System.Buffers;

namespace BsDiff;

internal static class StreamExtensions
{
	public static void ReadExactly(this Stream stream, Span<byte> buffer)
	{
		var bytes = ArrayPool<byte>.Shared.Rent(buffer.Length);
		ReadExactly(stream, bytes, 0, buffer.Length);
		bytes.AsSpan(0, buffer.Length).CopyTo(buffer);
		ArrayPool<byte>.Shared.Return(bytes);
	}

	public static void ReadExactly(this Stream stream, byte[] buffer, int offset, int length)
	{
		var bytesCopied = 0;
		while (bytesCopied < length)
		{
			var bytesRead = stream.Read(buffer, offset + bytesCopied, length - bytesCopied);
			if (bytesRead == 0)
				throw new EndOfStreamException();
			bytesCopied += bytesRead;
		}
	}

	public static void Write(this Stream stream, ReadOnlySpan<byte> buffer)
	{
		var bytes = ArrayPool<byte>.Shared.Rent(buffer.Length);
		buffer.CopyTo(bytes.AsSpan(0, buffer.Length));
		stream.Write(bytes, 0, buffer.Length);
		ArrayPool<byte>.Shared.Return(bytes);
	}
}
#endif
