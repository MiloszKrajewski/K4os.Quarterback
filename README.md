# K4os.Quarterback

[![NuGet Stats](https://img.shields.io/nuget/v/K4os.Quarterback.svg?label=K4os.Quarterback&logo=nuget)](https://www.nuget.org/packages/K4os.Quarterback)
[![NuGet Stats](https://img.shields.io/nuget/v/K4os.Quarterback.Abstractions.svg?label=K4os.Quarterback.Abstractions&logo=nuget)](https://www.nuget.org/packages/K4os.Quarterback.Abstractions)

# TL;DR

Quarterback is yet another implementation of 
[mediator pattern](https://en.wikipedia.org/wiki/Mediator_pattern) for .NET.
It started as learning project with usual "how complicated it may be?".

It is very similar to [MediatR](https://github.com/jbogard/MediatR), 
it has very similar API and performance characteristics.
The differences are mainly a matter of different opinion (in other words: bike shedding).

# What Quarterback does

We already have one mediator in .NET world and it is widely accepted as gold standard of simple mediators: 
[MediatR](https://github.com/jbogard/MediatR).

As **MediatR**'s subtitle is "simple, unambitious mediator implementation in .NET" I'm afraid 
I have to say that **Quarterback** is probably even more unambitious. 

Its only goal is to find the right handler for given message, right wrappers for that handler, 
and executed them all together. 

**Quarterback** depends on .NET `IServiceProvider`. It does not care which one you use, 
but also it does not work without one. It may behave slightly differently depending on your 
DI container. It is a leaky abstraction, you may need to configure things differently 
depending on which DI container you use. 

It is tested with [DryIoc](https://github.com/dadhi/DryIoc) but, as I said before, will work with any.

# Handlers

There are three types of handlers is **Quarterback**:

* **Command**: action, one handler, no result 
* **Event**: notification, zero or multiple handlers, no result
* **Request**: request or query, one handler, with result

As a learning project (remember?) I started with very canonical *commands*, *events*, and *queries* 
but queries which were used to modify data didn't feel right so I renamed them to request.
**Request** is either a command which returns value or query (which returns value by definition).

To implement a handler you need to implement one of these interfaces:

```c#
interface ICommandHandler<in TCommand>
{
    Task Handle(TCommand command, CancellationToken token);
}

interface IEventHandler<in TEvent>
{
    Task Handle(TEvent @event, CancellationToken token);
}

interface IRequestHandler<in TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken token);
}
```

(NOTE: if you read carefully you may notice that `TResponse` could be a covariant, 
I mean like in `IRequestHandler<in TRequest, out TResponse>`, but unfortunately this does 
not work with `Task<T>`).

To send a message all you need is `IServiceProvider` as **Quarterback** has been implemented
as extension methods for `IServiceProvider`.

```c#
await provider.Send(new Command());
await provider.Publish(new Event());
await provider.Expecting<Response>().Request(new Request()); 
```

As long a handlers are registered in DI container, messages will be delivered to appropriate handler.

Please note, the messages are just classes. What kind of handler is used depends solely on the method
which was used to send it. There are no `ICommand`, or `IEvent` or `IRequest<TResponse>` interfaces.
Because **Quarterback** does not care what is inside your messages it does not care what is the type.
You can technically send anything you want:

```c#
var result = await publisher.Expecting<double>().Request("2+2");
```

This design decision is a trade-off. **Quarterback** cannot validate what and how you send.
Maybe you are trying to send an event as request? If you think you need this, it is still 
doable to build abstraction on top of it and use **Quarterback** engine but not **Quarterback** 
API directly. On the other hand, it allows to send 3rd party commands, events or requests though
**Quarterback** without some mapping layer. I know, a puppy probably died somewhere in the world
at the exact moment I came up with this idea.

# Pipeline

Sometimes handlers need to wrapped in some cross-cutting concerns. Logging, performance metrics, 
retry policies, filtering, rerouting, and whatnot. Technically, it could be called from inside 
the handler and generalized in some base class. 
But, it can be done with so called pipeline. They are wrappers around handlers.

Pipeline handler must implement one of these interfaces:

```c#
interface ICommandPipeline<in THandler, in TCommand>
    where THandler: ICommandHandler<TCommand>
{
    Task Handle(THandler handler, TCommand command, Func<Task> next, CancellationToken token);
}

interface IEventPipeline<in THandler, in TEvent>
    where THandler: IEventHandler<TEvent>
{
    Task Handle(THandler handler, TEvent @event, Func<Task> next, CancellationToken token);
}

interface IRequestPipeline<in THandler, in TRequest, TResponse>
    where THandler: IRequestHandler<TRequest, TResponse>
{
    Task<TResponse> Handle(
        THandler handler, TRequest request, Func<Task<TResponse>> next,
        CancellationToken token);
}
```

To get most from this feature (I would even say: to make usable) your DI container
needs to support "constrained open generics".
Without it you have only two choices: wrap every handler separately 
(it defeats the idea of cross-cutting concerns) or all of them (which maybe works for logging
but not for database retry policy, as not every operation is a database operation).  

```c#
class WrapperForA: ICommandPipeline<HandlerA, CommandA> { ... }
class WrapperForB: ICommandPipeline<HandlerB, CommandB> { ... }
class WrapperForAll<H, C>: ICommandPipeline<H, C> { ... }
```

with "constrained open generics" it possible to wrap only certain handler or command types:

```c#
class RetryWrapper<H, C>: ICommandPipeline<H, C> 
    where H: IDatabaseOperation { ... }
```

so this wrapper will be used only around handlers which are also `IDatabaseOperation`.

Please note, availability of this feature depends on DI container. 
[DryIoc](https://github.com/dadhi/DryIoc) does support it. I assume, but I did not test it,
[Microsoft.Extensions.DependencyInjection also does it](https://jimmybogard.com/constrained-open-generics-support-merged-in-net-core-di-container/)
(maybe not the same way).


# Build

```shell
paket install
fake build
```

# Performance

Performance was not a goal, but it can be measured.
It seem that **Quarterback** is a little bit faster than **MediatR** when 
dispatching messages dynamically (~1000ns faster), but a little bit slower 
for static invocation (~200ns).

This is actually understandable as **Quarterback** does not really implement 
static resolution, it always uses dynamic resolution, even if types are statically known.
There is some space for improvement.

```
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
AMD Ryzen 5 3600, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=5.0.103
  [Host]     : .NET Core 5.0.3 (CoreCLR 5.0.321.7212, CoreFX 5.0.321.7212), X64 RyuJIT
  DefaultJob : .NET Core 5.0.3 (CoreCLR 5.0.321.7212, CoreFX 5.0.321.7212), X64 RyuJIT


|                Method |       Mean |    Error |   StdDev |
|---------------------- |-----------:|---------:|---------:|
|     UseDynamicMediatR | 1,874.8 ns | 35.20 ns | 32.92 ns |
| UseDynamicQuarterback |   855.6 ns |  2.49 ns |  2.21 ns |
|      UseStaticMediatR |   629.9 ns |  2.17 ns |  1.92 ns |
|  UseStaticQuarterback |   849.8 ns |  2.31 ns |  2.04 ns |
```

Please note, what scale we are talking about. 
Even slowest handler above (dynamic **MediatR**) is ~2000ns, which is 2μs, which is 0.002ms. 
If your request takes 20ms (which is relatively quick) resolving handler is 0.0001 of a 
whole request (1% of 1%).

This is not the time you should worry about.
