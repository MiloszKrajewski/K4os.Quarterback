using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace K4os.RoutR
{
	internal static class Extensions
	{
		private const string UnknownParamName = "<unknown>";
		private const string UnknownParamDescription = "Expression";

		private static ArgumentNullException ShouldNotBeNull(string? name) =>
			new(name ?? UnknownParamName, $"{name ?? UnknownParamDescription} should not be null");

		public static T Required<T>(this T? subject, string? name = null) =>
			subject ?? throw ShouldNotBeNull(name);

		public static T Required<T>(this T? subject, string? name = null) where T: struct =>
			subject ?? throw ShouldNotBeNull(name);

		public static async Task<T> As<T>(this Task<object> task) => (T) await task;
		public static async Task<object?> AsObject<T>(this Task<T> task) => await task;

		public static T[] AsArray<T>(this IEnumerable<T>? collection) =>
			collection switch { null => Array.Empty<T>(), T[] a => a, var x => x.ToArray() };

		public static Task WhenAll(this IEnumerable<Task> tasks) => Task.WhenAll(tasks);

		public static IEnumerable<T> NoNulls<T>(this IEnumerable<T?> sequence) =>
			sequence.Where(e => e is not null)!;
		
		public static T? MinBy<T, R>(this IEnumerable<T> collection, Func<T, R> score)
			where R: IComparable<R>
		{
			var first = true;
			var result = default(T);
			var resultScore = default(R);

			foreach (var item in collection)
			{
				var itemScore = score(item);
				var replace = first || itemScore.CompareTo(resultScore!) < 0;
				if (!replace) continue;

				result = item;
				resultScore = itemScore;
				first = false;
			}

			return result;
		}
		
		public static T MinBy<T, R>(this IEnumerable<T> collection, Func<T, R> score, T fallback)
			where R: IComparable<R> =>
			MinBy(collection, score) ?? fallback;
	}
}
