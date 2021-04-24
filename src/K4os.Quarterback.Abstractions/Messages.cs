namespace K4os.Quarterback.Abstractions
{
	/// <summary>Marker interface for events.</summary>
	public interface IEvent { }

	/// <summary>Marker interface for commands.</summary>
	public interface ICommand { }

	/// <summary>Marker interface for requests and queries.</summary>
	public interface IRequest<TResponse> { }
}
