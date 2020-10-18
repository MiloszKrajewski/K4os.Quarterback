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

		private static ArgumentNullException ShouldNotBeNull(string name) =>
			new ArgumentNullException(
				name ?? UnknownParamName,
				$"{name ?? UnknownParamDescription} should not be null");

		public static T Required<T>(this T subject, string name = null) =>
			subject ?? throw ShouldNotBeNull(name);

		public static T Required<T>(this T? subject, string name = null) where T: struct =>
			subject ?? throw ShouldNotBeNull(name);

		public static async Task<T> As<T>(this Task<object> task) => (T) await task;
		public static async Task<object> AsObject<T>(this Task<T> task) => await task;

		public static T[] AsArray<T>(this IEnumerable<T> collection) =>
			collection is null ? Array.Empty<T>() :
			collection is T[] array ? array :
			collection.ToArray();

		public static Task WhenAll(this IEnumerable<Task> tasks) => Task.WhenAll(tasks);

		public static T MinBy<T, R>(
			this IEnumerable<T> collection, Func<T, R> score, T fallback = default)
			where R: IComparable<R>
		{
			var first = true;
			var result = fallback;
			var resultScore = default(R);

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
	}
}
