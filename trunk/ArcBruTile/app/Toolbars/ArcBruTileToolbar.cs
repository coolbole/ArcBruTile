using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using BrutileArcGIS.MenuDefs;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using log4net;
using log4net.Config;

namespace BruTileArcGIS
{
    [Guid("059B9A69-335D-41EA-B511-4D3F985D27CC")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ArcBruTileToolbar")]
    public sealed class ArcBruTileToolbar : BaseToolbar
    {
        private static readonly ILog Logger = LogManager.GetLogger("ArcBruTileSystemLogger");

        [ComRegisterFunction]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            ArcGISCategoryRegistration(registerType);
        }

        [ComUnregisterFunction]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            ArcGISCategoryUnregistration(registerType);
        }

        private static void ArcGISCategoryRegistration(Type registerType)
        {
            var regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommandBars.Register(regKey);
        }

        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            var regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommandBars.Unregister(regKey);
        }

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
