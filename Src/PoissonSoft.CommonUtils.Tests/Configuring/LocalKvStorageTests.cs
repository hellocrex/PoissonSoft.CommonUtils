using System.IO;
using PoissonSoft.CommonUtils.Configuring;
using Xunit;

namespace PoissonSoft.CommonUtils.Tests.Configuring
{
    public class LocalKvStorageTests
    {
        [Fact]
        public void GetDataTest()
        {
            var xml =
                "<?xml version='1.0' encoding='utf-8'?>" +
                "<LocalKvStorage>" +
                "  <Item key='test1'>test value 1</Item>" +
                "  <Item key='test2'>test value 2</Item>" +
                "</LocalKvStorage>";
            const string fileName = "example.xml";
            File.WriteAllText(fileName, xml);

            var stg = new LocalKvStorage(fileName);
            Assert.Equal("test value 1", stg["test1"]);
            Assert.Equal("test value 2", stg["test2"]);
        }
    }
}
