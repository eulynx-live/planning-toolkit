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

// DW3
var r1 = builder.ConnectEndToStart(edge1, edge3, LeftRight.right);
var r2 = builder.ConnectEndToStart(edge1, edge2, LeftRight.left);
var dw3 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r1, r2 }, TurnoutOrientation.Right, "DW3");

// DW15
var r3 = builder.ConnectEndToStart(edge2, edge4, LeftRight.right);
var r4 = builder.ConnectEndToStart(edge3, edge4, LeftRight.left);
var dw15 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r3, r4 }, TurnoutOrientation.Left, "DW15");

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
var section100A = builder.AddAxleCountingSection<RastaAxleCountingSection>("100A")
    .WithAssociatedHead(head__100A__WW3)
    .WithAssociatedHead(head__100__100A)
    .Build();
var section101 = builder.AddAxleCountingSection<RastaAxleCountingSection>("101")
    .WithAssociatedHead(head__101__101C)
    .WithAssociatedHead(head__101B__101)
    .Build();
var section101B = builder.AddAxleCountingSection<RastaAxleCountingSection>("101B")
    .WithAssociatedHead(head__101B__101)
    .WithAssociatedHead(head__WW3__101B)
    .Build();
var section101C = builder.AddAxleCountingSection<RastaAxleCountingSection>("101C")
    .WithAssociatedHead(head__101C__WW15)
    .WithAssociatedHead(head__101__101C)
    .Build();
var section102 = builder.AddAxleCountingSection<RastaAxleCountingSection>("102")
    .WithAssociatedHead(head__102__102C)
    .WithAssociatedHead(head__102B__102)
    .Build();
var section102B = builder.AddAxleCountingSection<RastaAxleCountingSection>("102B")
    .WithAssociatedHead(head__102B__102)
    .WithAssociatedHead(head__WW3__102B)
    .Build();
var section102C = builder.AddAxleCountingSection<RastaAxleCountingSection>("102C")
    .WithAssociatedHead(head__102C__WW15)
    .WithAssociatedHead(head__102__102C)
    .Build();
var section103 = builder.AddAxleCountingSection<RastaAxleCountingSection>("103")
    .WithAssociatedHead(head__103)
    .WithAssociatedHead(head__103D__103)
    .Build();
var section103D = builder.AddAxleCountingSection<RastaAxleCountingSection>("103D")
    .WithAssociatedHead(head__103D__103)
    .WithAssociatedHead(head__WW15__103D)
    .Build();
var sectionW3 = builder.AddAxleCountingSection<RastaAxleCountingSection>("WW3")
    .WithAssociatedHead(head__WW3__101B)
    .WithAssociatedHead(head__WW3__102B)
    .WithAssociatedHead(head__100A__WW3)
    .Build();
var sectionW15 = builder.AddAxleCountingSection<RastaAxleCountingSection>("WW15")
    .WithAssociatedHead(head__WW15__103D)
    .WithAssociatedHead(head__101C__WW15)
    .WithAssociatedHead(head__102C__WW15)
    .Build();

var sA = builder.AddSignal<RastaSignal>("A", edge1, 0.9, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.entry);
var sB1 = builder.AddSignal<RastaSignal>("B1", edge2, 0.1, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.exit);
var sB3 = builder.AddSignal<RastaSignal>("B3", edge3, 0.1, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.exit);
var sC1 = builder.AddSignal<RastaSignal>("C1", edge2, 0.9, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.exit);
var sC3 = builder.AddSignal<RastaSignal>("C3", edge3, 0.9, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.exit);
var sD = builder.AddSignal<RastaSignal>("D", edge4, 0.1, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.entry);
var sDTES = builder.AddSignal<RastaSignal>("DTES", edge1, 0, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.trainDestinationOnlySignal);
var sDDRE = builder.AddSignal<RastaSignal>("DDRE", edge4, 1, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.trainDestinationOnlySignal);

dw3.RastaId = 230;
dw15.RastaId = 231;
section100.RastaId = 190;
section100A.RastaId = 190;
section101.RastaId = 190;
section101B.RastaId = 190;
section101C.RastaId = 190;
section102.RastaId = 190;
section102B.RastaId = 190;
section102C.RastaId = 190;
section103.RastaId = 190;
section103D.RastaId = 190;
sectionW3.RastaId = 190;
sectionW15.RastaId = 190;
sA.RastaId = 980;
sB1.RastaId = 980;
sB3.RastaId = 980;
sC1.RastaId = 980;
sC3.RastaId = 980;
sD.RastaId = 980;

var routeBuilder = new RouteBuilder(builder);
var _ = routeBuilder.AddRoutes();
routeBuilder.AddConflictingRoutes();

// Serialize to file
new EulynxExport(builder.DataPrep).SerializeToFile("Poersten.exml");
