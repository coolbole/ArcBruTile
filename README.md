**Project Description**  

ArcBruTile displays maps from [Air Quality Index](https://aqicn.org), [Baidu](http://map.baidu.com/), [Bing](http://www.bing.com/maps/), [CartoDB](https://cartodb.com/), [Daum](http://map.daum.net/), [GIS Cloud](http://www.giscloud.com), [Humanitarian OpenStreetMap (HOT)](https://hotosm.org/), [Klokan Technologies](http://www.klokantech.com/), [Mapbox](http://www.mapbox.com), [MapQues](http://www.mapquest.com/)t, [Nationaal Georegister](http://nationaalgeoregister.nl/) (PDOK), [National Library of Scotland](http://www.nls.uk/) (NLS), [Nokia HERE](https://maps.here.com), [Naver](http://map.naver.com/), [OpenStreetMap](http://www.openstreetmap.org/), [OpenRailwayMap](http://www.openrailwaymap.org/), [OpenWeatherMap](http://openweathermap.org/), [OpenSeaMap](http://www.openseamap.org/), [OSM2Vectortiles](http://osm2vectortiles.org/maps/), [Stamen](http://www.stamen.com), [Strava](http://www.strava.com/), [Thunderforest](http://www.thunderforest.com/), [Tianditu](http://www.tianditu.cn), [VWorld](http://www.vworld.kr) and others in ArcGIS Desktop 10.0/10.1/10.2/10.3/10.4.

Latest release: [ArcBruTile 0.7 Download](https://github.com/ArcBruTile/ArcBruTile/releases/tag/0.7)

![Baidu](https://dl.dropboxusercontent.com/u/9984329/ArcBruTile/doc/baidu.png)

<span style="font-size: 13.3333px;">**Slack**</span>

Get in contact with other ArcBruTile users/developers, go to [http://arcbrutile.slack.com](http://arcbrutile.slack.com)

**N****ews**

30-05-2016: Added Nokia HERE maps: Streets, Satellite, Hybrid, Terrrain, Traffic, Transit

29-05-2016: Project moved from CodePlex to GitHub

17-05-2016: Added [Air Quality Index](https://aqicn.org) services: aqi, pm25, o3, no2, so2, co, asean-pm10

27-04-2016: [Arcgis 와 국내지도의 연동 - ArcBruTile 0.7버전](http://www.biz-gis.com/index.php?document_srl=188708)

24-04-2016: ArcBruTile 0.7 released.

. add toolbars for China (Baidu, Tianditu) and Korea (Daum, Naver, VWorld)  
. add CartoDB services (Darkmatter, Positron)  
. add services from OpenWeatherMap, Thunderforest, OpenRailWayMap and National Library of Scotland (NLS)

21-04-2016: Add Historical Maps of Great Britain from [National Library of Scotland](http://www.nls.uk/) (NLS). Use the 'Add TMS Service' option and choose NLS.

15-02-2016: Add [OpenRailwayMap](http://www.openrailwaymap.org/): Rails, Speed and Signals.  

14-02-2016: Add [Daum](http://map.daum.net/) maps (Korea): Streets, Hybrid and Satellite. Scheduled for next release ArcBruTile 0.7.

14-2-2016: Added [CartoDB](https://cartodb.com/) maps: dark, light, dark no labels, light no labels, dark only labels. Use the 'Add TMS Service' option and choose CartoDB.

14-2-2016: Added [Thunderforest](http://www.thunderforest.com/) maps:  Landscape, OpenCycleMap, Outdoors, Spinal, Transport, Transport-dark. Use the 'Add TMS Service' option and choose Thunderforest.

13-2-2016: Added [Baidu](http://map.baidu.com/) maps (China): Normal, Streets, Satellite. Scheduled for next release ArcBruTile 0.7.

12-2-2016: Added [Naver](http://map.naver.com/) maps (Korea): Streets, Satellite, Hybrid and Cadastral. Scheduled for next release ArcBruTile 0.7.

11-2-2016: Added [OpenWeatherMap](http://openweathermap.org/) (Clouds, Clouds Classic, Snow, Precipitation, Rain, Wind, Temperature, Precipitation Forecast, Pressure, Clouds Forecast) and OpenSeaMap.  Use the 'Add TMS Service' option and choose OpenWeatherMap or OpenSeaMap

**Supported maps**

See [supported_maps.md](supported_maps.md)

**Functionality**

*   all projections;
*   client side caching;
*   ArcGIS 10.0, 10.1, 10.2, 10.3, 10.4;
*   Printing; 
*   MapTiler support (see [ArcBruTile and MapTiler](wikipage?title=ArcBruTile%20and%20MapTiler) for details).

ArcBruTile is based on the generic tiling library [BruTile](http://brutile.codeplex.com). For other viewers based on BruTile library  see the [Mapsui](http://mapsui.codeplex.com) project.

 **Why ArcBruTile is developed:**

Because ArcGIS, one of the most popular desktop GIS program around, lacks support for non-Esri tile services, especially the increasingly popular [OpenStreetMap](http://www.openstreetmap.org) maps. And because we're bored searching for the always slow or lost or broken disk/service with actual map reference data :-)  
ArcBruTile also solves the projections problem: all tiles are transformed on the fly to the desired projection if needed. ArcBruTile makes it very easy to combine data with different projections. 

**Getting started for developers**

Prerequisites: Visual Studio + ArcGIS Desktop 10.X is installed

*   git clone https://github.com/ArcBruTile/ArcBruTile.git
*   cd ArcBruTile\trunk\ArcBruTile
*   Open ArcBruTile.sln in Visual Studio 2015
*   Rebuild solution
*   cd app\bin
*   Right click on ArcBruTile -> Open with -> Select Esri Registration Assembly Utility -> Register with ArcGIS Desktop
*   Open project properties of project ArcBruTile -> Debug -> Start action -> Start external program -> Select C:\Program Files (x86)\ArcGIS\Desktop10.3\bin\ArcMap.exe
*   Press F5 and ArcMap will start in debug mode
 
**Testimonials:**

*   "Great tool, saves me having to set up a local tile server. " - [mrsleepy](http://arcbrutile.codeplex.com/WorkItem/View.aspx?WorkItemId=5226)
*   "surprisingly zippy" - [Mike Olkin](http://twitter.com/MikeOlkin/status/8206992508)
*   "It works pretty well" - [Node Dangles weblog](http://nodedangles.wordpress.com/2010/09/09/arcbrutile)
*   "ArcBruTile is a great bolt on for Desktop" - [ArcGIS Desktop 10 Tips and Tricks](http://gis.stackexchange.com/questions/1987/arcgis-desktop-10-tips-and-tricks)
*   "What I like so much about ArcBruTile is that it is connecting OSS to the ESRI/Microsoft world." - [Paul den Dulk](http://pauldendulk.com/2010/01/arcbrutile-released.html)
*   这个工具还可以吧。一起学习讨论！- [Xqiunshi](http://xqiushi.com/archives/78249.html)
*   "Oggi mi sento Brutile!" - [Milo's Tec](http://milotec.tumblr.com/post/2346470732/oggi-mi-sento-brutile) 
*   "Super great add-in for ArcGis" - [Winbladh](http://arcbrutile.codeplex.com/wikipage?action=Edit&title=Home&referringTitle=Home)
*   "Handig!" - [Koen Rutten](https://twitter.com/hetblijftgissen/status/212515896550367232)
*   "I wish I knew this sooner" - [Hugo Ahlenius](https://twitter.com/nordpil/status/191936286754488321)
*   方便指数：*****  
    操作难度：*  
    时间耗费：* - [http://site.douban.com/129653/widget/notes/5349640/note/194403530/](http://site.douban.com/129653/widget/notes/5349640/note/194403530/)
*   "<span id="ReviewListText0">es bueno para mi trabajo contar con esyas"  - [joni_dtx](https://www.codeplex.com/site/users/view/joni_dtx)</span>
*   <span id="ReviewListText0">The good tools Thanks a lot! -</span> [PersianPolaris](https://www.codeplex.com/site/users/view/PersianPolaris)

**Future development:**  

Want to add your maps or new functionality to ArcMap? [Contact us](https://www.codeplex.com/site/users/contact/bertt?OriginalUrl=http://www.codeplex.com/site/users/view/bertt)! 

**References:**

*   [Haiti/2010 Earthquake CrisisCommons Wiki - CrisisCommons.org](http://wiki.crisiscommons.org/index.php?title=Haiti/2010_Earthquake)
*   [Public Safety: Accessing OpenStreetMap data within ArcGIS to support the Haiti Earthquake Response - ESRI](http://blogs.esri.com/Dev/blogs/publicsafety/archive/2010/01/20/Accessing-OpenStreetMap-data-within-ArcGIS-to-support-the-Haiti-Earthquake-Response.aspx)
*   [Basemaps In ArcGIS Desktop With ArcBruTile - Slashgeo](http://industry.slashgeo.org/article.pl?sid=10/01/24/1655222)
*   [Can custom map tiles be consumed via ArcObjects within my ArcGIS 10 Desktop AddIn? - gis.stackexchange.com](http://gis.stackexchange.com/questions/2217/can-custom-map-tiles-be-consumed-via-arcobjects-within-my-arcgis-10-desktop-addin)
*   [http://wiki.openstreetmap.org/wiki/ArcBruTile](http://wiki.openstreetmap.org/wiki/ArcBruTile)

**Powered by:**  

[![ArcGIS](http://i3.codeplex.com/download?ProjectName=arcbrutile&DownloadId=101931 "ArcGIS")](http://www.esri.com)[![BruTile](http://i3.codeplex.com/download?ProjectName=arcbrutile&DownloadId=101932 "BruTile")](http://brutile.codeplex.com)[![OpenStreetMap](http://i3.codeplex.com/download?ProjectName=arcbrutile&DownloadId=101933 "OpenStreetMap")](http://www.openstreetmap.org)[![Bing Maps](http://i3.codeplex.com/download?ProjectName=arcbrutile&DownloadId=101934 "Bing Maps")](http://maps.live.com)  ![](https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS4bE-xZhP6Efv3ixHu_Q-05i9HgIaRG0BI3NDtYyEAOkQ9jXFl6wT3CbU)![](http://images.spatiallyadjusted.com/GISCloud-Logo.gif)![](https://d21buns5ku92am.cloudfront.net/27712/images/90475-logos_full_cartodb_light-medium-1365655273.png)![](https://upload.wikimedia.org/wikipedia/commons/e/ef/Daum_communication_logo.png)![](http://www.bigtrends.com/wp-content/uploads/2015/10/baidu-bidu-logo-earnings-2015-stock-market-options-trading-technical-analysis-chart-etf-china-chinese-tech-stocks-active-investor.jpg)  

**Statistics:**  

![](http://www.myworldmaps.net/map.ashx/6fad71fc-58da-44a4-8f3d-4eb5d05e57a7/big)

[![web
counter](http://c.statcounter.com/10943089/0/32c9a51b/0/)](http://statcounter.com/p10943089/summary/?guest=1 "web counter")
