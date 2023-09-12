using Models.TopoModels.EULYNX.generic;
using Models.TopoModels.EULYNX.rsmCommon;

namespace PlanningToolkit.Objects;

public record CoveredEdge(LinearElementWithLength Edge, double Start, double End);

public record AreaLocation(List<CoveredEdge> CoveredEdges) {
    public bool IntersectsWith(Models.TopoModels.EULYNX.rsmCommon.AreaLocation areaLocation, EulynxDataPrepInterface dp) {
        return (
            from coveredEdge in areaLocation.associatedNetElements
            where coveredEdge.bounds.Count == 2
            let edgeId = coveredEdge.netElement!.@ref
            let limits = coveredEdge.bounds.Select(dp.GetById<IntrinsicCoordinate>)
            join x in CoveredEdges on edgeId equals x.Edge.id!
            let otherStart = limits.First().value!.Value
            let otherEnd = limits.Last().value!.Value
            where
                otherStart <= x.Start && x.Start < otherEnd ||
                otherStart < x.End && x.End <= otherEnd
            select edgeId
        ).Any();
    }
}
