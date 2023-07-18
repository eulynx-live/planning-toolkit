using Models.TopoModels.EULYNX.db;
using Models.TopoModels.EULYNX.rsmCommon;
using Models.TopoModels.EULYNX.rsmTrack;
using PlanningToolkit;
using PlanningToolkit.PlanningAutomation;
using rsmCommon = Models.TopoModels.EULYNX.rsmCommon;


var builder = new PlanningBuilder();

// Edges
var edge1 = builder.AddEdge((decimal)100);
var edge2 = builder.AddEdge((decimal)101);
var edge3 = builder.AddEdge((decimal)102);
var edge4 = builder.AddEdge((decimal)103);

// Points

// W1
var r1 = builder.ConnectEndToStart(edge1, edge3, rsmCommon.LeftRight.right);
var r2 = builder.ConnectEndToStart(edge1, edge2, rsmCommon.LeftRight.left);
var w1 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r1, r2 }, TurnoutOrientation.Right, "DW3");
w1.RastaId = 230;

// W2
var r3 = builder.ConnectEndToStart(edge2, edge4, rsmCommon.LeftRight.right);
var r4 = builder.ConnectEndToStart(edge3, edge4, rsmCommon.LeftRight.left);
var w2 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r3, r4 }, TurnoutOrientation.Left, "DW15");
w2.RastaId = 231;

// Axle Counting Sections
var section100 = builder.AddAxleCountingSection<RastaAxleCountingSection>("100");
section100.RastaId = 190;
var section100A = builder.AddAxleCountingSection<RastaAxleCountingSection>("100A");
section100A.RastaId = 190;
var section101 = builder.AddAxleCountingSection<RastaAxleCountingSection>("101");
section101.RastaId = 190;
var section101B = builder.AddAxleCountingSection<RastaAxleCountingSection>("101B");
section101.RastaId = 190;
var section101C = builder.AddAxleCountingSection<RastaAxleCountingSection>("101C");
section101C.RastaId = 190;
var section102 = builder.AddAxleCountingSection<RastaAxleCountingSection>("102");
section102.RastaId = 190;
var section102B = builder.AddAxleCountingSection<RastaAxleCountingSection>("102B");
section102B.RastaId = 190;
var section102C = builder.AddAxleCountingSection<RastaAxleCountingSection>("102C");
section102C.RastaId = 190;
var section103 = builder.AddAxleCountingSection<RastaAxleCountingSection>("103");
section103.RastaId = 190;
var section103D = builder.AddAxleCountingSection<RastaAxleCountingSection>("103D");
section103D.RastaId = 190;
var sectionW3 = builder.AddAxleCountingSection<RastaAxleCountingSection>("WW3");
sectionW3.RastaId = 190;
var sectionW15 = builder.AddAxleCountingSection<RastaAxleCountingSection>("WW15");
sectionW15.RastaId = 190;

// Axle Counting Heads
builder.AddAxleCountingHead("- / 100", edge1, 0.1, new [] { section100 });
builder.AddAxleCountingHead("100 / 100A", edge1, 0.85, new [] { section100, section100A });
builder.AddAxleCountingHead("100A / WW3", edge1, 0.95, new [] { section100A, sectionW3 });
builder.AddAxleCountingHead("WW3 / 101B", edge2, 0.05, new [] { sectionW3, section101B });
builder.AddAxleCountingHead("101B / 101", edge2, 0.15, new [] { section101B, section101 });
builder.AddAxleCountingHead("WW3 / 102B", edge3, 0.05, new [] { sectionW3, section102B });
builder.AddAxleCountingHead("102B / 102", edge3, 0.15, new [] { section102B, section102 });
builder.AddAxleCountingHead("101 / 101C", edge2, 0.85, new [] { section101, section101C });
builder.AddAxleCountingHead("101C / WW15", edge2, 0.95, new [] { section101C, sectionW15 });
builder.AddAxleCountingHead("102 / 102C", edge3, 0.85, new [] { section102, section102C });
builder.AddAxleCountingHead("102C / WW15", edge3, 0.95, new [] { section102C, sectionW15 });
builder.AddAxleCountingHead("WW15 / 103D", edge4, 0.05, new [] { sectionW15, section103D });
builder.AddAxleCountingHead("103D / 103", edge4, 0.15, new [] { section103D, section103 });
builder.AddAxleCountingHead("103 / -", edge4, 0.9, new [] { section103 });

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
