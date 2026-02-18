namespace SkyTower.Domain.Extensions;

/// <summary>
/// Extension methods for <see cref="IReadOnlyList{T}"/>.
/// </summary>
public static class ReadOnlyListExtensions
{
	/// <summary>
	/// Finds the index of the first element in the list that satisfies the specified predicate. Returns -1 if no such element is found.
	/// </summary>
	/// <param name="list"></param>
	/// <param name="predicate"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static int FindIndex<T>(this IReadOnlyList<T> list, Func<T, bool> predicate)
	{
		try
		{
			return list.Select((element, index) => (element, index)).First(item => predicate(item.element)).index;
		}
		catch (InvalidOperationException)
		{
			return -1;
		}
	}
}