using Models.TopoModels.EULYNX.db;
using Models.TopoModels.EULYNX.generic;
using Models.TopoModels.EULYNX.rsmCommon;
using Models.TopoModels.EULYNX.rsmSig;
using Models.TopoModels.EULYNX.sig;

using RsmSignal = Models.TopoModels.EULYNX.rsmSig.Signal;
using SignalFunction = Models.TopoModels.EULYNX.db.SignalFunction;
using SignalType = Models.TopoModels.EULYNX.db.SignalType;
using SignalTypeTypes = Models.TopoModels.EULYNX.db.SignalTypeTypes;
using Signal = Models.TopoModels.EULYNX.sig.Signal;
using LeftRight = Models.TopoModels.EULYNX.rsmCommon.LeftRight;
using Models.TopoModels.EULYNX.rsmTrack;
using VehiclePassageDetector = Models.TopoModels.EULYNX.rsmSig.VehiclePassageDetector;
using RsmRouteBody = Models.TopoModels.EULYNX.rsmSig.RouteBody;
using RouteBody = Models.TopoModels.EULYNX.sig.RouteBody;
using PlanningToolkit.Builder;

namespace PlanningToolkit
{
    public class PlanningBuilder
    {
        public EulynxDataPrepInterface DataPrep { get; }

        public Models.TopoModels.EULYNX.generic.DataPrepEntities DataPrepEntities => DataPrep.hasDataContainer.Single().ownsDataPrepEntities!;
        public RsmEntities RsmEntities => DataPrep.hasDataContainer.Single().ownsRsmEntities!;

        private HashSet<string> pointElementsAndPositions = new HashSet<string>();

        /// <summary>
        /// The train station with all its assets
        /// </summary>
        public PlanningBuilder()
        {
            DataPrep = new();

            var container = new DataContainer
            {
                ownsDataPrepEntities = new(),
                ownsRsmEntities = new()
                {
                    usesTrackTopology = new(),
                    usesTopography = new()
                }
            };

            DataPrep.hasDataContainer.Add(container);
        }

        private PositionedRelation? GetPositionedRelation(String relationId) =>
            RsmEntities.usesTrackTopology?.usesPositionedRelation.Where(x => x.id == relationId).Single();
        public TrackAsset? GetTrackAsset(String assetId) =>
            DataPrepEntities.ownsTrackAsset?.Where(x => x.id == assetId).Single();
        public LinearElementWithLength? GetLinearElementWithLength(String elementId) =>
            RsmEntities.usesTrackTopology?.usesNetElement.OfType<LinearElementWithLength>().Where(l => l.id == elementId).Single();
        private IntrinsicCoordinate? GetCoordinates(String boundsId) =>
            RsmEntities.usesTopography?.usesIntrinsicCoordinate.Where(x => x.id == boundsId).Single();
        private SpotLocation GetSpotLocation(String location) =>
            RsmEntities.usesLocation.OfType<SpotLocation>().Where(l => l.id == location).Single();
        private LinearLocation? GetLinearLocation(String location) =>
            RsmEntities.usesLocation.Where(l => l.id == location).Single() as LinearLocation;
        public RsmSignal? GetRsmSignal(String signalId) =>
            RsmEntities.ownsSignal.Where(s => s.id == signalId).Single();
        public void AddTrackAsset(TrackAsset asset) =>
            DataPrepEntities.ownsTrackAsset?.Add(asset);
        public void AddCoordinates(IntrinsicCoordinate bounds) {
            if (RsmEntities.usesTopography?.usesIntrinsicCoordinate.Any(x => x.id == bounds.id) ?? false)
                //  throw new Exception("duplicate key");
                return;
            RsmEntities.usesTopography?.usesIntrinsicCoordinate.Add(bounds);
        }
        public void AddSpotLocation(BaseLocation location) =>
            RsmEntities.usesLocation.Add(location);
        public void AddLocation(BaseLocation location) =>
            RsmEntities.usesLocation.Add(location);
        public void AddTrackSignallingDevice(OnTrackSignallingDevice device) =>
            RsmEntities.ownsOnTrackSignallingDevice.Add(device);
        public void AddRsmSignal(RsmSignal signal) =>
            RsmEntities.ownsSignal.Add(signal);
        public void AddLinearElementWithLength(LinearElementWithLength element) =>
            RsmEntities.usesTrackTopology?.usesNetElement.Add(element);
        public void AddPositionedRelation(PositionedRelation relation) =>
            RsmEntities.usesTrackTopology?.usesPositionedRelation?.Add(relation);
        public void AddPoint<T>(T point) where T : Turnout =>
            RsmEntities.ownsPoint.Add(point);
        public void AddRsmRouteBody(RsmRouteBody rsmRouteBody) =>
            RsmEntities.ownsRouteBody?.Add(rsmRouteBody);
        public void AddRouteBody(RouteBody routeBody) =>
            DataPrepEntities.ownsRouteBody.Add(routeBody);
        public void AddRoute<T>(T route) where T : Route =>
            DataPrepEntities.ownsRoute.Add(route);
        public void AddSectionList(SectionList sectionList) =>
            DataPrepEntities.ownsRouteBodyProperty.Add(sectionList);
        public void AddConflictingRoute(ConflictingRoute conflictingRoute) =>
            DataPrepEntities.ownsConflictingRoute.Add(conflictingRoute);

