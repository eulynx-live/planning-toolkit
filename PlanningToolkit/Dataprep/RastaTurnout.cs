using System.Xml.Serialization;
using Models.TopoModels.EULYNX.rsmTrack;

[Serializable]
[System.ComponentModel.DesignerCategory("code")]
[XmlType(Namespace="http://dataprep.eulynx.eu/schema/EulynxLive/1.1")]
public partial class RastaTurnout: Turnout
{

    [XmlElement("rastaId", Order = 4)]
    public UInt32? RastaId { get; set; }

}
