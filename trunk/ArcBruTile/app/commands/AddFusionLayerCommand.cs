using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BrutileArcGIS.Properties;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;

namespace BruTileArcGIS
{
    [Guid("A1CEBE35-2F73-4130-9D48-1C5EE07ADAC8")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("AddFusionLayerCommand")]
    public sealed class AddFusionLayerCommand:BaseCommand
    {
                #region private members
        private IApplication application;
        #endregion

        #region constructors
        /// <summary>
        /// Initialises a new BruTileCommand.
        /// </summary>
        public AddFusionLayerCommand()
        {
            base.m_category = "BruTile";
            base.m_caption = "&Fusion";
            base.m_message = "Add Fusion Layer";
            base.m_toolTip = base.m_message;
            base.m_name = "AddFusionLayer";
            base.m_bitmap = Resources.kml;
        }
        #endregion

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
        /// Gets a value indicating whether this <see cref="BerekenenDHMCommand"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public override bool Enabled
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                IMxDocument mxdoc = (IMxDocument)application.Document;
                IMap map = mxdoc.FocusMap;

                KMLLayerClass kmlLayer = new KMLLayerClass();
                kmlLayer.Name = "test";
                kmlLayer.URL = "http://www.bgs.ac.uk/feeds/MhSeismology.kml";
                //kmlLayer.URL = "http://www.google.com/fusiontables/api/query?sql=SELECT%20*%20FROM%20297903%20WHERE%20ST_INTERSECTS(geometry,RECTANGLE(LATLNG(35.7108378353,-97.6025390625),LATLNG(35.7108378353,-97.6025390625)))%20LIMIT%20250";
                kmlLayer.RefreshRate = 100;
                kmlLayer.ShowTips = true;

                kmlLayer.GetKMLData();
                ILayer layer = kmlLayer as ILayer;

                kmlLayer.HotlinkField = "sip";
                kmlLayer.HotlinkType = esriHyperlinkType.esriHyperlinkTypeDocument;
                
                map.AddLayer(layer);
                ((IActiveView)map).Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

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
        #endregion

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
    }
}
