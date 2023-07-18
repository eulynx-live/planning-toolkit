using System.Xml;
using System.Xml.Serialization;
using Models.TopoModels.EULYNX.generic;
using PlanningToolkit.Dataprep;

namespace PlanningToolkit
{
    public class EulynxImport
    {
        public EulynxDataPrepInterface dp;

        public EulynxImport(EulynxDataPrepInterface eulynxDataPrepInterface)
        {
            dp = eulynxDataPrepInterface;
        }

        public static EulynxImport DeserializeFromString(string xml) {
            var serializer = CreateSerializer();
            using var reader = new StringReader(xml);
            return new EulynxImport((EulynxDataPrepInterface)serializer.Deserialize(reader));
        }

        public static EulynxImport DeserializeFromStream(Stream xml) {
            var serializer = CreateSerializer();
            return new EulynxImport((EulynxDataPrepInterface)serializer.Deserialize(xml));
        }

        /// <summary>
        /// Deserialize the interlocking data from Eulynx Dataprep file
        /// </summary>
        /// <param name="path"></param>
        public static EulynxImport DeserializeFromFile(string path)
        {
            var serializer = CreateSerializer();

            using var file = File.OpenRead(path);
            using var reader = XmlReader.Create(file);

            return new EulynxImport((EulynxDataPrepInterface)serializer.Deserialize(reader));
        }

        private static XmlSerializer CreateSerializer() {
            return new XmlSerializer(typeof(EulynxDataPrepInterface),
                new Type[] { typeof(RastaSignal), typeof(RastaTurnout), typeof(RastaAxleCountingSection), typeof(GerEtcsBaliseGroup) });
        }
    }
}