        /// <summary>
        /// Add an AssociatedNetElement to the train station
        /// </summary>
        /// <param name="side"></param>
        /// <param name="direction"></param>
        /// <param name="bounds"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public AssociatedNetElement MakeNetElement(Side side, ApplicationDirection direction, IntrinsicCoordinate bounds, LinearElementWithLength edge)
        {
            var netElement = new AssociatedNetElement
            {
                appliesInDirection = direction,
                hasLateralPosition = new LateralSide() { side = side },
                netElement = new tElementWithIDref(edge.id ?? ""),
                bounds = { new tElementWithIDref(bounds.id ?? "") }
            };

            return netElement;
        }

        /// <summary>
        /// Add a SpotLocation to the train station
        /// </summary>
        /// <param name="side"></param>
        /// <param name="direction"></param>
        /// <param name="bounds"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        private SpotLocation AddSpotLocation(Side side, ApplicationDirection direction, IntrinsicCoordinate bounds, LinearElementWithLength edge)
        {
            var netElement = MakeNetElement(side, direction, bounds, edge);

            var location = new SpotLocation();
            location.associatedNetElements.Add(netElement);
            location.id = IdManager.computeUuid5<SpotLocation>($"{netElement.bounds.Single().@ref}.{netElement?.netElement?.@ref}.{netElement?.hasLateralPosition}.{netElement?.appliesInDirection}");

            AddSpotLocation(location);
            return location;
        }

        /// <summary>
        /// Add a LinearLocation to the train station
        /// </summary>
        /// <param name="side"></param>
        /// <param name="direction"></param>
        /// <param name="bounds"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        private LinearLocation AddLinearLocation(Side side, ApplicationDirection direction, IntrinsicCoordinate bounds, LinearElementWithLength edge)
        {
            var netElement = MakeNetElement(side, direction, bounds, edge);

            var location = new LinearLocation();
            location.associatedNetElements.Add(netElement);
            location.id = IdManager.computeUuid5<LinearLocation>($"{netElement.bounds.Single().@ref}.{netElement?.netElement?.@ref}.{netElement?.hasLateralPosition}.{netElement?.appliesInDirection}");

            AddLocation(location);
            return location;
        }

        /// <summary>
        /// Add an Edge to the train station
        /// </summary>
        /// <param name="length"></param>
        /// <returns>The Edge</returns>
        public LinearElementWithLength AddEdge(double length, string name)
        {
            var edge = new LinearElementWithLength() {
                elementLength = new ElementLength(length),
                name = name,
                id = IdManager.computeUuid5<LinearElementWithLength>(name)
            };
            AddLinearElementWithLength(edge);
            return edge;
        }

        /// <summary>
        /// Add an Edge to the train station
        /// </summary>
        /// <param name="length"></param>
        /// <returns>The Edge</returns>
        [Obsolete("EULYNX DataPrep Length can only store floating point values")]
        public LinearElementWithLength AddEdge(decimal length, string name)
        {
            var edge = new LinearElementWithLength() {
                elementLength = new ElementLength((double)length),
                name = name,
                id = IdManager.computeUuid5<LinearElementWithLength>(name)
            };
            AddLinearElementWithLength(edge);
            return edge;
        }

