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

    public record RouteData(Signal entrySignal)
    {
        public Signal EntrySignal { get; } = entrySignal;
        public Signal? ExitSignal { get; set; }
        public List<Tuple<Turnout, LeftRight>> Points { get; init; } = new List<Tuple<Turnout, LeftRight>>();
        public List<TvpSection> TvpSections { get; init; } = new List<TvpSection>();
    }

    /// <summary>
    /// Find all routes starting with the given startSignal and add them to container.
    /// </summary>
    /// <param name="startSignal"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private List<RouteData> AddRoutesForStartSignal<T>(Signal startSignal) where T : Route, new()
    {
        var edge = _builder.GetEdgeForSignal(startSignal);
        var applicationDirection = _builder.GetSignalDirection(startSignal);
        if (applicationDirection != null && edge != null)
        {
            var mainSignals = _builder.GetMainSignalsWithDirection(edge, applicationDirection.Value);

            // Check for signals on the starting edge
            var signalsAfterStartSignal = mainSignals?
                .Where(x => (applicationDirection == ApplicationDirection.normal) ?
                    _builder.GetSignalCoordinate(x)?.value > _builder.GetSignalCoordinate(startSignal)?.value :
                    _builder.GetSignalCoordinate(x)?.value < _builder.GetSignalCoordinate(startSignal)?.value)
                .ToList();

            if (signalsAfterStartSignal?.Count() > 0)
            {
                var orderedSignals = _builder.OrderedSignals(signalsAfterStartSignal, applicationDirection.Value);
                var endSignal = orderedSignals.First();
                var tvpSections = _builder.GetOverlappingTvpSections(edge, applicationDirection.Value, startSignal, endSignal);
                var routeData = new RouteData(startSignal) { ExitSignal = endSignal, TvpSections = tvpSections };
                return new List<RouteData> { AddRoute<T>(routeData) };
            }
            else
            {
                var tvpSections = _builder.GetOverlappingTvpSections(edge, applicationDirection.Value, startSignal, null);
                return AddRouteForNextEdges<T>(edge, applicationDirection.Value,
                    new RouteData(startSignal) { TvpSections = tvpSections }
                );
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
        if (nextPositionedRelations != null)
        {
            foreach (var relation in nextPositionedRelations)
            {
                var nextEdge = _builder.GetLinearElementWithLength(relation.elementA?.@ref ?? "") == edge ?
                    _builder.GetLinearElementWithLength(relation.elementB?.@ref ?? "") :
                    _builder.GetLinearElementWithLength(relation.elementA?.@ref ?? "");
                var nextPoint = _builder.GetNextPoint(edge, direction);
                var routeDataCopy = new RouteData(routeData.EntrySignal)
                {
                    Points = new List<Tuple<Turnout, LeftRight>>(routeData.Points),
                    TvpSections = new List<TvpSection>(routeData.TvpSections)
                };
                if (nextPoint != null)
                {
                    var pointDirection = relation.leadsTowards == LeftRight.left ? LeftRight.left : LeftRight.right;
                    routeDataCopy.Points.Add(new Tuple<Turnout, LeftRight>(nextPoint, pointDirection));
                }
                if (nextEdge != null)
                {
                    routes.AddRange(AddRouteForNextEdge<T>(nextEdge, direction, routeDataCopy));
                }
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
        var signals = _builder.GetSignalsForEdge(edge);
        var mainSignals = _builder.GetMainSignalsWithDirection(edge, direction);
        var lastSectionId = routeData.TvpSections.Last().id;
        if (mainSignals?.Count() > 0)
        {
            var orderedSignals = _builder.OrderedSignals(mainSignals, direction);
            var endSignal = orderedSignals.First();
            var tvpSection = _builder.GetOverlappingTvpSections(edge, direction, null, endSignal);
            if (tvpSection != null)
            {
                routeData.TvpSections.AddRange(tvpSection);
            }
            routeData.ExitSignal = endSignal;
            return new List<RouteData> { AddRoute<T>(routeData) };
        }
        else
        {
            routeData.TvpSections.AddRange(_builder.GetOverlappingTvpSections(edge, direction, null, null));
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
            String.Format("{0}.{1}",
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

        // Connect route with sections that need to be clear for this route to be set
        var csc = new ConditionSectionsClear()
        {
            affectsRoute = new tElementWithIDref(route.id ?? ""),
            provesSection = routeData.TvpSections.Select(s => new tElementWithIDref(s.id ?? "")).ToList()
        };
        csc.id = IdManager.computeUuid5<ConditionSectionsClear>($"{routeData.EntrySignal.id}.{routeData.ExitSignal?.id}");
        _builder.DataPrep.hasDataContainer[0].ownsDataPrepEntities?.ownsConditionSectionsClear.Add(csc);

        var conflictingRoute = _builder.AddConflictingRoute(csc.affectsRoute, csc.provesSection);

        List<tElementWithIDref> listOfSections = routeData.TvpSections.Select(s => new tElementWithIDref(s.id ?? "")).ToList();
        var sectionList = _builder.AddSectionList(routeBody, listOfSections);

        _builder.AddRoute(route);
        return routeData;
    }


    /// <summary>
    /// Finds all conflicting routes.
    /// </summary>
    /// <param name="routes"></param>
    /// <returns></returns>
    public Dictionary<String, List<RouteData>> AddConflictingRoutes(List<RouteData> routes)
    {
        var conflictingRoutesMap = new Dictionary<String, List<RouteData>>();
        for (int i = 0; i < routes.Count(); i++)
        {
            // WARNING: We assume that there is only ever one route from entrySignal to exitSignal!
            var routeId = $"{routes[i].EntrySignal.id}.{routes[i].ExitSignal?.id}";
            var conflictingRoutes = new List<RouteData>();
            for (int j = 0; j < routes.Count(); j++)
            {
                // Check if two routes share any tvpSection and add conflict if true
                if (i != j && routes[i].TvpSections.Intersect(routes[j].TvpSections).Count() > 0)
                {
                    conflictingRoutes.Add(routes[j]);
                }
            }
            conflictingRoutesMap.Add(routeId, conflictingRoutes);
        }
        return conflictingRoutesMap;
    }

}
