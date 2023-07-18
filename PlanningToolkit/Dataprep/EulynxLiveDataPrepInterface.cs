using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;
using Models.TopoModels.EULYNX.generic;
using Models.TopoModels.EULYNX.rsmCommon;

namespace PlanningToolkit.Dataprep;

public class EulynxLiveDataContainer : DataContainer
{

    [XmlElement("ownsEulynxLiveEntities", Order = 4)]
    public EulynxLiveEntities? ownsEulynxLiveEntities { get; set; }
}

public class EulynxLiveEntities
{
    [XmlElement("ownsGerEtcsBaliseGroup", Order = 1)]
    public List<GerEtcsBaliseGroup> ownsGerEtcsBaliseGroup { get; set; } = new();

    [XmlElement("ownsRastaAxleCountingSection", Order = 2)]
    public List<RastaAxleCountingSection> ownsRastaAxleCountingSection { get; set; } = new();

    [XmlElement("ownsRastaSignal", Order = 3)]
    public List<RastaSignal> ownsRastaSignal { get; set; } = new();

    [XmlElement("ownsRastaTurnout", Order = 4)]
    public List<RastaTurnout> ownsRastaTurnout { get; set; } = new();
}
