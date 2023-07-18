using System.Text;

namespace PlanningToolkit.Test;

public class EulynxImportTest
{
    [Fact]
    public void TestImportFromString()
    {
        var xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<generic:eulynxDataPrepInterface xmlns:sig=\"http://dataprep.eulynx.eu/schema/Signalling/1.1\" xmlns:db=\"http://dataprep.eulynx.eu/schema/DB/1.1\" xmlns:sncf=\"http://dataprep.eulynx.eu/schema/SNCF/1.1\" xmlns:nr=\"http://dataprep.eulynx.eu/schema/NR/1.1\" xmlns:prorail=\"http://dataprep.eulynx.eu/schema/ProRail/1.1\" xmlns:rfi=\"http://dataprep.eulynx.eu/schema/RFI/1.1\" xmlns:trv=\"http://dataprep.eulynx.eu/schema/TRV/1.1\" xmlns:rsmCommon=\"http://www.railsystemmodel.org/schemas/Common/202206\" xmlns:rsmNE=\"http://www.railsystemmodel.org/schemas/NetEntity/202206\" xmlns:rsmTrack=\"http://www.railsystemmodel.org/schemas/Track/202206\" xmlns:rsmSig=\"http://www.railsystemmodel.org/schemas/Signalling/202206\" xmlns:eulynxLive=\"http://dataprep.eulynx.eu/schema/EulynxLive/1.1\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:generic=\"http://dataprep.eulynx.eu/schema/Generic/1.1\">\n  <rsmCommon:id xsi:nil=\"true\" />\n  <generic:hasTimeStamp xsi:nil=\"true\" />\n  <generic:toolName xsi:nil=\"true\" />\n  <generic:toolVersion xsi:nil=\"true\" />\n</generic:eulynxDataPrepInterface>";
        var import = EulynxImport.DeserializeFromString(xml);
        Assert.NotNull(import.dp);
    }


    [Fact]
    public void TestImportFromStream()
    {
        var xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<generic:eulynxDataPrepInterface xmlns:sig=\"http://dataprep.eulynx.eu/schema/Signalling/1.1\" xmlns:db=\"http://dataprep.eulynx.eu/schema/DB/1.1\" xmlns:sncf=\"http://dataprep.eulynx.eu/schema/SNCF/1.1\" xmlns:nr=\"http://dataprep.eulynx.eu/schema/NR/1.1\" xmlns:prorail=\"http://dataprep.eulynx.eu/schema/ProRail/1.1\" xmlns:rfi=\"http://dataprep.eulynx.eu/schema/RFI/1.1\" xmlns:trv=\"http://dataprep.eulynx.eu/schema/TRV/1.1\" xmlns:rsmCommon=\"http://www.railsystemmodel.org/schemas/Common/202206\" xmlns:rsmNE=\"http://www.railsystemmodel.org/schemas/NetEntity/202206\" xmlns:rsmTrack=\"http://www.railsystemmodel.org/schemas/Track/202206\" xmlns:rsmSig=\"http://www.railsystemmodel.org/schemas/Signalling/202206\" xmlns:eulynxLive=\"http://dataprep.eulynx.eu/schema/EulynxLive/1.1\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:generic=\"http://dataprep.eulynx.eu/schema/Generic/1.1\">\n  <rsmCommon:id xsi:nil=\"true\" />\n  <generic:hasTimeStamp xsi:nil=\"true\" />\n  <generic:toolName xsi:nil=\"true\" />\n  <generic:toolVersion xsi:nil=\"true\" />\n</generic:eulynxDataPrepInterface>";
        using var memoryStream = new MemoryStream();

        var sw = new StreamWriter(memoryStream, Encoding.UTF8);
        sw.Write(xml);
        sw.Flush();

        memoryStream.Seek(0, SeekOrigin.Begin);

        var import = EulynxImport.DeserializeFromStream(memoryStream);
        Assert.NotNull(import.dp);
    }
}
