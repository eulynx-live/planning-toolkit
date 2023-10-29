using Models.TopoModels.EULYNX.db;
using Models.TopoModels.EULYNX.rsmCommon;
using Models.TopoModels.EULYNX.rsmTrack;
using Models.TopoModels.EULYNX.sig;
using PlanningToolkit;
using PlanningToolkit.PlanningAutomation;
using rsmCommon = Models.TopoModels.EULYNX.rsmCommon;

var builder = new PlanningBuilder();

// EDGES
var edge1 = builder.AddEdge(100.0, "edge1");
var edge2 = builder.AddEdge(101.0, "edge2");
var edge3 = builder.AddEdge(102.0, "edge3");
var edge7 = builder.AddEdge(106.0, "edge7");

// POINTS

// W1
var r1 = builder.ConnectEndToStart(edge1, edge2, rsmCommon.LeftRight.right);
var r2 = builder.ConnectEndToStart(edge1, edge3, rsmCommon.LeftRight.left);
var w1 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation>{r1, r2}, TurnoutOrientation.Left, "W1");

// W4
var r8 = builder.ConnectEndToStart(edge2, edge7, rsmCommon.LeftRight.left);
var r7 = builder.ConnectEndToStart(edge3, edge7, rsmCommon.LeftRight.right);
var w2 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r7,r8 }, TurnoutOrientation.Right, "W2");

// AXLECOUNTINGHEADS
var head__100 = builder.AddAxleCountingHead("- / 100", edge1, 0);
var head__100__WW1 = builder.AddAxleCountingHead("100 / WW1", edge1, 0.9);
var head__WW1__101 = builder.AddAxleCountingHead("WW1 / 101", edge2, 0.1);
var head__101__WW2 = builder.AddAxleCountingHead("101 / WW2", edge2, 0.9);
var head__WW1__102 = builder.AddAxleCountingHead("WW1 / 102", edge3, 0.1);
var head__102__WW2 = builder.AddAxleCountingHead("102 / WW2", edge3, 0.9);
var head__WW2__108 = builder.AddAxleCountingHead("WW2 / 108", edge7, 0.1);
var head__108 = builder.AddAxleCountingHead("108 / -", edge7, 0.9);

// AXLECOUNTINGSECTIONS
var section100 = builder.AddAxleCountingSection<RastaAxleCountingSection>("100")
    .WithAssociatedHead(head__100)
    .WithAssociatedHead(head__100__WW1)
    .Build();
var section101 = builder.AddAxleCountingSection<RastaAxleCountingSection>("101")
    .WithAssociatedHead(head__WW1__101)
    .WithAssociatedHead(head__101__WW2)
    .Build();
var section102 = builder.AddAxleCountingSection<RastaAxleCountingSection>("102")
    .WithAssociatedHead(head__WW1__102)
    .WithAssociatedHead(head__102__WW2)
    .Build();
var sectionW1 = builder.AddAxleCountingSection<RastaAxleCountingSection>("WW1")
    .WithAssociatedHead(head__WW1__101)
    .WithAssociatedHead(head__WW1__102)
    .WithAssociatedHead(head__100__WW1)
    .Build();
var sectionW2 = builder.AddAxleCountingSection<RastaAxleCountingSection>("WW2")
    .WithAssociatedHead(head__WW2__108)
    .WithAssociatedHead(head__101__WW2)
    .WithAssociatedHead(head__102__WW2)
    .Build();
var section108 = builder.AddAxleCountingSection<RastaAxleCountingSection>("108")
    .WithAssociatedHead(head__WW2__108)
    .WithAssociatedHead(head__108)
    .Build();

var sA = builder.AddSignal<RastaSignal>("A", edge1, 0.8 ,  Side.right, ApplicationDirection.normal,  SignalTypeTypes.main, SignalFunctionTypes.entry);
var sP3 = builder.AddSignal<RastaSignal>("B1", edge2, 0.2 ,  Side.right, ApplicationDirection.reverse,  SignalTypeTypes.main, SignalFunctionTypes.exit);
var sN3 = builder.AddSignal<RastaSignal>("C1", edge2, 0.8 ,  Side.right, ApplicationDirection.normal,  SignalTypeTypes.main, SignalFunctionTypes.exit);
var sF = builder.AddSignal<RastaSignal>("D", edge7, 0.2,   Side.right, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.entry);
var sDSCL = builder.AddSignal<RastaSignal>("DSCL", edge1, 0, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.trainDestinationOnlySignal);
var sDMAR = builder.AddSignal<RastaSignal>("DMAR", edge7, 1, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.trainDestinationOnlySignal);

// Generate possible routes
var routeBuilder = new RouteBuilder(builder);
var _ = routeBuilder.AddRoutes();
routeBuilder.AddConflictingRoutes();

// RaSTA Configuration

w1.RastaId = 0x60;
w2.RastaId = 0x60;
sA.RastaId = 0x60;
sP3.RastaId = 0x60;
sN3.RastaId = 0x60;
sF.RastaId = 0x60;
section100.RastaId = 0x60;
section101.RastaId = 0x60;
section102.RastaId = 0x60;
sectionW1.RastaId = 0x60;
sectionW2.RastaId = 0x60;
section108.RastaId = 0x60;

// Serialize the result
new EulynxExport(builder.DataPrep).SerializeToFile("Simple.exml");