        /// <summary>
        /// Returns the edge(s) following the given edge in the given direction.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        IEnumerable<LinearElementWithLength>? GetNextEdgeRelationsInDirection(LinearElementWithLength edge, ApplicationDirection direction)
        {
            var relations = GetPositionedRelationsInDirection(edge, direction);

            // We are looking for relations connecting the given edge in the given direction, meaning connecting as elementA or elementB
            return relations?.Select(relation => (GetLinearElementWithLength(relation.elementA?.@ref ?? "") == edge) ?
                GetLinearElementWithLength(relation.elementB?.@ref ?? "") :
                GetLinearElementWithLength(relation.elementA?.@ref ?? "")
            ).OfType<LinearElementWithLength>();
        }

        /// <summary>
        /// Returns the PositionedRelationes that follow on an Edge in a given direction.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public IEnumerable<PositionedRelation> GetPositionedRelationsInDirection(LinearElementWithLength edge, ApplicationDirection direction)
        {
            return RsmEntities.usesTrackTopology?.usesPositionedRelation
                .Where(r =>
                {
                    return (direction == ApplicationDirection.normal) ? r.elementA?.@ref == edge.id : r.elementB?.@ref == edge.id;
                }) ?? Enumerable.Empty<PositionedRelation>();
        }

        /// <summary>
        /// Add an asset of type AxleCountingSection to the train station
        /// </summary>
        /// <param name="name">The name of the AxleCountingSection</param>
        /// <typeparam name="T">The type of AxleCountingSection</typeparam>
        /// <returns>The AxleCountingSection</returns>
        public AxleCountingSectionBuilder<T> AddAxleCountingSection<T>(string name) where T : AxleCountingSection, new()
        {
            return new AxleCountingSectionBuilder<T>(name, this);
        }

        /// <summary>
        /// Add an AxleCountingHead to the train station
        /// </summary>
        /// <param name="name"></param>
        /// <param name="edge"></param>
        /// <param name="position"></param>
        /// <param name="tvpSectionIds"></param>
        public AxleCountingHead AddAxleCountingHead(string name, LinearElementWithLength edge, double position)
        {
            var bounds = new IntrinsicCoordinate
            {
                value = position,
                id = IdManager.computeUuid5<IntrinsicCoordinate>(name + position.ToString())
            };

            var location = new SpotLocation();
            var netElement = MakeNetElement(Side.right, ApplicationDirection.undefined, bounds, edge);

            location.associatedNetElements.Add(netElement);
            location.id = IdManager.computeUuid5<SpotLocation>($"{bounds.id}.{edge.id}");

            var rsmHead = new VehiclePassageDetector
            {
                name = name,
                longname = name,
                id = IdManager.computeUuid5<Models.TopoModels.EULYNX.rsmSig.VehiclePassageDetector>(name)
            };
            rsmHead.locations.Add(new tElementWithIDref(location.id!));

            var head = new AxleCountingHead
            {
                refersToRsmVehiclePassageDetector = new tElementWithIDref(rsmHead.id!),
                id = IdManager.computeUuid5<AxleCountingHead>(rsmHead.name)
            };

            AddCoordinates(bounds);
            AddSpotLocation(location);
            AddTrackSignallingDevice(rsmHead);
            AddTrackAsset(head);

            return head;
        }

        /// <summary>
        /// Returns the AxleCountingHead that refers to the given VehiclePassageDetector.
        /// </summary>
        /// <param name="vehiclePassageDetector"></param>
        /// <returns></returns>
        AxleCountingHead GetAxleCountingHeadForVehiclePassageDetector(VehiclePassageDetector vehiclePassageDetector)
        {
            return DataPrepEntities.ownsTrackAsset
                .OfType<AxleCountingHead>()
                .Where(x => x?.refersToRsmVehiclePassageDetector?.@ref == vehiclePassageDetector.id)
                .Single();
        }

        /// <summary>
        /// Returns all VehiclePassageDetectors on the given Edge.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        IEnumerable<VehiclePassageDetector> GetVehiclePassageDetectorsOnEdge(LinearElementWithLength edge)
        {
            return RsmEntities.ownsOnTrackSignallingDevice
                .OfType<VehiclePassageDetector>()
                .Where(x => GetSpotLocation(x.locations.Single().@ref ?? "")
                    .associatedNetElements.Single().netElement?.@ref == edge.id);
        }

