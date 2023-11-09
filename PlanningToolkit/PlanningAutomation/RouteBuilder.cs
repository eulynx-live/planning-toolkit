using Models.TopoModels.EULYNX.generic;
using Models.TopoModels.EULYNX.rsmCommon;
using Models.TopoModels.EULYNX.rsmTrack;
using Models.TopoModels.EULYNX.sig;
using MainRoute = Models.TopoModels.EULYNX.db.MainRoute;

namespace PlanningToolkit.PlanningAutomation;

public class RouteBuilder
{
    private readonly PlanningBuilder _builder;

    public RouteBuilder(PlanningBuilder builder)
    {
        _builder = builder;
    }

    public class RouteData
    {
        public Signal EntrySignal { get; }
        public Signal? ExitSignal { get; set; }
        public List<(Turnout, Models.TopoModels.EULYNX.rsmCommon.LeftRight)> Points { get; init; } = new();
        public List<TvpSection> TvpSections { get; init; } = new List<TvpSection>();
        public List<(string Edge, double Offset)> Area { get; set; } = new();

        public RouteData(Signal entrySignal)
        {
            EntrySignal = entrySignal;
        }
    }

    /// <summary>
    /// Find all routes starting with the given startSignal and add them to container.
    /// </summary>
    /// <param name="startSignal"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private List<RouteData> AddRoutesForStartSignal<T>(Signal startSignal) where T : Route, new()
    {
        var routeData = new RouteData(startSignal);
        var startSignalOffset = _builder.GetSignalCoordinate(startSignal).value!.Value;
        var edge = _builder.GetEdgeForSignal(startSignal)!;
        routeData.Area.Add((edge.id!, startSignalOffset));
        var applicationDirection = _builder.GetSignalDirection(startSignal);
        if (applicationDirection != null && edge != null)
        {
            var mainSignals = _builder.GetMainSignalsWithDirection(edge, applicationDirection.Value);

            // Check for signals on the starting edge
            var signalsAfterStartSignal = mainSignals
                .Where(x => (applicationDirection == ApplicationDirection.normal) ?
                    _builder.GetSignalCoordinate(x)?.value > startSignalOffset :
                    _builder.GetSignalCoordinate(x)?.value < startSignalOffset)
                .ToList();

            if (signalsAfterStartSignal.Count > 0)
            {
                var orderedSignals = _builder.OrderedSignals(signalsAfterStartSignal, applicationDirection.Value);
                var endSignal = orderedSignals.First();
                var endSignalOffset = _builder.GetSignalCoordinate(endSignal).value!.Value;
                routeData.ExitSignal = endSignal;
                routeData.Area.Add((edge.id!, endSignalOffset));
                routeData.TvpSections.AddRange(_builder.GetOverlappingTvpSections(routeData.Area));
                return new List<RouteData> { AddRoute<T>(routeData) };
            }
            else
            {
                routeData.Area.Add((edge.id!, startSignalOffset));
                return AddRouteForNextEdges<T>(edge, applicationDirection.Value, routeData);
            }
        }
        else
        {
            return new List<RouteData>();
        }
    }


    /// <summary>
    /// Finds edges/ poisitioned relations in direction and starts a new route finding for each.
    /// </summary>
    /// <param name="edge"></param>
    /// <param name="direction"></param>
    /// <param name="routeData"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    List<RouteData> AddRouteForNextEdges<T>(LinearElementWithLength edge, ApplicationDirection direction, RouteData routeData)
        where T : Route, new()
    {
        var nextPositionedRelations = _builder.GetPositionedRelationsInDirection(edge, direction);
        var routes = new List<RouteData>();
        foreach (var relation in nextPositionedRelations)
        {
            var nextEdge = _builder.GetLinearElementWithLength(relation.elementA!.@ref) == edge ?
                _builder.GetLinearElementWithLength(relation.elementB!.@ref) :
                _builder.GetLinearElementWithLength(relation.elementA!.@ref);
            var nextPoint = _builder.GetNextPoint(edge, direction);
            var routeDataCopy = new RouteData(routeData.EntrySignal)
            {
                Points = new(routeData.Points),
                Area = new(routeData.Area)
            };
            if (nextPoint != null)
            {
                routeDataCopy.Points.Add((nextPoint, relation.leadsTowards!.Value));
            }
            if (nextEdge != null)
            {
                routes.AddRange(AddRouteForNextEdge<T>(nextEdge, direction, routeDataCopy));
            }
        }
        return routes;
    }

