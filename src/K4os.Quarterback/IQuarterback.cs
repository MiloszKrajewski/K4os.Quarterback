using System;

namespace K4os.Quarterback
{
	/// <summary>
	/// Used to explicitly indicate that you need it for purpose of sending messages.
	/// It is not really needed, it just make code base more readable.
	/// Technically, it is just <see cref="IServiceProvider"/>. 
	/// </summary>
	public interface IQuarterback: IServiceProvider { }
}
