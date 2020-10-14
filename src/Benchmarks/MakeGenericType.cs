using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
	public class MakeGenericType
	{
		private static readonly ConcurrentDictionary<(Type, Type), Type> Types =
			new ConcurrentDictionary<(Type, Type), Type>();

		private static readonly Type Type1 = typeof(string);
		private static readonly Type Type2 = typeof(int);

		[Benchmark]
		public void JustCreate() { _ = typeof(IDictionary<,>).MakeGenericType(Type1, Type2); }

		[Benchmark]
		public void GetOrCreate() { _ = GetConstructedType(Type1, Type2); }

		private static Type GetConstructedType(Type type1, Type type2)
		{
			return Types.GetOrAdd((type1, type2), ConstructNewType);
		}

		// ReSharper disable AssignNullToNotNullAttribute
		private static Type ConstructNewType((Type type1, Type type2) types) =>
			typeof(IDictionary<,>).MakeGenericType(types.type1, types.type2);
		// ReSharper restore AssignNullToNotNullAttribute
	}
}
