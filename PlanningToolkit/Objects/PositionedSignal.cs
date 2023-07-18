
namespace PlanningToolkit.Objects;

public record PositionedSignal(
    Models.TopoModels.EULYNX.rsmSig.Signal RsmSignal,
    Models.TopoModels.EULYNX.sig.Signal Signal,
    Models.TopoModels.EULYNX.rsmCommon.SpotLocation Location,
    Models.TopoModels.EULYNX.rsmCommon.IntrinsicCoordinate Coordinate
);
