using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using System.Configuration;
using BruTileArcGIS;
using ESRI.ArcGIS.Carto;

namespace BrutileArcGIS.commands
{
    /// <summary>
    /// Summary description for AddGeoserverLayerCommand.
    /// </summary>
    [Guid("b1b6ff8e-0947-485f-9edd-ed38134814eb")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("AddGeoserverLayerCommand")]
    public sealed class AddGeoserverLayerCommand : BaseCommand
    {
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
        private IMap map;
        private IApplication application;
        public AddGeoserverLayerCommand()
        {
            base.m_category = "BruTile";
            base.m_caption = "&Geoserver";
            base.m_message = "Add Geoserver Layer";
            base.m_toolTip = base.m_message;
            base.m_name = "AddGeoserverLayer";

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
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
            try
            {
                Configuration config = ConfigurationHelper.GetConfig();
                IMxDocument mxdoc = (IMxDocument)application.Document;
                map = mxdoc.FocusMap;
                
                BruTileLayer brutileLayer = new BruTileLayer(application, EnumBruTileLayer.GeoserverWms);
                brutileLayer.Name = "Geoserver";
                brutileLayer.Visible = true;

                //IEnumLayer lyrs;
                //((IMapLayers)map).AddLayers((ILayer)brutileLayer);
                map.AddLayer((ILayer)brutileLayer);
                Util.SetBruTilePropertyPage(application, brutileLayer);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

        }

        #endregion
    }
}