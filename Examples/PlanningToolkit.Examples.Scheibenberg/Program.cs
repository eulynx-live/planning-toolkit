using Models.TopoModels.EULYNX.db;
using Models.TopoModels.EULYNX.rsmCommon;
using Models.TopoModels.EULYNX.rsmTrack;
using PlanningToolkit;
using rsmCommon = Models.TopoModels.EULYNX.rsmCommon;

var builder = new PlanningBuilder();

// Edges

var edge1 = builder.AddEdge(769.928m);
var edge2 = builder.AddEdge(264.983m);
var edge3 = builder.AddEdge(263.885m);
var edge4 = builder.AddEdge(389.040m);
var edge5 = builder.AddEdge(1411.648m);
var edge6 = builder.AddEdge(47.156m);
var edge7 = builder.AddEdge(1024.627m);
var edge8 = builder.AddEdge(3174.399m);

// Points

// W1
var r1 = builder.ConnectEndToStart(edge1, edge5, rsmCommon.LeftRight.right);
var r2 = builder.ConnectEndToStart(edge1, edge4, rsmCommon.LeftRight.left);
var w1 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r1, r2 }, TurnoutOrientation.Left, "W1");

// W2
var r3 = builder.ConnectEndToStart(edge2, edge6, rsmCommon.LeftRight.right);
var r4 = builder.ConnectEndToStart(edge3, edge6, rsmCommon.LeftRight.left);
var w2 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r3, r4 }, TurnoutOrientation.Left, "W2");

// W3
var r5 = builder.ConnectEndToStart(edge6, edge7, rsmCommon.LeftRight.right);
var r6 = builder.ConnectEndToStart(edge4, edge7, rsmCommon.LeftRight.left);
var w3 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r5, r6 }, TurnoutOrientation.Right, "W3");

// W4
var r7 = builder.ConnectEndToStart(edge7, edge8, rsmCommon.LeftRight.right);
var r8 = builder.ConnectEndToStart(edge5, edge8, rsmCommon.LeftRight.left);
var w4 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r7, r8 }, TurnoutOrientation.Right, "W4");


// Axle Counting Sections
var sectionA = builder.AddAxleCountingSection<RastaAxleCountingSection>("A");
var section103 = builder.AddAxleCountingSection<RastaAxleCountingSection>("103");
var section203 = builder.AddAxleCountingSection<RastaAxleCountingSection>("203");
var sectionW1 = builder.AddAxleCountingSection<RastaAxleCountingSection>("W1");
var section403 = builder.AddAxleCountingSection<RastaAxleCountingSection>("403");
var section504 = builder.AddAxleCountingSection<RastaAxleCountingSection>("504");
var section503 = builder.AddAxleCountingSection<RastaAxleCountingSection>("503");
var sectionW3 = builder.AddAxleCountingSection<RastaAxleCountingSection>("W3");
var section604 = builder.AddAxleCountingSection<RastaAxleCountingSection>("604");
var section501 = builder.AddAxleCountingSection<RastaAxleCountingSection>("501");
var section502 = builder.AddAxleCountingSection<RastaAxleCountingSection>("502");
var section603 = builder.AddAxleCountingSection<RastaAxleCountingSection>("603");
var section704 = builder.AddAxleCountingSection<RastaAxleCountingSection>("704");
var section703 = builder.AddAxleCountingSection<RastaAxleCountingSection>("703");
var section101 = builder.AddAxleCountingSection<RastaAxleCountingSection>("101");
var section102 = builder.AddAxleCountingSection<RastaAxleCountingSection>("102");
var sectionW4 = builder.AddAxleCountingSection<RastaAxleCountingSection>("W4");
var section201 = builder.AddAxleCountingSection<RastaAxleCountingSection>("201");

