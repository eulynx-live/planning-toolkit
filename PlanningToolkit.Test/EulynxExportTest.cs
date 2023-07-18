using Models.TopoModels.EULYNX.generic;

namespace PlanningToolkit.Test;

public class EulynxExportTest
{

    [Fact]
    public void SerializerCreatesXmlDocument()
    {
        var eulynx = new EulynxDataPrepInterface();

        var export = new EulynxExport(eulynx);

        using (var stream = new MemoryStream())
        {
            export.SerializeToStream(stream);

            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                var actual = reader.ReadToEnd();
                var expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<generic:eulynxDataPrepInterface xmlns:sig=\"http://dataprep.eulynx.eu/schema/Signalling/1.1\" xmlns:db=\"http://dataprep.eulynx.eu/schema/DB/1.1\" xmlns:sncf=\"http://dataprep.eulynx.eu/schema/SNCF/1.1\" xmlns:nr=\"http://dataprep.eulynx.eu/schema/NR/1.1\" xmlns:prorail=\"http://dataprep.eulynx.eu/schema/ProRail/1.1\" xmlns:rfi=\"http://dataprep.eulynx.eu/schema/RFI/1.1\" xmlns:trv=\"http://dataprep.eulynx.eu/schema/TRV/1.1\" xmlns:rsmCommon=\"http://www.railsystemmodel.org/schemas/Common/202206\" xmlns:rsmNE=\"http://www.railsystemmodel.org/schemas/NetEntity/202206\" xmlns:rsmTrack=\"http://www.railsystemmodel.org/schemas/Track/202206\" xmlns:rsmSig=\"http://www.railsystemmodel.org/schemas/Signalling/202206\" xmlns:eulynxLive=\"http://dataprep.eulynx.eu/schema/EulynxLive/1.1\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:generic=\"http://dataprep.eulynx.eu/schema/Generic/1.1\">\n  <rsmCommon:id xsi:nil=\"true\" />\n  <generic:hasTimeStamp xsi:nil=\"true\" />\n  <generic:toolName xsi:nil=\"true\" />\n  <generic:toolVersion xsi:nil=\"true\" />\n</generic:eulynxDataPrepInterface>";
                Assert.Equal(expected, actual);
            }
        }
    }
}
