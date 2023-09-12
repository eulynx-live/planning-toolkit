using System.Security.Cryptography.X509Certificates;
using Models.TopoModels.EULYNX.db;
using Models.TopoModels.EULYNX.generic;
using Models.TopoModels.EULYNX.rsmCommon;
using Models.TopoModels.EULYNX.sig;
using VehiclePassageDetector = Models.TopoModels.EULYNX.rsmSig.VehiclePassageDetector;

namespace PlanningToolkit.Builder
{
    public class AxleCountingSectionBuilder<T> where T : AxleCountingSection, new() {
        private readonly string _name;
        private readonly PlanningBuilder _builder;
        private readonly TvpSection _tvpSection;
        private readonly AreaLocation _areaLocation;
        private readonly T _axleCountingSection;
        private readonly List<AxleCountingHead> _heads = new();
        private readonly List<IntrinsicCoordinate> _extraCoordinates = new();
        private readonly List<(AssociatedNetElement NetElement, IntrinsicCoordinate Offset)> _bounds = new();

        public AxleCountingSectionBuilder(string name, PlanningBuilder builder)
        {
            _name = name;
            _builder = builder;
            _areaLocation = new AreaLocation
            {
                id = IdManager.computeUuid5<AreaLocation>(_name),
                associatedNetElements = new List<AssociatedNetElement>()
            };
            _tvpSection = new TvpSection
            {
                id = IdManager.computeUuid5<TvpSection>(_name),
                isLocatedAt = new tElementWithIDref(_areaLocation.id!)
            };
            _axleCountingSection = new T
            {
                id = IdManager.computeUuid5<AxleCountingSection>(_name),
                appliesToTvpSection = new tElementWithIDref(_tvpSection.id!),
                hasConfiguration = new Configuration()
            };
            _axleCountingSection.hasConfiguration.hasConfigurationProperty.Add(new TdsDesignation { localName = _name, longNameLayoutPlan = _name });
        }

        public AxleCountingSectionBuilder<T> LimitedByUnconnectedEndOfEdge(LinearElementWithLength edge) {
            var positionedRelationsA = _builder.DataPrep.Get<PositionedRelation>().Where(x => x.elementA?.@ref == edge.id);
            var positionedRelationsB = _builder.DataPrep.Get<PositionedRelation>().Where(x => x.elementB?.@ref == edge.id);

            var startConnected = positionedRelationsA.Any(x => x.positionOnA == Usage.start)
                || positionedRelationsB.Any(x => x.positionOnB == Usage.start);
            var endConnected = positionedRelationsA.Any(x => x.positionOnA == Usage.end)
                || positionedRelationsB.Any(x => x.positionOnB == Usage.end);

            if (startConnected && endConnected) {
                throw new Exception("Both ends of edge are connected");
            } else if (startConnected || endConnected) {
                var offset = startConnected ? 0 : 1;
                var bounds = new IntrinsicCoordinate
                {
                    value = offset,
                    id = IdManager.computeUuid5<IntrinsicCoordinate>($"{edge.id}.{offset}")
                };
                _extraCoordinates.Add(bounds);

                var netElement = _builder.MakeNetElement(Side.undefined, ApplicationDirection.undefined, bounds, edge);
                _bounds.Add((netElement, bounds));
            } else {
                throw new Exception("No end of edge is connected");
            }

            return this;
        }

        public AxleCountingSectionBuilder<T> CoveringEdge(LinearElementWithLength edge) {
            var start = new IntrinsicCoordinate
            {
                value = 0,
                id = IdManager.computeUuid5<IntrinsicCoordinate>($"{edge.id}.0")
            };
            _extraCoordinates.Add(start);
            var end = new IntrinsicCoordinate
            {
                value = 1,
                id = IdManager.computeUuid5<IntrinsicCoordinate>($"{edge.id}.1")
            };
            _extraCoordinates.Add(start);
            _extraCoordinates.Add(end);

            var netElementStart = _builder.MakeNetElement(Side.undefined, ApplicationDirection.undefined, start, edge);
            var netElementEnd = _builder.MakeNetElement(Side.undefined, ApplicationDirection.undefined, end, edge);
            _bounds.Add((netElementStart, start));
            _bounds.Add((netElementEnd, end));

            return this;
        }