        /// <summary>
        /// Finds VehiclePassageDetectors on the given Edge, compares their position using the given lambda and returns the associated AxleCountingHeads.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="comparePosition"></param>
        /// <returns></returns>
        IEnumerable<TvpSection> GetTvpSectionsForEdgeWithPosition(LinearElementWithLength edge, Func<double, bool> comparePosition)
        {
            var vehiclePassageDetectors = GetVehiclePassageDetectorsOnEdge(edge) ?? new List<VehiclePassageDetector>();
            var vehiclePassageDetectorsWithPosition = vehiclePassageDetectors
                .Select(x => (VehiclePassageDetector: x, Position: GetCoordinates(
                    GetSpotLocation(x.locations.Single().@ref ?? "")?.associatedNetElements.Single().bounds.Single().@ref ?? "")?.value))
                .Where(x => x.Position != null && comparePosition(x.Position.Value))
                .Select(x => x.VehiclePassageDetector);

            var axleCountingHeads = vehiclePassageDetectorsWithPosition.Select(v => GetAxleCountingHeadForVehiclePassageDetector(v)).OfType<AxleCountingHead>();
            return axleCountingHeads.SelectMany(a => a.limitsTdsSection.Select(l => GetTrackAsset(l.@ref)).OfType<TvpSection>()).Distinct();
        }

        /********************************* SIGNALLING *********************************/

        /// <summary>
        /// Adds the SignalFunction for a given signal.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        private SignalFunction AddSignalFunction(SignalFunctionTypes function)
        {
            var signalFunction = new SignalFunction();
            signalFunction.description = function.ToString();
            signalFunction.id = IdManager.computeUuid5<SignalFunction>(function.ToString());
            signalFunction.isOfSignalFunctionType = function;
            return signalFunction;
        }

        /// <summary>
        /// Adds the SignalType for a given signal.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private SignalType AddSignalType(SignalTypeTypes type)
        {
            var signalType = new SignalType();
            signalType.isOfSignalTypeType = type;
            signalType.id = IdManager.computeUuid5<SignalType>(type.ToString());
            return signalType;
        }

        /// <summary>
        /// Adds an RsmSignal for a given signal.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        private RsmSignal AddRsmSignal(string name, List<tElementWithIDref> locations)
        {
            var rsmSignal = new RsmSignal();
            rsmSignal.name = name;
            rsmSignal.longname = name;
            rsmSignal.locations = locations;
            rsmSignal.id = IdManager.computeUuid5<RsmSignal>(name);

            AddRsmSignal(rsmSignal);
            return rsmSignal;
        }

        /// <summary>
        /// Add an asset of type Signal to the train station
        /// </summary>
        /// <param name="name"></param>
        /// <param name="edge"></param>
        /// <param name="offset">The distance offset on that edge</param>
        /// <param name="side">The side (left/right) on which the signal is positioned</param>
        /// <param name="applicationDirection"></param>
        /// <param name="signalTypeType"></param>
        /// <param name="signalFunctionType"></param>
        /// <typeparam name="T">The type of Signal</typeparam>
        /// <returns>The Signal</returns>
        public T AddSignal<T>(
            string name,
            LinearElementWithLength edge,
            double offset,
            Side side,
            ApplicationDirection applicationDirection,
            SignalTypeTypes signalTypeType, SignalFunctionTypes signalFunctionType) where T : Signal, new()
        {
            SignalType signalType = AddSignalType(signalTypeType);
            SignalFunction signalFunction = AddSignalFunction(signalFunctionType);

            var position = new IntrinsicCoordinate();
            position.value = offset;
            position.id = IdManager.computeUuid5<IntrinsicCoordinate>($"{position.value.ToString()}.{name}");
            AddCoordinates(position);

            var location = AddSpotLocation(side, applicationDirection, position, edge);
            var rsmSignal = AddRsmSignal(name, new List<tElementWithIDref> { new tElementWithIDref(location.id!) });

            var signal = new T();
            signal.hasProperty = new List<SignalProperty> { signalFunction, signalType };
            signal.id = IdManager.computeUuid5<Signal>(rsmSignal.id!);
            signal.refersToRsmSignal = new tElementWithIDref(rsmSignal.id!);
            AddTrackAsset(signal);

            return signal;
        }

        /// <summary>
        /// Sort Signals by their position using the given direction.
        /// </summary>
        /// <param name="signals"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public IEnumerable<Signal> OrderedSignals(List<Signal> signals, ApplicationDirection direction)
            => direction == ApplicationDirection.normal ?
                signals.OrderBy(x => GetSignalCoordinate(x)?.value) :
                signals.OrderByDescending(x => GetSignalCoordinate(x)?.value);

