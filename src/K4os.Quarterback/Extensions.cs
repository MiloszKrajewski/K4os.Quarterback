using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace K4os.Quarterback
{
	internal static class Extensions
	{
		private const string UnknownParamName = "<unknown>";
		private const string UnknownParamDescription = "Expression";

		private static ArgumentNullException ShouldNotBeNull(string? name) =>
			new(name ?? UnknownParamName, $"{name ?? UnknownParamDescription} should not be null");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Required<T>(this T? subject, string? name = null) =>
			subject ?? throw ShouldNotBeNull(name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Required<T>(this T? subject, string? name = null) where T: struct =>
			subject ?? throw ShouldNotBeNull(name);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async Task<object?> Box<T>(this Task<T> task) => await task;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async Task<T> Unbox<T>(this Task<object> task) => (T) await task;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] AsArray<T>(this IEnumerable<T>? sequence) =>
			sequence switch { null => Array.Empty<T>(), T[] a => a, _ => sequence.ToArray() };

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static V? GetOrNull<K, V>(this ConcurrentDictionary<K, V> dictionary, K key) => 
			dictionary.TryGetValue(key, out var result) ? result : default;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Task WhenAll(this IEnumerable<Task> tasks) => Task.WhenAll(tasks);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> NoNulls<T>(this IEnumerable<T?> sequence) =>
			sequence.Where(e => e is not null)!;
		
		public static T? MinBy<T, R>(this IEnumerable<T> collection, Func<T, R> score)
			where R: IComparable<R>
		{
			var first = true;
			var result = default(T?);
			var resultScore = default(R)!;

			foreach (var item in collection)
			{
				var itemScore = score(item);
				var replace = first || itemScore.CompareTo(resultScore) < 0;
				if (!replace) continue;

				result = item;
				resultScore = itemScore;
				first = false;
			}

			return result;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T MinBy<T, R>(this IEnumerable<T> collection, Func<T, R> score, T fallback)
			where R: IComparable<R> =>
			MinBy(collection, score) ?? fallback;
	}
}
