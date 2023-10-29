using System.Xml.Serialization;
using Models.TopoModels.EULYNX.sig;

[Serializable]
[System.ComponentModel.DesignerCategory("code")]
[XmlType(Namespace="http://dataprep.eulynx.eu/schema/EulynxLive/1.1")]
public partial class RastaAxleCountingSection : AxleCountingSection
{
    [XmlElement("rastaId", Order = 3)]
    public uint? RastaId { get; set; }
}
