using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace K4os.RoutR.Internals
{
	internal static class TypeExtensions
	{
		/// <summary>
		/// Type distance cache. It should Concurrent dictionary but it is not available
		/// on all flavors of Portable Class Library.
		/// </summary>
		private static readonly ConcurrentDictionary<(Type, Type), int> TypeDistanceMap =
			new ConcurrentDictionary<(Type, Type), int>();

		/// <summary>Checks if child type inherits (or implements) from parent.</summary>
		/// <param name="child">The child.</param>
		/// <param name="parent">The parent.</param>
		/// <returns><c>true</c> if child type inherits (or implements) from parent; <c>false</c> otherwise</returns>
		public static bool InheritsFrom(this Type child, Type parent) =>
			parent.IsAssignableFrom(child);

		/// <summary>Calculates distance between child and parent type.</summary>
		/// <param name="child">The child.</param>
		/// <param name="grandparent">The parent.</param>
		/// <returns>Inheritance distance between child and parent.</returns>
		/// <exception cref="System.ArgumentException">Thrown when child does not inherit from parent at all.</exception>
		public static int DistanceFrom(this Type child, Type grandparent)
		{
			if (child is null)
				throw new ArgumentNullException(nameof(child));
			if (grandparent is null)
				throw new ArgumentNullException(nameof(grandparent));

			if (child == grandparent)
				return 0;

			return TypeDistanceMap.GetOrAdd((child, grandparent), ResolveDistance);
		}

		private static int ResolveDistance((Type, Type) types)
		{
			var (child, grandparent) = types;
			if (child == grandparent)
				return 0;

			if (!child.InheritsFrom(grandparent))
				throw new ArgumentException(
					$"Type '{child.Name}' does not inherit nor implements '{grandparent.Name}'");

			int Up1(int value) => value == int.MaxValue ? value : value + 1;

			var distances = GetIntermediateParents(child, grandparent)
				.Select(t => Up1(DistanceFrom(t, grandparent)))
				.ToArray();

			// this may happen with covariant interfaces
			// they are "assignable from" but not "inheriting" from each other
			return distances.Length == 0 ? int.MaxValue : distances.Min();
		}

		/// <summary>Gets the list of parent types which also inherit for grandparent.</summary>
		/// <param name="child">The child.</param>
		/// <param name="grandparent">The parent.</param>
		/// <returns>Collection of types.</returns>
		private static IEnumerable<Type> GetIntermediateParents(Type child, Type grandparent)
		{
			var baseType = child.BaseType;

			if (grandparent.IsInterface)
			{
				// determines if given interface "leads" to grandparent
				// and if child is first implementor of given interface
				// note: this is special case for interfaces as they are reported on every child
				// along the way, and we want the most distant one (when it was implemented
				// for the first time in hierarchy)
				bool IsFirstImplementation(Type interfaceType) =>
					interfaceType.InheritsFrom(grandparent) && // right path 
					(baseType == null || !baseType.InheritsFrom(interfaceType)); // first time

				var baseInterfaces = child.GetInterfaces().Where(IsFirstImplementation);
				foreach (var i in baseInterfaces)
					yield return i;
			}

			if (baseType.InheritsFrom(grandparent))
				yield return baseType;
		}

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

		public static string GetObjectFriendlyName(this object subject)
		{
			if (subject == null) return "<null>";

			return string.Format(
				"{0}@{1:x}",
				subject.GetType().GetFriendlyName(),
				RuntimeHelpers.GetHashCode(subject));
		}
	}
}
