BruTile dll customizations for version 0.4 (changeset 42178)
http://brutile.codeplex.com/SourceControl/changeset/changes/42178
Author: BT
Date: 26/4/2010

Some customizations i've made to the BruTile dll:
1] method BruTile.PreDefined.SphericalMercatorWorldSchema.SphericalMercatorWorldSchema()
Old code:
            this.Srs = "EPSG:3785";
New code:
            this.Srs = "EPSG:900913";
Reason: EPSG 3785 doesn't seem to work in ArcMap?!?

2] method BruTile.Cache.FileCache.GetFileName(TileIndex index)
Old code:
        private string GetFileName(TileIndex index)
New code: 
        public string GetFileName(TileIndex index)
Reason: We need to know the name of the file to save it to disk

3] method BruTile.Cache.FileCache.Exists(TileIndex index)
Old code:
        private string Exists(TileIndex index)
New code: 
        public string Exists(TileIndex index)
Reason: We need to know if the file already exists on disk

4] property BruTile.Web.WebTileProvider.requestBuilder 
Old code:
        IRequest requestBuilder;
New code:
        public IRequest requestBuilder;
Reason: We need to know the url's of the tiles to fetch tiles in a threadpool

5] Method BruTile.Web.RequestHelper.FetchImage(Uri uri, string userAgent, string referer, bool keepAlive)
Added:
            webRequest.AllowAutoRedirect=true;
Reason: The SpatialCloud service uses redirection to retrieve an image

NB: I didn't use the Silverlight version of BruTile, because of errors with System.dll. Maybe this
is because the ArcBruTile extension or ArcMap already uses 







