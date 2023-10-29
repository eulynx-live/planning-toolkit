using Models.TopoModels.EULYNX.db;
using Models.TopoModels.EULYNX.rsmCommon;
using Models.TopoModels.EULYNX.rsmTrack;
using Models.TopoModels.EULYNX.sig;
using PlanningToolkit;
using PlanningToolkit.PlanningAutomation;

var builder = new PlanningBuilder();

// Edges

var edge1 = builder.AddEdge(769.928, "edge1");
var edge2 = builder.AddEdge(264.983, "edge2");
var edge3 = builder.AddEdge(263.885, "edge3");
var edge4 = builder.AddEdge(389.040, "edge4");
var edge5 = builder.AddEdge(1411.648, "edge5");
var edge6 = builder.AddEdge(47.156, "edge6");
var edge7 = builder.AddEdge(1024.627, "edge7");
var edge8 = builder.AddEdge(3174.399, "edge8");

// Points

// W1
var r1 = builder.ConnectEndToStart(edge1, edge5, LeftRight.right);
var r2 = builder.ConnectEndToStart(edge1, edge4, LeftRight.left);
var w1 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r1, r2 }, TurnoutOrientation.Left, "W1");

// W2
var r3 = builder.ConnectEndToStart(edge2, edge6, LeftRight.right);
var r4 = builder.ConnectEndToStart(edge3, edge6, LeftRight.left);
var w2 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r3, r4 }, TurnoutOrientation.Left, "W2");

// W3
var r5 = builder.ConnectEndToStart(edge6, edge7, LeftRight.right);
var r6 = builder.ConnectEndToStart(edge4, edge7, LeftRight.left);
var w3 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r5, r6 }, TurnoutOrientation.Right, "W3");

// W4
var r7 = builder.ConnectEndToStart(edge7, edge8, LeftRight.right);
var r8 = builder.ConnectEndToStart(edge5, edge8, LeftRight.left);
var w4 = builder.AddPoint<RastaTurnout>(new List<PositionedRelation> { r7, r8 }, TurnoutOrientation.Right, "W4");

// Axle Counting Heads
var head_A__103 = builder.AddAxleCountingHead("A/103", edge1, 0.5194);
var head_103__203 = builder.AddAxleCountingHead("103/203", edge1, 0.6493);
var head_203__W1 = builder.AddAxleCountingHead("203/W1", edge1, 0.9947);
var head_W1__403 = builder.AddAxleCountingHead("W1/403", edge4, 0.2498);
var head_W1__504 = builder.AddAxleCountingHead("W1/504", edge5, 0.2690);
var head_403__503 = builder.AddAxleCountingHead("403/503", edge4, 0.4770);
var head_503__W3 = builder.AddAxleCountingHead("503/W3", edge4, 0.8231);
var head_504__604 = builder.AddAxleCountingHead("504/604", edge5, 0.2262);
var head_501__W3 = builder.AddAxleCountingHead("501/W3", edge2, 0.5900);
var head_502__W3 = builder.AddAxleCountingHead("502/W3", edge3, 0.5919);
var head_W3__603 = builder.AddAxleCountingHead("W3/603", edge7, 0.0324);
var head_604__704 = builder.AddAxleCountingHead("604/704", edge5, 0.2975);
var head_603__703 = builder.AddAxleCountingHead("603/703", edge7, 0.2071);
var head_703__101 = builder.AddAxleCountingHead("703/101", edge7, 0.3145);
var head_704__102 = builder.AddAxleCountingHead("704/102", edge5, 0.5007);
var head_101__W4 = builder.AddAxleCountingHead("101/W4", edge7, 0.8903);
var head_102__W4 = builder.AddAxleCountingHead("102/W4", edge5, 0.9202);
var head_W4__201 = builder.AddAxleCountingHead("W4/201", edge8, 0.0002);
var head_201 = builder.AddAxleCountingHead("201/-", edge8, 0.0276);

// Axle Counting Sections
var sectionA = builder.AddAxleCountingSection<RastaAxleCountingSection>("A")
    .WithAssociatedHead(head_A__103)
    .LimitedByUnconnectedEndOfEdge(edge1)
    .Build();
var section103 = builder.AddAxleCountingSection<RastaAxleCountingSection>("99B103")
    .WithAssociatedHead(head_A__103)
    .WithAssociatedHead(head_103__203)
    .Build();
var section203 = builder.AddAxleCountingSection<RastaAxleCountingSection>("203")
    .WithAssociatedHead(head_103__203)
    .WithAssociatedHead(head_203__W1)
    .Build();