    /// <summary>
    /// Searches for main Signals with the same direction and then adds a new Route.
    /// If none are found, it recursively searches on connected edges in that direction.
    /// </summary>
    /// <param name="edge"></param>
    /// <param name="direction"></param>
    /// <param name="routeData"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    List<RouteData> AddRouteForNextEdge<T>(LinearElementWithLength edge, ApplicationDirection direction, RouteData routeData)
        where T : Route, new()
    {
        routeData.Area.Add((edge.id!, 0));
        var mainSignals = _builder.GetMainSignalsWithDirection(edge, direction);
        if (mainSignals.Count > 0)
        {
            var orderedSignals = _builder.OrderedSignals(mainSignals, direction);
            var nextSignal = orderedSignals.First();
            var nextSignalOffset = _builder.GetSignalCoordinate(nextSignal).value!.Value;
            routeData.Area.Add((edge.id!, nextSignalOffset));

            routeData.TvpSections.AddRange(_builder.GetOverlappingTvpSections(routeData.Area));
            routeData.ExitSignal = nextSignal;
            return new List<RouteData> { AddRoute<T>(routeData) };
        }
        else
        {
            routeData.Area.Add((edge.id!, 1));
            routeData.TvpSections.AddRange(_builder.GetOverlappingTvpSections(routeData.Area));
            return AddRouteForNextEdges<T>(edge, direction, routeData);
        }
    }

    /// <summary>
    /// Find all routes for each main Signal of the station and add them to container.
    /// </summary>
    public List<RouteData> AddRoutes()
    {
        var signals = _builder.DataPrepEntities.ownsTrackAsset
            .GetSignals()
            .Where(x => _builder.IsMainSignal(x));
        return signals.SelectMany(x => AddRoutesForStartSignal<MainRoute>(x)).ToList();
    }


    /// <summary>
    /// Add a new route to the station, complete with route body, point positions and clearing sections.
    /// </summary>
    /// <param name="routeData"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public RouteData AddRoute<T>(RouteData routeData) where T : Route, new()
    {
        var route = new T();

        var routeBody = _builder.AddRouteBody(
            string.Format("{0}.{1}",
                _builder.GetRsmSignal(routeData.EntrySignal.refersToRsmSignal?.@ref ?? "")?.name ?? "",
                _builder.GetRsmSignal(routeData.ExitSignal?.refersToRsmSignal?.@ref ?? "")?.name ?? ""),
            new tElementWithIDref(routeData.EntrySignal.id ?? ""),
            new tElementWithIDref(routeData.ExitSignal?.id ?? "")
        );
        route.appliesToRouteBody = new tElementWithIDref(routeBody.id!);
        route.id = IdManager.computeUuid5<Route>($"{routeData.EntrySignal.id}.{routeData.ExitSignal?.id}");

        // Connect routebody w/ point states
        foreach (var point in routeData.Points)
        {
            var peap = _builder.AddPointElementAndPosition(routeBody, point.Item1, point.Item2);
        }


        // Remove any section that overlaps with the start signal
        // (and would thus prevent the route from being set)
        var (EdgeId, Offset) = _builder.GetSignalPosition(routeData.EntrySignal);
        var overlappingSections = _builder.GetOverlappingTvpSectionWithPosition(EdgeId, Offset);
        foreach (var overlappingSection in overlappingSections) {
            routeData.TvpSections.Remove(overlappingSection);
        }

        // Connect route with sections that need to be clear for this route to be set
        var csc = new ConditionSectionsClear
        {
            affectsRoute = new tElementWithIDref(route.id ?? ""),
            provesSection = routeData.TvpSections.Select(s => new tElementWithIDref(s.id ?? "")).ToList(),
            id = IdManager.computeUuid5<ConditionSectionsClear>($"{routeData.EntrySignal.id}.{routeData.ExitSignal?.id}")
        };
        _builder.DataPrep.hasDataContainer[0].ownsDataPrepEntities?.ownsConditionSectionsClear.Add(csc);

        var listOfSections = routeData.TvpSections.Select(s => new tElementWithIDref(s.id ?? "")).ToList();
        var sectionList = _builder.AddSectionList(routeBody, listOfSections);

        _builder.AddRoute(route);
        return routeData;
    }


    /// <summary>
    /// Finds all conflicting routes.
    /// </summary>
    /// <param name="routes"></param>
    /// <returns></returns>
    public void AddConflictingRoutes()
    {
        var routes = _builder.DataPrep.Get<MainRoute>().ToList();

        foreach (var route in routes)
        {
            var sections = _builder.DataPrep.Get<ConditionSectionsClear>()
                .Where(x => x.affectsRoute?.@ref == route.id)
                .SelectMany(x => x.provesSection)
                .Select(x => x.@ref)
                .ToList();

            var conflictingRoutes = new List<MainRoute>();

            foreach (var otherRoute in routes.Where(x => x != route))
            {
                // Check if the two routes share any tvpSection and add a conflict if true

                var otherSections = _builder.DataPrep.Get<ConditionSectionsClear>()
                    .Where(x => x.affectsRoute?.@ref == otherRoute.id)
                    .SelectMany(x => x.provesSection)
                    .Select(x => x.@ref);

                if (sections.Intersect(otherSections).Any())
                {
                    conflictingRoutes.Add(otherRoute);
                }
            }

            _builder.AddConflictingRoutes(route, conflictingRoutes);
        }
    }
}
