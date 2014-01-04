using System;
using System.IO;
using System.Reflection;
using BrutileArcGIS.Lib;
using BrutileArcGIS.MenuDefs;
using ESRI.ArcGIS.ADF.BaseClasses;
using log4net;
using log4net.Config;

namespace BruTileArcGIS
{
    public sealed class ArcBruTileToolbar : BaseToolbar
    {
        private static readonly ILog Logger = LogManager.GetLogger("ArcBruTileSystemLogger");

        public ArcBruTileToolbar()
        {
            XmlConfigurator.Configure(new FileInfo(Assembly.GetExecutingAssembly().Location + ".config"));

            try
            {
                Logger.Info("Startup ArcBruTile");
                AddItem(typeof(BruTileMenuDef));
                var config = ConfigurationHelper.GetConfig();

                //Status sectie
                BeginGroup();

                BeginGroup();
                if (Convert.ToBoolean(config.AppSettings.Settings["useOSM"].Value))
                {
                    AddItem(typeof(OsmMenuDef));
                }
                if (Convert.ToBoolean(config.AppSettings.Settings["useBing"].Value))
                {
                    AddItem(typeof(BingMenuDef));
                }
                if (Convert.ToBoolean(config.AppSettings.Settings["useStamen"].Value))
                {
                    AddItem(typeof(StamenMenuDef));
                }
                if (Convert.ToBoolean(config.AppSettings.Settings["useMapBox"].Value))
                {
                    AddItem(typeof(MapBoxMenuDef));
                }
                if (Convert.ToBoolean(config.AppSettings.Settings["useCloudMade"].Value))
                {
                    AddItem(typeof(CloudMadeMenuDef));
                }
                if (Convert.ToBoolean(config.AppSettings.Settings["useMapQuest"].Value))
                {
                    AddItem(typeof(MapQuestMenuDef));
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        public override string Caption
        {
            get
            {
                return "ArcBruTile";
            }
        }

        public override string Name
        {
            get
            {
                return "ArcBruTile toolbar";
            }
        }
    }
}
