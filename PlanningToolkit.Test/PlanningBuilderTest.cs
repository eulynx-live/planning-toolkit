using Models.TopoModels.EULYNX.db;
using Models.TopoModels.EULYNX.rsmCommon;

namespace PlanningToolkit.Test;

public class PlanningBuilderTest
{

    [Fact]
    public void RouteGenerator_ShouldGenerateSimpleRoute()
    {
        var builder = new PlanningBuilder();

        var edge = builder.AddEdge(100m);

        var signalA = builder.AddSignal<RastaSignal>("A", edge, 0.2, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.entry);
        var signalN = builder.AddSignal<RastaSignal>("N", edge, 0.8, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.exit);

        var route = builder.AddRoutes().Single();

        Assert.Equal(signalA, route.EntrySignal);
        Assert.Equal(signalN, route.ExitSignal);
    }

    [Fact]
    public void RouteGenerator_ShouldGenerateSimpleRouteWithTvpSection()
    {
        var builder = new PlanningBuilder();

        var edge = builder.AddEdge(100m);

        var section = builder.AddAxleCountingSection<RastaAxleCountingSection>("101");
        builder.AddAxleCountingHead("-/101", edge, 0.1, new[] { section });
        builder.AddAxleCountingHead("101/-", edge, 0.9, new[] { section });

        var signalA = builder.AddSignal<RastaSignal>("A", edge, 0.2, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.entry);
        var signalN = builder.AddSignal<RastaSignal>("N", edge, 0.8, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.exit);

        var route = builder.AddRoutes().Single();

        Assert.Equal(signalA, route.EntrySignal);
        Assert.Equal(signalN, route.ExitSignal);
        Assert.Equal(section.appliesToTvpSection.@ref, route.TvpSections.Single().id);
    }
}
