using FluentAssertions;
using Models.TopoModels.EULYNX.db;
using Models.TopoModels.EULYNX.rsmCommon;
using PlanningToolkit.PlanningAutomation;

namespace PlanningToolkit.Test;

public class RouteBuilderTest
{
    [Fact]
    public void RouteBuilder_GetOverlappingTvpSections_ShouldWorkForSimpleCase()
    {
        var builder = new PlanningBuilder();

        var edge = builder.AddEdge(100.0, "edge");

        var h1 = builder.AddAxleCountingHead("-/101", edge, 0.1);
        var h2 = builder.AddAxleCountingHead("101/-", edge, 0.9);
        var section = builder.AddAxleCountingSection<RastaAxleCountingSection>("101")
            .WithAssociatedHead(h1).WithAssociatedHead(h2).Build();

        var signalA = builder.AddSignal<RastaSignal>("A", edge, 0.2, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.entry);
        var signalN = builder.AddSignal<RastaSignal>("N", edge, 0.8, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.exit);

        var routeBuilder = new RouteBuilder(builder);
        var route = routeBuilder.AddRoutes().Single();

        Assert.Equal(1, route.TvpSections.Count);
    }

    [Fact]
    public void RouteBuilder_ShouldGenerateSimpleRoute()
    {
        var builder = new PlanningBuilder();

        var edge = builder.AddEdge(100.0, "edge");

        var signalA = builder.AddSignal<RastaSignal>("A", edge, 0.2, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.entry);
        var signalN = builder.AddSignal<RastaSignal>("N", edge, 0.8, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.exit);

        var routeBuilder = new RouteBuilder(builder);
        var route = routeBuilder.AddRoutes().Single();

        Assert.Equal(signalA, route.EntrySignal);
        Assert.Equal(signalN, route.ExitSignal);
    }

    [Fact]
    public void RouteBuilder_ShouldGenerateSimpleRouteWithTvpSection()
    {
        var builder = new PlanningBuilder();

        var edge = builder.AddEdge(100.0, "edge");

        var h1 = builder.AddAxleCountingHead("-/101", edge, 0.1);
        var h2 = builder.AddAxleCountingHead("101/-", edge, 0.9);
        var section = builder.AddAxleCountingSection<RastaAxleCountingSection>("101")
            .WithAssociatedHead(h1).WithAssociatedHead(h2).Build();

        var signalA = builder.AddSignal<RastaSignal>("A", edge, 0.2, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.entry);
        var signalN = builder.AddSignal<RastaSignal>("N", edge, 0.8, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.exit);

        var routeBuilder = new RouteBuilder(builder);
        var route = routeBuilder.AddRoutes().Single();

        Assert.Equal(signalA, route.EntrySignal);
        Assert.Equal(signalN, route.ExitSignal);
        Assert.Equal(section.appliesToTvpSection!.@ref, route.TvpSections.Single().id);
    }
}