        /// <summary>
        /// Returns all the Signals associated with the given edge.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public IEnumerable<Signal> GetSignalsForEdge(LinearElementWithLength edge)
        {
            var signals = DataPrepEntities.ownsTrackAsset.OfType<Signal>();
            return signals.Where(x => GetEdgeForSignal(x) == edge);
        }

        /// <summary>
        /// Returns the direction for a given signal on its edge.
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public ApplicationDirection? GetSignalDirection(Signal signal)
        {
            return GetSpotLocation(
                GetRsmSignal(signal.refersToRsmSignal?.@ref ?? "")?.locations.Single().@ref ?? ""
            ).associatedNetElements.Single().appliesInDirection;
        }

        /// <summary>
        /// Returns the coordinates for a given signal on its edge.
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public IntrinsicCoordinate GetSignalCoordinate(Signal signal) {
            if (signal.refersToRsmSignal == null)
                throw new ArgumentException();

            var rsmSignal = DataPrep.GetById<RsmSignal>(signal.refersToRsmSignal) ?? throw new ArgumentException();

            var spotLocation = DataPrep.GetById<SpotLocation>(rsmSignal.locations.Single()) ?? throw new ArgumentException();

            return GetCoordinates(spotLocation.associatedNetElements.Single().bounds.Single().@ref) ?? throw new ArgumentException();
        }

        // GetCoordinates(
        //     GetSpotLocation(
        //         GetRsmSignal(signal.refersToRsmSignal?.@ref)?.locations.Single().@ref
        //     )?
        // .associatedNetElements.Single().bounds.Single().@ref);

        /// <summary>
        /// Returns all main signals with a direction on a given edge.
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public List<Signal> GetMainSignalsWithDirection(LinearElementWithLength edge, ApplicationDirection direction) =>
            GetSignalsForEdge(edge).Where(x => GetSignalDirection(x) == direction && IsMainSignal(x)).ToList();

        /// <summary>
        /// Returns whether a given signal is a main signal.
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public bool IsMainSignal(Signal signal) =>
            signal.hasProperty.OfType<SignalType>().Any(p => p.isOfSignalTypeType == SignalTypeTypes.main);

        IEnumerable<TvpSection>? GetTvpSectionsForEdgeWithPosition(LinearElementWithLength edge, Func<double?, Boolean> comparePosition, ApplicationDirection? direction)
        {
            Func<VehiclePassageDetector, double?> GetLocationValue = (VehiclePassageDetector vpd) => GetCoordinates(
                    GetSpotLocation(vpd.locations.Single().@ref ?? "")?.associatedNetElements.Single().bounds.Single().@ref ?? ""
                )?.value;
            var vehiclePassageDetectors = GetVehiclePassageDetectorsOnEdge(edge);
            var vehiclePassageDetectorsWithPosition = vehiclePassageDetectors?
                .Where(h => comparePosition(GetLocationValue(h)));
            var sortedVehiclePassageDetectorsWithPosition = (direction == ApplicationDirection.normal)
                ? vehiclePassageDetectorsWithPosition?.OrderBy(v => GetLocationValue(v))
                : vehiclePassageDetectorsWithPosition?.OrderByDescending(v =>GetLocationValue(v)
            );
            var axleCountingHeads = vehiclePassageDetectorsWithPosition?
                .Select(v => GetAxleCountingHeadForVehiclePassageDetector(v))
                .OfType<AxleCountingHead>();
            var orderedSections = axleCountingHeads?.SelectMany(a => a.limitsTdsSection);
            return (direction == ApplicationDirection.normal) ?
                orderedSections?.Select(l => GetTrackAsset(l.@ref)).OfType<TvpSection>().Distinct() :
                orderedSections?.Reverse().Select(l => GetTrackAsset(l.@ref)).OfType<TvpSection>().Distinct();
        }

        /// <summary>
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        bool BeforeSignal(Signal signal, ApplicationDirection direction, SpotLocation l)
        {
            var location = l.associatedNetElements.Single().bounds.Single().@ref;
            var position = GetCoordinates(location)?.value;

            return (direction == ApplicationDirection.normal) ?
                GetSignalCoordinate(signal)?.value > position : GetSignalCoordinate(signal)?.value < position;
        }

