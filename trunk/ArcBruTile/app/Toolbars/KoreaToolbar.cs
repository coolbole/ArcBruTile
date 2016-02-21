using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using BrutileArcGIS.MenuDefs;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using log4net;
using log4net.Config;

namespace BrutileArcGIS.Toolbars
{
    [Guid("759F6D96-4B55-4D4B-B21C-10DD685FCD1D")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("BrutileArcGIS.Toolbars.Chinatoolbar")]
    public class KoreaToolbar : BaseToolbar
    {
        private static readonly ILog Logger = LogManager.GetLogger("ArcBruTileSystemLogger");

        public KoreaToolbar()
        {
            XmlConfigurator.Configure(new FileInfo(Assembly.GetExecutingAssembly().Location + ".config"));

            try
            {
                AddItem(typeof(BruTileMenuDef));
                AddItem(typeof(DaumMenuDef));
                AddItem(typeof(NaverMenuDef));
                AddItem(typeof(VworldMenuDef));
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
                return "ArcBruTile - Korea";
            }
        }

        public override string Name
        {
            get
            {
                return "ArcBruTile Korea toolbar";
            }
        }

        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);
        }

        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommandBars.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommandBars.Unregister(regKey);
        }
    }
}
