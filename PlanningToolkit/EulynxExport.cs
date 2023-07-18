using System.Xml;
using System.Xml.Serialization;
using Models.TopoModels.EULYNX.generic;

namespace PlanningToolkit
{
    public class EulynxExport
    {
        private EulynxDataPrepInterface _dp;

        public EulynxExport(EulynxDataPrepInterface dpInterface)
        {
            _dp = dpInterface;
        }

        private Dictionary<string, string> XsdNamespaces = new Dictionary<string, string> {
            ["generic"] = (Attribute.GetCustomAttribute(typeof(Models.TopoModels.EULYNX.generic.BaseObject), typeof(XmlTypeAttribute)) as XmlTypeAttribute)?.Namespace ?? throw new Exception("Missing namespace"),
            ["sig"] = (Attribute.GetCustomAttribute(typeof(Models.TopoModels.EULYNX.sig.LightSignal), typeof(XmlTypeAttribute)) as XmlTypeAttribute)?.Namespace ?? throw new Exception("Missing namespace"),
            ["db"] = (Attribute.GetCustomAttribute(typeof(Models.TopoModels.EULYNX.db.LightSignal), typeof(XmlTypeAttribute)) as XmlTypeAttribute)?.Namespace ?? throw new Exception("Missing namespace"),
            ["sncf"] = (Attribute.GetCustomAttribute(typeof(Models.TopoModels.EULYNX.sncf.Crocodile), typeof(XmlTypeAttribute)) as XmlTypeAttribute)?.Namespace ?? throw new Exception("Missing namespace"),
            ["nr"] = (Attribute.GetCustomAttribute(typeof(Models.TopoModels.EULYNX.nr.LightSignal), typeof(XmlTypeAttribute)) as XmlTypeAttribute)?.Namespace ?? throw new Exception("Missing namespace"),
            ["prorail"] = (Attribute.GetCustomAttribute(typeof(Models.TopoModels.EULYNX.prorail.AtbLoop), typeof(XmlTypeAttribute)) as XmlTypeAttribute)?.Namespace ?? throw new Exception("Missing namespace"),
            ["rfi"] = (Attribute.GetCustomAttribute(typeof(Models.TopoModels.EULYNX.rfi.LensDiffuser), typeof(XmlTypeAttribute)) as XmlTypeAttribute)?.Namespace ?? throw new Exception("Missing namespace"),
            ["trv"] = (Attribute.GetCustomAttribute(typeof(Models.TopoModels.EULYNX.trv.AtcBalise), typeof(XmlTypeAttribute)) as XmlTypeAttribute)?.Namespace ?? throw new Exception("Missing namespace"),
            ["rsmCommon"] = (Attribute.GetCustomAttribute(typeof(Models.TopoModels.EULYNX.rsmCommon.NamedResource), typeof(XmlTypeAttribute)) as XmlTypeAttribute)?.Namespace ?? throw new Exception("Missing namespace"),
            ["rsmNE"] = (Attribute.GetCustomAttribute(typeof(Models.TopoModels.EULYNX.rsmNE.NetEntity), typeof(XmlTypeAttribute)) as XmlTypeAttribute)?.Namespace ?? throw new Exception("Missing namespace"),
            ["rsmTrack"] = (Attribute.GetCustomAttribute(typeof(Models.TopoModels.EULYNX.rsmTrack.Turnout), typeof(XmlTypeAttribute)) as XmlTypeAttribute)?.Namespace ?? throw new Exception("Missing namespace"),
            ["rsmSig"] = (Attribute.GetCustomAttribute(typeof(Models.TopoModels.EULYNX.rsmSig.Signal), typeof(XmlTypeAttribute)) as XmlTypeAttribute)?.Namespace ?? throw new Exception("Missing namespace"),
            ["eulynxLive"] = (Attribute.GetCustomAttribute(typeof(RastaAxleCountingSection), typeof(XmlTypeAttribute)) as XmlTypeAttribute)?.Namespace ?? throw new Exception("Missing namespace"),
            ["xsi"] = "http://www.w3.org/2001/XMLSchema-instance"
        };

        /// <summary>
        /// Serialize the interlocking data to a Eulynx DataPrep file
        /// </summary>
        /// <param name="path"></param>
        public void SerializeToFile(string path)
        {
            using var file = File.Create(path);
            SerializeToStream(file);
        }

        public void SerializeToStream(Stream stream)
        {
            XmlWriterSettings writerSettings = new XmlWriterSettings { Indent = true };

            XmlSerializer serializer = new XmlSerializer(typeof(EulynxDataPrepInterface), EulynxImport.CustomTypes);
            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();

            // set up the prefixes and namespaces
            foreach (string prefix in XsdNamespaces.Keys)
                xmlns.Add(prefix, XsdNamespaces[prefix]);

            using XmlWriter writer = XmlWriter.Create(stream, writerSettings);
            serializer.Serialize(writer, _dp, xmlns);
        }
    }
}
