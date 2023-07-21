using Models.TopoModels.EULYNX.generic;
using EulynxAxleCountingHead = Models.TopoModels.EULYNX.sig.AxleCountingHead;
using Models.TopoModels.EULYNX.rsmCommon;

using Models.TopoModels.EULYNX.rsmSig;
using Models.TopoModels.EULYNX.sig;
using rsmSignal = Models.TopoModels.EULYNX.rsmSig.Signal;

using Models.TopoModels.EULYNX.rsmNE;
using Signal = Models.TopoModels.EULYNX.sig.Signal;
using RsmSignal = Models.TopoModels.EULYNX.rsmSig.Signal;
using VehiclePassageDetector = Models.TopoModels.EULYNX.rsmSig.VehiclePassageDetector;
using Models.TopoModels.EULYNX.rsmTrack;

namespace PlanningToolkit
{
    public static class EulynxExtensions
    {
        private static bool IsSubclassOf<BaseClass, T>() => typeof(BaseClass).IsAssignableFrom(typeof(T));

        public static T? GetById<T>(this EulynxDataPrepInterface This, tElementWithIDref idRef) where T : Models.TopoModels.EULYNX.rsmCommon.BaseObject
        {
            var dataPrep = This.hasDataContainer.Single().ownsDataPrepEntities!;
            var rsm = This.hasDataContainer.Single().ownsRsmEntities!;

            if (IsSubclassOf<TrackAsset, T>()) {
                return dataPrep.ownsTrackAsset.OfType<T>().ResolveByIdRef(idRef);
            } else if (IsSubclassOf<OnTrackSignallingDevice, T>()) {
                return rsm.ownsOnTrackSignallingDevice.OfType<T>().ResolveByIdRef(idRef);
            } else if (IsSubclassOf<BaseLocation, T>()) {
                return rsm.usesLocation.OfType<T>().ResolveByIdRef(idRef);
            }
            else if (IsSubclassOf<PositioningNetElement, T>())
            {
                return rsm.usesTrackTopology?.usesNetElement?.OfType<T>().ResolveByIdRef(idRef);
            }
            else if (IsSubclassOf<IntrinsicCoordinate, T>())
            {
                return rsm.usesTopography?.usesIntrinsicCoordinate?.OfType<T>().ResolveByIdRef(idRef);
            }
            else if (IsSubclassOf<LocationProxy, T>())
            {
                return dataPrep.ownsLocationProxy.OfType<T>().ResolveByIdRef(idRef);
            }
            else if (IsSubclassOf<BaseLocation, T>())
            {
                return rsm.usesLocation.OfType<T>().ResolveByIdRef(idRef);
            }
            else if (IsSubclassOf<rsmSignal, T>())
            {
                return rsm.ownsSignal.OfType<T>().ResolveByIdRef(idRef);
            }

            throw new NotImplementedException();
        }

        public static T? ResolveByIdRef<T>(this IEnumerable<T> collection, tElementWithIDref idRef) where T : Models.TopoModels.EULYNX.rsmCommon.BaseObject
        {
            return collection.SingleOrDefault(x => x.id == idRef.@ref);
        }

        public static IntrinsicCoordinate GetCoordinate(this IEnumerable<IntrinsicCoordinate> This, string id)
        {
            return This.OfType<IntrinsicCoordinate>().Single(x => x.id == id);
        }

        public static IEnumerable<VehiclePassageDetector> GetVehiclePassageDetectors(this IEnumerable<OnTrackSignallingDevice> This)
        {
            return This.OfType<VehiclePassageDetector>();
        }

        public static VehiclePassageDetector GetVehiclePassageDetector(this IEnumerable<OnTrackSignallingDevice> This, string id)
        {
            return This.GetVehiclePassageDetectors().Single(x => x.id == id);
        }

        public static RsmSignal GetRsmSignal(this IEnumerable<RsmSignal> This, string id)
        {
            return This.OfType<RsmSignal>().Single(x => x.id == id);
        }

        public static PositionedRelation GetPositionedRelation(this IEnumerable<PositionedRelation> This, string id)
        {
            return This.OfType<PositionedRelation>().Single(x => x.id == id);
        }

        public static SpotLocation GetSpotLocation(this IEnumerable<BaseLocation> This, string id)
        {
            return This.OfType<SpotLocation>().Single(x => x.id == id);
        }

        public static OnTrackSignallingDevice GetDevice(this IEnumerable<OnTrackSignallingDevice> This, EulynxAxleCountingHead ach)
        {
            return (
                from device in This
                where device.id == ach.refersToRsmVehiclePassageDetector.@ref
                select device
            ).Single();
        }

        public static SpotLocation GetLocation(this IEnumerable<BaseLocation> This, LocatedNetEntity device)
        {
            return (
                from location in This.OfType<SpotLocation>()
                where location.id == device.locations.Single().@ref
                select location
            ).Single();
        }

        public static IntrinsicCoordinate GetIntrinsicCoordinate(this IEnumerable<IntrinsicCoordinate> This, AssociatedNetElement associatedNetElement)
        {
            return (
                from coordinate in This.OfType<IntrinsicCoordinate>()
                where coordinate.id == associatedNetElement.bounds.Single().@ref
                select coordinate
            ).Single();
        }

        public static rsmSignal GetRsmSignal(this IEnumerable<rsmSignal> This, Signal rastaSignal)
        {
            return (
                from signal in This
                where signal.id == rastaSignal.refersToRsmSignal.@ref
                select signal
            ).Single();
        }

        public static string[] GetAxleCountingHeads(this IEnumerable<TrackAsset> This, AxleCountingSection axleCountingSection)
        {
            return (
                from axleCountingHead in This.OfType<EulynxAxleCountingHead>()
                where axleCountingHead.limitsTdsSection.Any(tdsSection => tdsSection.@ref == axleCountingSection.appliesToTvpSection.@ref)
                select axleCountingHead.id
            ).ToArray();
        }

        public static IEnumerable<Signal> GetSignals(this IEnumerable<TrackAsset> This) => This.OfType<Signal>();

        public static Turnout GetPointFromRelations(this RsmEntities rsmEntities, IEnumerable<PositionedRelation> relations)
        {
            var lookupPositionedRelation = (string id) => rsmEntities.usesTrackTopology.usesPositionedRelation.GetPositionedRelation(id);
            return rsmEntities.ownsPoint
                .Where(x => x.handles.Any(h => relations != null && relations.Contains(lookupPositionedRelation(h.@ref))))
                .Single();
        }

        public static List<PointElementAndPosition> GetPointsInPosition(this IEnumerable<AssetAndState> This, Models.TopoModels.EULYNX.sig.RouteBody routeBody)
        {
            return routeBody.requiresMovableElementInPositionInRouteBody
                .Select(pointPosition => This.OfType<PointElementAndPosition>().Single(x => x.id == pointPosition.@ref)).ToList();
        }

        public static double? GetLength(this LinearElementWithLength element)
        {
            return element.elementLength?.quantity.OfType<Length>().Single().value;
        }
    }
}