// Axle Counting Heads
builder.AddAxleCountingHead("A/103", edge1, 0.5194, new[] { sectionA, section103 });
builder.AddAxleCountingHead("103/203", edge1, 0.6493, new[] { section103, section203 });
builder.AddAxleCountingHead("203/W1", edge1, 0.9947, new[] { section203, sectionW1 });
builder.AddAxleCountingHead("W1/403", edge4, 0.2498, new[] { sectionW1, section403 });
builder.AddAxleCountingHead("W1/504", edge5, 0.2690, new[] { sectionW1, section504 });
builder.AddAxleCountingHead("403/503", edge4, 0.4770, new[] { section403, section503 });
builder.AddAxleCountingHead("503/W3", edge4, 0.8231, new[] { section503, sectionW3 });
builder.AddAxleCountingHead("504/604", edge5, 0.2262, new[] { section504, section604 });
builder.AddAxleCountingHead("501/W3", edge2, 0.5900, new[] { section501, sectionW3 });
builder.AddAxleCountingHead("502/W3", edge3, 0.5919, new[] { section502, sectionW3 });
builder.AddAxleCountingHead("W3/603", edge7, 0.0324, new[] { sectionW3, section603 });
builder.AddAxleCountingHead("604/704", edge5, 0.2975, new[] { section604, section704 });
builder.AddAxleCountingHead("603/703", edge7, 0.2071, new[] { section603, section703 });
builder.AddAxleCountingHead("703/101", edge7, 0.3145, new[] { section703, section101 });
builder.AddAxleCountingHead("704/102", edge5, 0.5007, new[] { section704, section102 });
builder.AddAxleCountingHead("101/W4", edge7, 0.8903, new[] { section101, sectionW4 });
builder.AddAxleCountingHead("102/W4", edge5, 0.9202, new[] { section102, sectionW4 });
builder.AddAxleCountingHead("W4/201", edge8, 0.0002, new[] { sectionW4, section201 });
builder.AddAxleCountingHead("201/-", edge8, 0.0276, new[] { section201 });

// Signals

var sA = builder.AddSignal<RastaSignal>("A", edge1, 0.8, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.entry);
var sP3 = builder.AddSignal<RastaSignal>("P3", edge4, 0.2, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.exit);
var sP4 = builder.AddSignal<RastaSignal>("P4", edge5, 0.2, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.exit);
var sN1 = builder.AddSignal<RastaSignal>("N1", edge2, 0.2, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.exit);
var sN2 = builder.AddSignal<RastaSignal>("N2", edge3, 0.8, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.exit);
var sN3 = builder.AddSignal<RastaSignal>("N3", edge4, 0.8, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.exit);
var sN4 = builder.AddSignal<RastaSignal>("N4", edge5, 0.8, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.exit);
var sZU13 = builder.AddSignal<RastaSignal>("ZU13", edge7, 0.8, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.intermediate);
var sZU14 = builder.AddSignal<RastaSignal>("ZU14", edge5, 0.8, Side.right, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.intermediate);
var sF = builder.AddSignal<RastaSignal>("F", edge7, 0.2, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.entry);
var sFF = builder.AddSignal<RastaSignal>("FF", edge5, 0.2, Side.right, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.entry);
var sZDS1 = builder.AddSignal<RastaSignal>("ZDS1", edge7, 0.2, Side.left, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.block);
var sZDS2 = builder.AddSignal<RastaSignal>("ZDS2", edge5, 0.2, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.block);
var sZDS3 = builder.AddSignal<RastaSignal>("ZDS3", edge8, 0.2, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.block);

var sDSCL = builder.AddSignal<RastaSignal>("DSCL", edge1, 0, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.trainDestinationOnlySignal);
var sDMAR = builder.AddSignal<RastaSignal>("DMAR", edge8, 1, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.trainDestinationOnlySignal);

// Generate possible routes
var routes = builder.AddRoutes();
var conflictingRoutes = builder.AddConflictingRoutes(routes);

// Rasta Configuration

w1.RastaId = 2345678;
w2.RastaId = 2345679;
w3.RastaId = 2345680;
w4.RastaId = 2345681;

sectionA.RastaId = 0;
section103.RastaId = 0;
section203.RastaId = 0;
sectionW1.RastaId = 0;
section403.RastaId = 0;
section504.RastaId = 0;
section503.RastaId = 0;
sectionW3.RastaId = 0;
section604.RastaId = 0;
section501.RastaId = 0;
section502.RastaId = 0;
section603.RastaId = 0;
section704.RastaId = 0;
section703.RastaId = 0;
section101.RastaId = 0;
section102.RastaId = 0;
sectionW4.RastaId = 0;
section201.RastaId = 0;

sA.RastaId = 0;
sP3.RastaId = 0;
sP4.RastaId = 0;
sN1.RastaId = 0;
sN2.RastaId = 0;
sN3.RastaId = 0;
sN4.RastaId = 0;
sZU13.RastaId = 0;
sZU14.RastaId = 0;
sF.RastaId = 0;
sFF.RastaId = 0;
sZDS1.RastaId = 0;
sZDS2.RastaId = 0;
sZDS3.RastaId = 0;
sDSCL.RastaId = 0;
sDMAR.RastaId = 0;

// Serialize the result
new EulynxExport(builder.DataPrep).SerializeToFile("Scheibenberg.exml");
