using Models.TopoModels.EULYNX.db;
using Models.TopoModels.EULYNX.rsmCommon;
using Models.TopoModels.EULYNX.rsmTrack;
using PlanningToolkit;
using PlanningToolkit.PlanningAutomation;

var builder = new PlanningBuilder();

// Edges
var edge1 = builder.AddEdge(100.0, "edge1");
var edge2 = builder.AddEdge(101.0, "edge2");
var edge3 = builder.AddEdge(102.0, "edge3");
var edge4 = builder.AddEdge(103.0, "edge4");

// Points

// W1
var r1 = builder.ConnectEndToStart(edge1, edge3, LeftRight.right);
var r2 = builder.ConnectEndToStart(edge1, edge2, LeftRight.left);
var w1 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r1, r2 }, TurnoutOrientation.Right, "DW3");
w1.RastaId = 230;

// W2
var r3 = builder.ConnectEndToStart(edge2, edge4, LeftRight.right);
var r4 = builder.ConnectEndToStart(edge3, edge4, LeftRight.left);
var w2 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r3, r4 }, TurnoutOrientation.Left, "DW15");
w2.RastaId = 231;

// Axle Counting Heads
var head__100 = builder.AddAxleCountingHead("- / 100", edge1, 0.1);
var head__100__100A = builder.AddAxleCountingHead("100 / 100A", edge1, 0.85);
var head__100A__WW3 = builder.AddAxleCountingHead("100A / WW3", edge1, 0.95);
var head__WW3__101B = builder.AddAxleCountingHead("WW3 / 101B", edge2, 0.05);
var head__101B__101 = builder.AddAxleCountingHead("101B / 101", edge2, 0.15);
var head__WW3__102B = builder.AddAxleCountingHead("WW3 / 102B", edge3, 0.05);
var head__102B__102 = builder.AddAxleCountingHead("102B / 102", edge3, 0.15);
var head__101__101C = builder.AddAxleCountingHead("101 / 101C", edge2, 0.85);
var head__101C__WW15 = builder.AddAxleCountingHead("101C / WW15", edge2, 0.95);
var head__102__102C = builder.AddAxleCountingHead("102 / 102C", edge3, 0.85);
var head__102C__WW15 = builder.AddAxleCountingHead("102C / WW15", edge3, 0.95);
var head__WW15__103D = builder.AddAxleCountingHead("WW15 / 103D", edge4, 0.05);
var head__103D__103 = builder.AddAxleCountingHead("103D / 103", edge4, 0.15);
var head__103 = builder.AddAxleCountingHead("103 / -", edge4, 0.9);

// Axle Counting Sections
var section100 = builder.AddAxleCountingSection<RastaAxleCountingSection>("100")
    .WithAssociatedHead(head__100)
    .WithAssociatedHead(head__100__100A)
    .Build();
section100.RastaId = 190;
var section100A = builder.AddAxleCountingSection<RastaAxleCountingSection>("100A")
    .WithAssociatedHead(head__100A__WW3)
    .WithAssociatedHead(head__100__100A)
    .Build();
section100A.RastaId = 190;
var section101 = builder.AddAxleCountingSection<RastaAxleCountingSection>("101")
    .WithAssociatedHead(head__101__101C)
    .WithAssociatedHead(head__101B__101)
    .Build();
section101.RastaId = 190;
var section101B = builder.AddAxleCountingSection<RastaAxleCountingSection>("101B")
    .WithAssociatedHead(head__101B__101)
    .WithAssociatedHead(head__WW3__101B)
    .Build();
section101B.RastaId = 190;
var section101C = builder.AddAxleCountingSection<RastaAxleCountingSection>("101C")
    .WithAssociatedHead(head__101C__WW15)
    .WithAssociatedHead(head__101__101C)
    .Build();
section101C.RastaId = 190;
var section102 = builder.AddAxleCountingSection<RastaAxleCountingSection>("102")
    .WithAssociatedHead(head__102__102C)
    .WithAssociatedHead(head__102B__102)
    .Build();
section102.RastaId = 190;
var section102B = builder.AddAxleCountingSection<RastaAxleCountingSection>("102B")
    .WithAssociatedHead(head__102B__102)
    .WithAssociatedHead(head__WW3__102B)
    .Build();
section102B.RastaId = 190;
var section102C = builder.AddAxleCountingSection<RastaAxleCountingSection>("102C")
    .WithAssociatedHead(head__102C__WW15)
    .WithAssociatedHead(head__102__102C)
    .Build();
section102C.RastaId = 190;
var section103 = builder.AddAxleCountingSection<RastaAxleCountingSection>("103")
    .WithAssociatedHead(head__103)
    .WithAssociatedHead(head__103D__103)
    .Build();
section103.RastaId = 190;
var section103D = builder.AddAxleCountingSection<RastaAxleCountingSection>("103D")
    .WithAssociatedHead(head__103D__103)
    .WithAssociatedHead(head__WW15__103D)
    .Build();
section103D.RastaId = 190;
var sectionW3 = builder.AddAxleCountingSection<RastaAxleCountingSection>("WW3")
    .WithAssociatedHead(head__WW3__101B)
    .WithAssociatedHead(head__WW3__102B)
    .WithAssociatedHead(head__100A__WW3)
    .Build();
sectionW3.RastaId = 190;
var sectionW15 = builder.AddAxleCountingSection<RastaAxleCountingSection>("WW15")
    .WithAssociatedHead(head__WW15__103D)
    .WithAssociatedHead(head__101C__WW15)
    .WithAssociatedHead(head__102C__WW15)
    .Build();
sectionW15.RastaId = 190;

var sA = builder.AddSignal<RastaSignal>("A", edge1, 0.9, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.entry);
sA.RastaId = 980;
var sB1 = builder.AddSignal<RastaSignal>("B1", edge2, 0.1, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.exit);
sB1.RastaId = 980;
var sB3 = builder.AddSignal<RastaSignal>("B3", edge3, 0.1, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.exit);
sB3.RastaId = 980;
var sC1 = builder.AddSignal<RastaSignal>("C1", edge2, 0.9, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.exit);
sC1.RastaId = 980;
var sC3 = builder.AddSignal<RastaSignal>("C3", edge3, 0.9, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.exit);
sC3.RastaId = 980;
var sD = builder.AddSignal<RastaSignal>("D", edge4, 0.1, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.entry);
sD.RastaId = 980;
var sDTES = builder.AddSignal<RastaSignal>("DTES", edge1, 0, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.trainDestinationOnlySignal);
var sDDRE = builder.AddSignal<RastaSignal>("DDRE", edge4, 1, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.trainDestinationOnlySignal);

var routeBuilder = new RouteBuilder(builder);
var routes = routeBuilder.AddRoutes();
var conflictingRoutes = routeBuilder.AddConflictingRoutes(routes);

// Serialize to file
new EulynxExport(builder.DataPrep).SerializeToFile("Poersten.exml");