        /// <summary>
        /// Returns whether the given position is positioned after the signal's position on an edge.
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        bool BehindSignal(Signal signal, ApplicationDirection direction, SpotLocation l)
        {
            var location = l.associatedNetElements.Single().bounds.Single().@ref;
            var position = GetCoordinates(location)?.value;
            return (direction == ApplicationDirection.normal) ?
                GetSignalCoordinate(signal)?.value < position : GetSignalCoordinate(signal)?.value > position;
        }

        public List<TvpSection> GetOverlappingTvpSections(List<(string Edge, double Offset)> area) {
            var offsetsByEdge =
                from bounds in area
                group bounds by bounds.Edge into b
                select b;

            return (
                from section in DataPrep.Get<TvpSection>()
                let areaLocation = DataPrep.GetById<AreaLocation>(section.isLocatedAt!)
                from edge in areaLocation.associatedNetElements.Select(x => (
                    Bounds: x.bounds.Select(b => DataPrep.GetById<IntrinsicCoordinate>(b)).ToList(),
                    Edge: x.netElement!.@ref
                ))
                join bounds in offsetsByEdge on edge.Edge equals bounds.Key
                where edge.Bounds.Count == 2 && bounds.Count() == 2
                let orderedBoundsTvp = edge.Bounds.OrderBy(x => x.value).ToList()
                let orderedBoundsRoute = bounds.OrderBy(x => x.Offset).ToList()
                let tStart = orderedBoundsTvp.First().value!.Value
                let tEnd = orderedBoundsTvp.Last().value!.Value
                let rStart = bounds.First().Offset
                let rEnd = bounds.Last().Offset
                where
                    tStart <= rStart && rStart < tEnd ||
                    tStart < rEnd && rEnd <= tEnd
                select section
            ).Distinct().ToList();
        }

        // public List<TvpSection> GetOverlappingTvpSections(LinearElementWithLength edge, ApplicationDirection direction, Signal? start, Signal? end) {
        //     var tvpSections = DataPrepEntities.ownsTrackAsset.OfType<TvpSection>()
        //         .Select(x => (
        //             Section: x,
        //             Heads: DataPrepEntities.ownsTrackAsset.OfType<AxleCountingHead>().Where(head => head.limitsTdsSection.Any(limits => limits.@ref == x.id)))
        //         ).Select(x => (x.Section, Heads: x.Heads.Select(head => (
        //             Head: head,
        //             VehiclePassageDetector: RsmEntities.ownsOnTrackSignallingDevice
        //                 .OfType<VehiclePassageDetector>()
        //                 .Single(x => x.id == head.refersToRsmVehiclePassageDetector.@ref))).ToList())
        //         ).Select(x => (x.Section, Heads: x.Heads.Select(head => (
        //             head.Head,
        //             head.VehiclePassageDetector,
        //             Location: GetSpotLocation(head.VehiclePassageDetector.locations.Single().@ref))))
        //         ).Where(x => x.Heads.Any(head => head.Location.associatedNetElements.Single().netElement?.@ref == edge.id)).ToList();

        //     if (start == null && end == null) {
        //         return tvpSections.Select(x => x.Section).ToList();
        //     }

        //     return tvpSections.Where(x =>
        //         // Case 1: If no start is provided: At least one head sits before the end signal
        //         (start == null && x.Heads.Any(head => BeforeSignal(end, direction, head.Location))) ||
        //         // Case 2: One head sits before the start signal, another head behind
        //         (start != null && x.Heads.Any(head => BeforeSignal(start, direction, head.Location)) && x.Heads.Any(head => BehindSignal(start, direction, head.Location))) ||
        //         // Case 3: If no end is provided: At least one head sits behind the start signal
        //         (end == null && x.Heads.Any(head => BehindSignal(start, direction, head.Location))) ||
        //         // Case 4: At least one head sits between start and end signal
        //         (start != null && end != null && x.Heads.Any(head => BehindSignal(start, direction, head.Location) && BeforeSignal(end, direction, head.Location)))
        //     ).Select(x => x.Section).ToList();
        // }

        /// <summary>
        /// Returns the edge (LinearElementWithLength) that is associated with the given signal.
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public LinearElementWithLength? GetEdgeForSignal(Signal signal)
        {
            var rsmSignal = GetRsmSignal(signal.refersToRsmSignal?.@ref ?? "");
            var element = GetSpotLocation(rsmSignal?.locations.Single().@ref ?? "")
                .associatedNetElements?.Single().netElement?.@ref;
            return GetLinearElementWithLength(element ?? "");
        }

