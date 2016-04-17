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
            var url = "http://editor.giscloud.com/map/449121/raster-map";

            // act
            var id = GisCloudUtil.GetProjectIdFromUrl(url);

            // assert
            Assert.True(id=="449121");
        }

        [Fact]
        public void UrlIsValid()
        {
            // arrange
            var url = "http://editor.giscloud.com/map/449121/raster-map";

            // act
            var isValid = GisCloudUtil.UrlIsValid(url);

            // assert
            Assert.True(isValid);
        }


    }
}