        public AxleCountingSectionBuilder<T> WithAssociatedHead(AxleCountingHead head) {
            _heads.Add(head);

            var rsmHead = _builder.DataPrep.GetById<VehiclePassageDetector>(head.refersToRsmVehiclePassageDetector!) ?? throw new Exception("Could not resolve rsm head");
            var headLocation = _builder.DataPrep.GetById<SpotLocation>(rsmHead.locations.Single()) ?? throw new Exception("Could not resolve rsm head location");
            var netElement = headLocation.associatedNetElements.Single();
            var bounds = _builder.DataPrep.GetById<IntrinsicCoordinate>(netElement.bounds.Single())!;
            _bounds.Add((netElement, bounds));

            return this;
        }

        private void ComputeAreaLocation() {
            var boundsOnEdges = _bounds.Select(x => (Edge: x.NetElement.netElement!.@ref, x.Offset));
            var edges = boundsOnEdges.Select(x => x.Edge).Distinct();

            foreach (var bound in boundsOnEdges) {
                // Find the corresponding bound

                // Case 1: Other bound on the same edge
                var other = boundsOnEdges.SingleOrDefault(x => x != bound && x.Edge == bound.Edge);
                if (other != default) {
                    _areaLocation.associatedNetElements.Add(new AssociatedNetElement{
                        appliesInDirection = ApplicationDirection.undefined,
                        hasLateralPosition = new LateralSide() { side = Side.undefined },
                        netElement = new tElementWithIDref(bound.Edge),
                        bounds = new [] { bound.Offset, other.Offset }
                            .OrderBy(x => x.value).Select(x => new tElementWithIDref(x.id!)).ToList()
                    });
                    continue;
                }

                // Case 2: One end of the edge is connected to at least another edge with a bound
                var a = _builder.DataPrep.Get<PositionedRelation>()
                    .Where(x => x.elementA?.@ref == bound.Edge)
                    .Where(x => boundsOnEdges.Any(b => b.Edge == x.elementB!.@ref))
                    .Select(x => x.positionOnA);
                var b = _builder.DataPrep.Get<PositionedRelation>()
                    .Where(x => x.elementB?.@ref == bound.Edge)
                    .Where(x => boundsOnEdges.Any(b => b.Edge == x.elementA!.@ref))
                    .Select(x => x.positionOnB);

                var connectedEnds = a.Concat(b).Distinct().ToList();

                if (connectedEnds.Contains(Usage.start) && connectedEnds.Contains(Usage.end)) {
                    throw new Exception("Axle counting section cannot contain entire edge with head on it");
                } else if (connectedEnds.Contains(Usage.start)) {
                    var start = new IntrinsicCoordinate
                    {
                        value = 0,
                        id = IdManager.computeUuid5<IntrinsicCoordinate>($"{bound.Edge}.0")
                    };
                    _extraCoordinates.Add(start);
                    _areaLocation.associatedNetElements.Add(new AssociatedNetElement{
                        appliesInDirection = ApplicationDirection.undefined,
                        hasLateralPosition = new LateralSide() { side = Side.undefined },
                        netElement = new tElementWithIDref(bound.Edge),
                        bounds = new [] { start, bound.Offset }
                            .OrderBy(x => x.value).Select(x => new tElementWithIDref(x.id!)).ToList()
                    });
                    continue;
                } else if (connectedEnds.Contains(Usage.end)) {
                    var end = new IntrinsicCoordinate
                    {
                        value = 1,
                        id = IdManager.computeUuid5<IntrinsicCoordinate>($"{bound.Edge}.1")
                    };
                    _extraCoordinates.Add(end);
                    _areaLocation.associatedNetElements.Add(new AssociatedNetElement{
                        appliesInDirection = ApplicationDirection.undefined,
                        hasLateralPosition = new LateralSide() { side = Side.undefined },
                        netElement = new tElementWithIDref(bound.Edge),
                        bounds = new [] { end, bound.Offset }
                            .OrderBy(x => x.value).Select(x => new tElementWithIDref(x.id!)).ToList()
                    });
                    continue;
                }

                throw new Exception("Bounds of axle counting section are not fully defined.");
            }
        }

        public T Build() {
            ComputeAreaLocation();

            foreach (var bound in _extraCoordinates) {
                _builder.AddCoordinates(bound);
            }

            _builder.AddLocation(_areaLocation);
            _builder.AddTrackAsset(_tvpSection);
            _builder.AddTrackAsset(_axleCountingSection);

            foreach (var head in _heads) {
                head.limitsTdsSection.Add(new tElementWithIDref(_axleCountingSection.id!));
            }

            return _axleCountingSection;
        }
    }
}