var sectionW1 = builder.AddAxleCountingSection<RastaAxleCountingSection>("W1")
    .WithAssociatedHead(head_203__W1)
    .WithAssociatedHead(head_W1__403)
    .WithAssociatedHead(head_W1__504)
    .Build();
var section403 = builder.AddAxleCountingSection<RastaAxleCountingSection>("403")
    .WithAssociatedHead(head_W1__403)
    .WithAssociatedHead(head_403__503)
    .Build();
var section504 = builder.AddAxleCountingSection<RastaAxleCountingSection>("504")
    .WithAssociatedHead(head_504__604)
    .WithAssociatedHead(head_W1__504)
    .Build();
var section503 = builder.AddAxleCountingSection<RastaAxleCountingSection>("503")
    .WithAssociatedHead(head_503__W3)
    .WithAssociatedHead(head_403__503)
    .Build();
var sectionW3 = builder.AddAxleCountingSection<RastaAxleCountingSection>("W3")
    .WithAssociatedHead(head_W3__603)
    .WithAssociatedHead(head_501__W3)
    .WithAssociatedHead(head_502__W3)
    .WithAssociatedHead(head_503__W3)
    .CoveringEdge(edge6)
    .Build();
var section604 = builder.AddAxleCountingSection<RastaAxleCountingSection>("604")
    .WithAssociatedHead(head_604__704)
    .WithAssociatedHead(head_504__604)
    .Build();
var section501 = builder.AddAxleCountingSection<RastaAxleCountingSection>("501")
    .WithAssociatedHead(head_501__W3)
    .LimitedByUnconnectedEndOfEdge(edge2)
    .Build();
var section502 = builder.AddAxleCountingSection<RastaAxleCountingSection>("502")
    .WithAssociatedHead(head_502__W3)
    .LimitedByUnconnectedEndOfEdge(edge3)
    .Build();
var section603 = builder.AddAxleCountingSection<RastaAxleCountingSection>("603")
    .WithAssociatedHead(head_603__703)
    .WithAssociatedHead(head_W3__603)
    .Build();
var section704 = builder.AddAxleCountingSection<RastaAxleCountingSection>("704")
    .WithAssociatedHead(head_704__102)
    .WithAssociatedHead(head_604__704)
    .Build();
var section703 = builder.AddAxleCountingSection<RastaAxleCountingSection>("703")
    .WithAssociatedHead(head_703__101)
    .WithAssociatedHead(head_603__703)
    .Build();
var section101 = builder.AddAxleCountingSection<RastaAxleCountingSection>("99B101")
    .WithAssociatedHead(head_101__W4)
    .WithAssociatedHead(head_703__101)
    .Build();
var section102 = builder.AddAxleCountingSection<RastaAxleCountingSection>("99B102")
    .WithAssociatedHead(head_102__W4)
    .WithAssociatedHead(head_103__203)
    .WithAssociatedHead(head_704__102)
    .Build();
var sectionW4 = builder.AddAxleCountingSection<RastaAxleCountingSection>("W4")
    .WithAssociatedHead(head_W4__201)
    .WithAssociatedHead(head_101__W4)
    .WithAssociatedHead(head_102__W4)
    .Build();
var section201 = builder.AddAxleCountingSection<RastaAxleCountingSection>("201")
    .WithAssociatedHead(head_201)
    .WithAssociatedHead(head_W4__201)
    .Build();

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

var sDSCL = builder.AddSignal<Signal>("DSCL", edge1, 0, Side.left, ApplicationDirection.reverse, SignalTypeTypes.main, SignalFunctionTypes.trainDestinationOnlySignal);
var sDMAR = builder.AddSignal<Signal>("DMAR", edge8, 1, Side.right, ApplicationDirection.normal, SignalTypeTypes.main, SignalFunctionTypes.trainDestinationOnlySignal);

// Generate possible routes
var routeBuilder = new RouteBuilder(builder);
var _ = routeBuilder.AddRoutes();
routeBuilder.AddConflictingRoutes();

// Rasta Configuration

w1.RastaId = 2345678;
w2.RastaId = 2345679;
w3.RastaId = 2345680;
w4.RastaId = 2345681;

sectionA.RastaId = 0;
section103.RastaId = 256;
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
section101.RastaId = 256;
section102.RastaId = 256;
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

// Serialize the result
new EulynxExport(builder.DataPrep).SerializeToFile("Scheibenberg.exml");
