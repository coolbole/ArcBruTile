using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrutileArcGIS.lib;
using BrutileArcGIS.Lib;
using BruTile.Web;
using Xunit;

namespace ArcBruTile.Tests
{
    public class GisCloudUtilTests
    {
        [Fact]
        public void CreateBingTileSourceReturnsTileSource()
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
