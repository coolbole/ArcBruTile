using BrutileArcGIS.lib;
using Xunit;

namespace ArcBruTile.Tests
{
    public class GisCloudUtilTests
    {
        [Fact]
        public void GetProjectIdReturnsCorrectId()
        {
            // arrange
            var expectedPrefix = "http://editor.giscloud.com/map/";
            var url = "http://editor.giscloud.com/map/449121/raster-map";

            // act
            var id = GisCloudUtil.GetProjectIdFromUrl(url,expectedPrefix);

            // assert
            Assert.True(id=="449121");
        }

    }
}