        /********************************* POINT *********************************/

        /// <summary>
        /// Add a PositionedRelation to the train station
        /// </summary>
        /// <param name="edge1"></param>
        /// <param name="edge2"></param>
        /// <param name="leftRight"></param>
        /// <param name="navigability"></param>
        /// <param name="positionOnA"></param>
        /// <param name="positionOnB"></param>
        /// <returns>The PositionedRelation</returns>
        public PositionedRelation ConnectEndToStart(LinearElementWithLength edge1, LinearElementWithLength edge2, LeftRight leftRight,
            Navigability? navigability = null, Usage? positionOnA = null, Usage? positionOnB = null)
        {
            var relation = new PositionedRelation($"{edge1.id}.{edge2.id}");
            relation.id = $"{edge1.id}.{edge2.id}";
            relation.elementA = new tElementWithIDref(edge1.id ?? "");
            relation.elementB = new tElementWithIDref(edge2.id ?? "");
            relation.leadsTowards = leftRight;
            relation.navigability = navigability ?? Navigability.Both;
            relation.positionOnA = positionOnA ?? Usage.end;
            relation.positionOnB = positionOnB ?? Usage.start;
            AddPositionedRelation(relation);
            return relation;
        }

        /// <summary>
        /// Add an asset of type Point to the train station
        /// </summary>
        /// <param name="relations"></param>
        /// <param name="orientation"></param>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The point</returns>
        public T AddPoint<T>(
            IEnumerable<PositionedRelation> relations,
            TurnoutOrientation orientation,
            string name
        ) where T : Turnout, new()
        {
            var point = new T();
            point.handles.AddRange(relations
                .Where(r => r.id != null)
                .Select(relation => new tElementWithIDref(relation.id ?? ""))
            );
            point.orientation = orientation;
            point.name = name;
            point.id = IdManager.computeUuid5<RastaTurnout>(point.name);
            AddPoint(point);
            return point;
        }

        /// <summary>
        /// Returns the Point (Turnout) that follows on an Edge in a given direction.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Turnout? GetNextPoint(LinearElementWithLength edge, ApplicationDirection direction)
        {
            var relations = GetPositionedRelationsInDirection(edge, direction);
            return GetPointFromRelations(relations);
        }

        /// <summary>
        /// Returns the Point (Turnout) that corresponds to the given PositionedRelations.
        /// </summary>
        /// <param name="relations"></param>
        /// <returns></returns>
        Turnout? GetPointFromRelations(IEnumerable<PositionedRelation>? relations)
        {
            var points = RsmEntities.ownsPoint
            .Where(x => x.handles.Any(h =>
            {
                return (relations != null) ?
                    relations.Contains(GetPositionedRelation(h.@ref)) :
                    false;
            }));
            if (points?.Count() > 0)
            {
                return points.Single();
            }
            return null;
        }

        /********************************* ROUTE *********************************/

        /// <summary>
        /// Adds a new RsmRouteBody to the station.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="locations"></param>
        /// <returns></returns>
        private RsmRouteBody AddRsmRouteBody(string name, List<tElementWithIDref> locations)
        {
            var rsmRouteBody = new RsmRouteBody();
            var id = String.Join("", locations.Select(l => l.@ref));
            rsmRouteBody.id = IdManager.computeUuid5<RsmRouteBody>(id);
            rsmRouteBody.locations = locations;
            rsmRouteBody.name = name;
            rsmRouteBody.longname = rsmRouteBody.name;

            AddRsmRouteBody(rsmRouteBody);
            return rsmRouteBody;
        }

        /// <summary>
        /// Adds a new RouteEntry to the station.
        /// </summary>
        /// <param name="entrySignal"></param>
        /// <returns></returns>
        private RouteEntry AddRouteEntry(tElementWithIDref entrySignal)
        {
            var routeEntry = new RouteEntry();
            routeEntry.id = IdManager.computeUuid5<RouteEntry>($"{entrySignal}");
            routeEntry.isAssociatedWithSignal = entrySignal;
            return routeEntry;
        }

