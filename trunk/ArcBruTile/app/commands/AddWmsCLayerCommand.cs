using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using System.Configuration;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using System.Windows.Forms;
using BrutileArcGIS.Properties;
using BruTile.Web;
using System.Net;
using System.IO;
using BruTile;

namespace BruTileArcGIS
{
    [Guid("EF0A28AE-946B-46B8-BB5F-1BA116076E78")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("AddWmsCLayerCommand")]
    public sealed class AddWmsCLayerCommand : BaseCommand
    {
        #region private members
        private IMap map;
        private IApplication application;
        #endregion

        #region constructors
        /// <summary>
        /// Initialises a new BruTileCommand.
        /// </summary>
        public AddWmsCLayerCommand()
        {
            base.m_category = "BruTile";
            base.m_caption = "Add &WMS-C service...";
            base.m_message = "Add WMS-C Layer";
            base.m_toolTip = base.m_message;
            base.m_name = "AddWmsCLayer";
            base.m_bitmap = Resources.WMS_icon;
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
                Configuration config = ConfigurationHelper.GetConfig();
                IMxDocument mxdoc = (IMxDocument)application.Document;
                map = mxdoc.FocusMap;


                AddWmsCForm addWmsCForm = new AddWmsCForm();
                DialogResult result = addWmsCForm.ShowDialog(new BrutileArcGIS.ArcMapWindow(application));

                if (result == DialogResult.OK)
                {
                    ITileSource tileSource = addWmsCForm.SelectedTileSource;
                    
                    IConfig configWmsC = new ConfigWmsC(tileSource);
                    BruTileLayer brutileLayer = new BruTileLayer(application,configWmsC);
                    brutileLayer.Name=configWmsC.CreateTileSource().Schema.Name;
                    brutileLayer.Visible = true;
                    map.AddLayer((ILayer)brutileLayer);
                }
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
