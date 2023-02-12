#if !NET7_0_OR_GREATER
namespace BsDiff;

internal static class MemoryExtensions
{
	public static int CommonPrefixLength(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> other)
	{
		int index;
		for (index = 0; index < span.Length && index < other.Length; index++)
		{
			if (span[index] != other[index])
				break;
		}
		return index;
	}
}
#endif
