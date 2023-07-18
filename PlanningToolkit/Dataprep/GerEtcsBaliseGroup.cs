using Models.TopoModels.EULYNX.rsmCommon;

namespace PlanningToolkit.Dataprep;

using System.Xml.Serialization;
using Models.TopoModels.EULYNX.sig;

[Serializable]
[System.ComponentModel.DesignerCategory("code")]
[XmlType(Namespace="http://dataprep.eulynx.eu/schema/EulynxLive/1.1")]
public partial class GerEtcsBaliseGroup : EtcsBaliseGroup
{
    [XmlElement("hasEtcsDangerpoint", Order = 3)]
    public EtcsDangerpoint? etcsDangerPoint { get; set; }

    [XmlElement("balises", Order = 4)]
    [ReferenceToClass(typeof (EtcsBalise))]
    public List<tElementWithIDref> balises { get; set; } = new ();
}