        /// <summary>
        /// Adds a new RouteExit to the station.
        /// </summary>
        /// <param name="exitSignal"></param>
        /// <returns></returns>
        private RouteExit AddRouteExit(tElementWithIDref exitSignal)
        {
            var routeExit = new RouteExit();
            routeExit.id = IdManager.computeUuid5<RouteEntry>($"{exitSignal}");
            routeExit.isAssociatedWithSignal = exitSignal;
            return routeExit;
        }

        /// <summary>
        /// Adds a SectionList of a given route.
        /// </summary>
        /// <param name="routeBody"></param>
        /// <param name="listOfSections"></param>
        /// <returns></returns>
        public SectionList AddSectionList(RouteBody routeBody, List<tElementWithIDref> listOfSections)
        {
            var sectionList = new SectionList();
            sectionList.appliesToRouteBody = new tElementWithIDref(routeBody.id ?? "");
            sectionList.hasSection = listOfSections;

            AddSectionList(sectionList);
            return sectionList;
        }

        /// <summary>
        /// Adds ConflictingRoutes to a route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="conflictedRoutes"></param>
        /// <returns></returns>
        public ConflictingRoute AddConflictingRoute(tElementWithIDref route, List<tElementWithIDref> conflictedRoutes)
        {
            var conflictingRoute = new ConflictingRoute();
            conflictingRoute.hasConflictsWithRoute = conflictedRoutes;
            conflictingRoute.requestedRoute = route;

            AddConflictingRoute(conflictingRoute);
            return conflictingRoute;
        }

        /// <summary>
        /// Adds a new RouteBody to the station, complete with -Entry, -Exit and Location.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entrySignal"></param>
        /// <param name="exitSignal"></param>
        /// <returns></returns>
        public RouteBody AddRouteBody(string name, tElementWithIDref entrySignal, tElementWithIDref exitSignal)
        {
            var associatedNetElements = new [] { entrySignal, exitSignal }.Select(t =>
                GetSpotLocation(
                    GetRsmSignal(
                        ((Signal)GetTrackAsset(t.@ref))?.refersToRsmSignal?.@ref)?
                    .locations.Single().@ref ?? "")?
                .associatedNetElements.Single()
            ).OfType<AssociatedNetElement>();

            // netElement & rsmRouteBody
            var location = new LinearLocation();
            location.associatedNetElements.AddRange(associatedNetElements);
            location.id = IdManager.computeUuid5<LinearLocation>($"{entrySignal.@ref}.{exitSignal.@ref}");
            AddLocation(location);
            var rsmRouteBody = AddRsmRouteBody(name,
                new List<tElementWithIDref> { new tElementWithIDref(location.id!) }.ToList()
            );

            // routeBody, routeEntry & routeExit
            var routeBody = new RouteBody();
            routeBody.id = IdManager.computeUuid5<RouteBody>(rsmRouteBody.id!);
            routeBody.refersToRsmRouteBody = new tElementWithIDref(rsmRouteBody.id!);

            var routeEntry = AddRouteEntry(entrySignal);
            routeEntry.id = IdManager.computeUuid5<RouteEntry>($"{name}.{GetTrackAsset(entrySignal.@ref)?.id}");
            routeBody.hasEntry = routeEntry;

            var routeExit = AddRouteExit(exitSignal);
            routeExit.id = IdManager.computeUuid5<RouteExit>($"{name}.{GetTrackAsset(exitSignal.@ref)?.id}");
            routeBody.hasExit = routeExit;

            AddRouteBody(routeBody);
            return routeBody;
        }

        public PointElementAndPosition AddPointElementAndPosition(RouteBody routeBody, Turnout point, LeftRight position)
        {
            var peap = new PointElementAndPosition()
            {
                refersToMovableElement = new tElementWithIDref(point.id ?? ""),
                inPosition = position == LeftRight.left ?
                    Models.TopoModels.EULYNX.generic.LeftRight.left :
                    Models.TopoModels.EULYNX.generic.LeftRight.right
            };
            peap.id = IdManager.computeUuid5<PointElementAndPosition>($"{point.name}.{position.ToString()}");
            var peapReference = new tElementWithIDref(peap.id ?? "");
            if (!pointElementsAndPositions.Contains(peap.id ?? ""))
            {
                pointElementsAndPositions.Add(peap.id ?? "");
                DataPrep.hasDataContainer[0].ownsDataPrepEntities?.knowsAssetAndState.Add(peap);
            }
            routeBody.requiresMovableElementInPositionInRouteBody.Add(peapReference);

            return peap;
        }
    }
}
