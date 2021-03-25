using System;
using DryIoc;
using System.Linq;
using System.Threading.Tasks;
using DryIoc.Microsoft.DependencyInjection;
using K4os.Quarterback.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace K4os.Quarterback.Test.Scopes
{
	public class ScopeTests
	{
		private readonly Container _container = new();

		public ScopeTests()
		{
			ScopeCounter.Reset();
			_container.Use<IServiceScopeFactory>(r => new DryIocServiceScopeFactory(r));
			_container.RegisterMany(
				new[] { typeof(TestHandler), typeof(TestPipeline) }, 
				Reuse.Transient);
			_container.Register<IBroker, Classic.Quarterback>(Reuse.Transient);
		}

		[Fact]
		public void ScopeIsDifferentWhenTransient()
		{
			_container.Register<ScopeCounter>(Reuse.Transient);
			var provider = (IServiceProvider) _container;

			var id1 = provider.GetService<ScopeCounter>().Instance;
			var id2 = provider.GetService<ScopeCounter>().Instance;

			Assert.Equal(id2, id1 + 1);
		}

		[Fact]
		public void ScopeIsTheSameWhenSingleton()
		{
			_container.Register<ScopeCounter>(Reuse.ScopedOrSingleton);
			var provider = (IServiceProvider) _container;

			var id1 = provider.GetService<ScopeCounter>().Instance;
			var id2 = provider.GetService<ScopeCounter>().Instance;

			Assert.Equal(id2, id1);
		}

		[Fact]
		public void ScopeIsTheSameWhenScoped()
		{
			_container.Register<ScopeCounter>(Reuse.ScopedOrSingleton);

			var root = (IServiceProvider) _container;

			var id0 = root.GetService<ScopeCounter>().Instance;

			var scope = root.CreateScope().ServiceProvider;

			var id1 = scope.GetService<ScopeCounter>().Instance;
			var id2 = scope.GetService<ScopeCounter>().Instance;

			Assert.Equal(id1, id0 + 1);
			Assert.Equal(id2, id1);
		}

		[Fact]
		public void ScopedObjectsDoNotResolveInRoot()
		{
			_container.Register<ScopeCounter>(Reuse.Scoped);

			var root = (IServiceProvider) _container;

			var obj0 = root.GetService<ScopeCounter>();

			var scope = root.CreateScope().ServiceProvider;

			var obj1 = scope.GetService<ScopeCounter>();

			Assert.Null(obj0);
			Assert.NotNull(obj1);
		}

		[Fact]
		public async Task CommandsExecutedInSameScopeShareScopedDependency()
		{
			_container.Register<ScopeCounter>(Reuse.Scoped);
			var scope = _container.CreateScope().ServiceProvider;

			var message = new TestCommand();
			await scope.Send(message);
			
			Assert.Equal("1,1,1", message.Trace());
		}
		
		[Fact]
		public async Task CommandsExecutedInSameScopeDontShareTransientDependency()
		{
			_container.Register<ScopeCounter>(Reuse.Transient);
			var scope = _container.CreateScope().ServiceProvider;

			var message = new TestCommand();
			await scope.Send(message);
			
			// NOTE: It is 2,1,2 not 1,2,1 because handler is resolved first
			// while pipeline is resolved later (because it depends on handler)
			Assert.Equal("2,1,2", message.Trace());
		}
		
		[Fact]
		public async Task TwoMessagesInSameScopeDontCreateNewDependencies()
		{
			_container.Register<ScopeCounter>(Reuse.Scoped);
			var scope = _container.CreateScope().ServiceProvider;

			var message1 = new TestCommand();
			await scope.Send(message1);
			var message2 = new TestCommand();
			await scope.Send(message2);
			
			Assert.Equal("1,1,1", message1.Trace());
			Assert.Equal("1,1,1", message1.Trace());
		}
		
		[Fact]
		public async Task TwoMessagesInTwoScopesHaveTheirOwnDependencies()
		{
			_container.Register<ScopeCounter>(Reuse.Scoped);
			var scope1 = _container.CreateScope().ServiceProvider;
			var scope2 = _container.CreateScope().ServiceProvider;

			var message1 = new TestCommand();
			await scope1.Send(message1);
			var message2 = new TestCommand();
			await scope2.Send(message2);
			
			Assert.Equal("1,1,1", message1.Trace());
			Assert.Equal("2,2,2", message2.Trace());
		}

		[Fact]
		public async Task QuarterbackRespectsCurrentScope()
		{
			_container.Register<ScopeCounter>(Reuse.Scoped);

			var scope1 = _container.CreateScope().ServiceProvider;
			var scope2 = _container.CreateScope().ServiceProvider;

			var message1 = new TestCommand();
			var message2 = new TestCommand();
			var message3 = new TestCommand();

			var qb1A = scope1.GetRequiredService<IBroker>();
			var qb2 = scope2.GetRequiredService<IBroker>();
			var qb1B = scope1.GetRequiredService<IBroker>();

			await qb1A.Send(message1);
			await qb2.Send(message2);
			await qb1B.Send(message3);
			
			// different scopes
			Assert.Equal("1,1,1", message1.Trace());
			Assert.Equal("2,2,2", message2.Trace());
			
			// same scope, different quarterbacks
			Assert.NotSame(qb1A, qb1B);
			Assert.Equal("1,1,1", message3.Trace());
		}
	}
}
