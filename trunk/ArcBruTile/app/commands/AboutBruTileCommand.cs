using System;
using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using log4net;

namespace BruTileArcGIS
{
    /// <summary>
    /// Command voor het tonen van de laser tools about box.
    /// </summary>
    [Guid("BC096A33-F2C4-459E-8E7C-CBB9DA72BDB6")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("AboutBruTileCommand")]
    public sealed class AboutBruTileCommand : BaseCommand
    {
        private static readonly log4net.ILog logger = LogManager.GetLogger("ArcBruTileSystemLogger");

        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IApplication application;

        /// <summary>
        /// Initialiseert een nieuwe instantie van het command.
        /// </summary>
        public AboutBruTileCommand()
        {
            base.m_category = "BruTile";
            base.m_caption = "&About ArcBruTile...";
            base.m_message = "About BruTile...";
            base.m_toolTip = base.m_caption;
            base.m_name = "AboutBruTileCommand";
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            BruTileAboutBox bruTileAboutBox = new BruTileAboutBox();
            bruTileAboutBox.ShowDialog(new ArcMapWindow(application));
        }

        #endregion
    }
}
