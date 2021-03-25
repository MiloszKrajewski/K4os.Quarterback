using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace K4os.Quarterback.Test
{
	public static class TypeExtensions
	{
		private static readonly ConcurrentDictionary<Type, string> FriendlyNames =
			new ConcurrentDictionary<Type, string>();

		public static string GetFriendlyName(this Type type) => 
			FriendlyNames.GetOrAdd(type, NewFriendlyName);

		private static string NewFriendlyName(Type type)
		{
			if (type == null) return "<null>";

			var typeName = type.Name;
			if (!type.IsGenericType)
				return typeName;

			var length = typeName.IndexOf('`');
			if (length < 0) length = typeName.Length;

			return new StringBuilder()
				.Append(typeName, 0, length)
				.Append('<')
				.Append(string.Join(",", type.GetGenericArguments().Select(GetFriendlyName)))
				.Append('>')
				.ToString();
		}
	}
}
