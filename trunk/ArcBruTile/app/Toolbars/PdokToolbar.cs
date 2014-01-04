using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using BrutileArcGIS.MenuDefs;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using log4net;
using log4net.Config;

namespace BrutileArcGIS
{
    [Guid("BB5D789D-3DC3-4963-A420-FF805E3C5237")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PdokToolbar")]
    public sealed class PdokToolbar:BaseToolbar
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


        public PdokToolbar()
        {
            XmlConfigurator.Configure(new FileInfo(Assembly.GetExecutingAssembly().Location + ".config"));

            try
            {
                AddItem(typeof(PdokMenuDef));

                //var config = ConfigurationHelper.GetConfig();


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
                return "ArcBruTile - PDOK";
            }
        }

        public override string Name
        {
            get
            {
                return "PDOK toolbar";
            }
        }

    }
}
