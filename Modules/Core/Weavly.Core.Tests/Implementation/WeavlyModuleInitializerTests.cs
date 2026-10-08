using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Weavly.Core.Implementation;
using Weavly.Core.Shared.Implementation;
using Wolverine;

namespace Weavly.Core.Tests.Implementation;

public sealed class WeavlyModuleInitializerTests
{
    [Fact]
    public async Task StartAsync_InitializesModulesInDependencyOrder_UsingScopedMessageBus()
    {
        var initializationOrder = new List<string>();
        var dependency = new DependencyModule(initializationOrder);
        var dependent = new DependentModule(initializationOrder);
        var bus = Substitute.For<IMessageBus>();
        await using var services = CreateServiceProvider(bus);
        var initializer = new WeavlyModuleInitializer(
            [dependent, dependency],
            services.GetRequiredService<IServiceScopeFactory>()
        );

        await initializer.StartAsync(CancellationToken.None);

        initializationOrder.ShouldBe(["dependency", "dependent"]);
        dependency.ReceivedBus.ShouldBeSameAs(bus);
        dependent.ReceivedBus.ShouldBeSameAs(bus);
    }

    [Fact]
    public void Constructor_Throws_WhenDependencyIsNotRegistered()
    {
        using var services = CreateServiceProvider(Substitute.For<IMessageBus>());

        Should.Throw<InvalidOperationException>(() =>
            new WeavlyModuleInitializer(
                [new MissingDependencyModule()],
                services.GetRequiredService<IServiceScopeFactory>()
            )
        );
    }

    [Fact]
    public void Constructor_Throws_WhenDependenciesContainCycle()
    {
        using var services = CreateServiceProvider(Substitute.For<IMessageBus>());

        Should.Throw<InvalidOperationException>(() =>
            new WeavlyModuleInitializer(
                [new CircularModuleA(), new CircularModuleB()],
                services.GetRequiredService<IServiceScopeFactory>()
            )
        );
    }

    [Fact]
    public async Task StartAsync_PropagatesInitializationFailure_AndDoesNotInitializeFollowingModules()
    {
        var initializationOrder = new List<string>();
        await using var services = CreateServiceProvider(Substitute.For<IMessageBus>());
        var initializer = new WeavlyModuleInitializer(
            [new FailingModule(), new FollowingModule(initializationOrder)],
            services.GetRequiredService<IServiceScopeFactory>()
        );

        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            initializer.StartAsync(CancellationToken.None)
        );

        exception.Message.ShouldBe("Initialization failed");
        initializationOrder.ShouldBeEmpty();
    }

    [Fact]
    public async Task StopAsync_CompletesSuccessfully()
    {
        await using var services = CreateServiceProvider(Substitute.For<IMessageBus>());
        var initializer = new WeavlyModuleInitializer([], services.GetRequiredService<IServiceScopeFactory>());

        await Should.NotThrowAsync(() => initializer.StopAsync(CancellationToken.None));
    }

    private static ServiceProvider CreateServiceProvider(IMessageBus bus) =>
        new ServiceCollection().AddScoped(_ => bus).BuildServiceProvider();

    private sealed class DependencyModule(List<string> initializationOrder) : WeavlyModule
    {
        public IMessageBus? ReceivedBus { get; private set; }

        public override Task InitializeAsync(IMessageBus bus)
        {
            ReceivedBus = bus;
            initializationOrder.Add("dependency");
            return Task.CompletedTask;
        }
    }

    private sealed class DependentModule(List<string> initializationOrder) : WeavlyModule
    {
        public IMessageBus? ReceivedBus { get; private set; }

        public override IReadOnlyCollection<string> InitializationDependencies => [nameof(DependencyModule)];

        public override Task InitializeAsync(IMessageBus bus)
        {
            ReceivedBus = bus;
            initializationOrder.Add("dependent");
            return Task.CompletedTask;
        }
    }

    private sealed class MissingDependencyModule : WeavlyModule
    {
        public override IReadOnlyCollection<string> InitializationDependencies => ["NotRegisteredModule"];
    }

    private sealed class CircularModuleA : WeavlyModule
    {
        public override IReadOnlyCollection<string> InitializationDependencies => [nameof(CircularModuleB)];
    }

    private sealed class CircularModuleB : WeavlyModule
    {
        public override IReadOnlyCollection<string> InitializationDependencies => [nameof(CircularModuleA)];
    }

    private sealed class FailingModule : WeavlyModule
    {
        public override Task InitializeAsync(IMessageBus bus) =>
            Task.FromException(new InvalidOperationException("Initialization failed"));
    }

    private sealed class FollowingModule(List<string> initializationOrder) : WeavlyModule
    {
        public override Task InitializeAsync(IMessageBus bus)
        {
            initializationOrder.Add("following");
            return Task.CompletedTask;
        }
    }
}