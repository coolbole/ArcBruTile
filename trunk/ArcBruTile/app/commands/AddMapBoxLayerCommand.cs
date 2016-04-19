using System.Net.Http;
using System.Runtime.InteropServices;
using BrutileArcGIS.lib;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddMapBoxSatelliteLayerCommand")]
    public sealed class AddMapBoxSatelliteLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxSatelliteLayerCommand()
            : base("BruTile", "&Satellite", "Add Satellite Layer", "MapBox Satellite", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/Satellite.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }

    [ProgId("AddMapBoxStreetsLayerCommand")]
    public sealed class AddMapBoxStreetsLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxStreetsLayerCommand() : base("BruTile", "&Streets", "Add Streets Layer", "MapBox Streets", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/Streets.xml", EnumBruTileLayer.InvertedTMS, "mapbox")
        {
        }
    }

    [ProgId("AddMapBoxLightCommand")]
    public sealed class AddMapBoxLightLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxLightLayerCommand() : base("BruTile", "&Light", "Add Light Layer", "MapBox Light", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/Light.xml", EnumBruTileLayer.InvertedTMS, "mapbox")
        {
        }
    }

    [ProgId("AddMapBoxDarkCommand")]
    public sealed class AddMapBoxDarkLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxDarkLayerCommand() : base("BruTile", "&Dark", "Add Dark Layer", "MapBox Dark", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/Dark.xml", EnumBruTileLayer.InvertedTMS, "mapbox")
        {
        }
    }

    [ProgId("AddMapBoxStreetsSatelliteCommand")]
    public sealed class AddMapBoxStreetsSatelliteLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxStreetsSatelliteLayerCommand() : base("BruTile", "&StreetsSatellite", "Add StreetsSatellite Layer", "MapBox StreetsSatellite", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/Streets-satellite.xml", EnumBruTileLayer.InvertedTMS, "mapbox")
        {
        }
    }

    [ProgId("AddMapBoxWheatpasteCommand")]
    public sealed class AddMapBoxWheatpasteLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxWheatpasteLayerCommand() : base("BruTile", "&Wheatpaste", "Add Wheatpaste Layer", "MapBox Wheatpaste", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/wheatpaste.xml", EnumBruTileLayer.InvertedTMS, "mapbox")
        {
        }
    }

    [ProgId("AddMapBoxStreetsBasicCommand")]
    public sealed class AddMapBoxStreetsBasicLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxStreetsBasicLayerCommand() : base("BruTile", "&Streets - basic", "Add Streets - basic Layer", "MapBox Streets - Basic", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/streets-basic.xml", EnumBruTileLayer.InvertedTMS, "mapbox")
        {
        }
    }

    [ProgId("AddMapBoxComicCommand")]
    public sealed class AddMapBoxComicLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxComicLayerCommand() : base("BruTile", "&Comic", "Add Comic", "MapBox Comic", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/comic.xml", EnumBruTileLayer.InvertedTMS, "mapbox")
        {
        }
    }

    [ProgId("AddMapBoxOutdoorsCommand")]
    public sealed class AddMapBoxOutdoorsLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxOutdoorsLayerCommand() : base("BruTile", "&Outdoors", "Add Outdoors", "MapBox Outdoors", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/outdoors.xml", EnumBruTileLayer.InvertedTMS, "mapbox")
        {
        }
    }

    [ProgId("AddMapBoxRunBikeHikeCommand")]
    public sealed class AddMapBoxRunBikeHikeLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxRunBikeHikeLayerCommand() : base("BruTile", "&Run - bike - hike", "Add Run - bike - hike", "MapBox Run - bike - hike", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/run-bike-hike.xml", EnumBruTileLayer.InvertedTMS, "mapbox")
        {
        }
    }

    [ProgId("AddMapBoxPencilCommand")]
    public sealed class AddMapBoxPencilLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxPencilLayerCommand() : base("BruTile", "&Pencil", "Add pencil", "MapBox Pencil", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/pencil.xml", EnumBruTileLayer.InvertedTMS, "mapbox")
        {
        }
    }

    [ProgId("AddMapBoxPiratesCommand")]
    public sealed class AddMapBoxPiratesLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxPiratesLayerCommand() : base("BruTile", "&Pirates", "Add pirates", "MapBox Pirates", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/pirates.xml", EnumBruTileLayer.InvertedTMS, "mapbox")
        {
        }
    }

    [ProgId("AddMapBoxEmeraldCommand")]
    public sealed class AddMapBoxEmeraldLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxEmeraldLayerCommand() : base("BruTile", "&Emerald", "Add emerald", "MapBox Emerald", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/emerald.xml", EnumBruTileLayer.InvertedTMS, "mapbox")
        {
        }
    }

    [ProgId("AddMapBoxHighContrastCommand")]
    public sealed class AddMapBoxHighContrastLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxHighContrastLayerCommand() : base("BruTile", "&High-contrast", "Add high-contrast", "MapBox High-Contrast", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/high-contrast.xml", EnumBruTileLayer.InvertedTMS, "mapbox")
        {
        }
    }

}
