using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace BruTileArcGIS
{
    /// <summary>
    /// Command used for showing dialog with predefined services.
    /// </summary>
    [Guid("BC096A33-F2C4-459E-8E7C-CBB9DA72BDB7")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("AddServicesCommand")]
    public sealed class AddServicesCommand : BaseCommand
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

        private IApplication application;

        /// <summary>
        /// Initialiseert een nieuwe instantie van het command.
        /// </summary>
        public AddServicesCommand()
        {
            base.m_category = "BruTile";
            base.m_caption = "&Add service...";
            base.m_message = "Add service...";
            base.m_toolTip = base.m_caption;
            base.m_name = "ServicesCommand";
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

                IMxDocument mxdoc = (IMxDocument)application.Document;
                IMap map = mxdoc.FocusMap;

                AddServicesForm addServicesForm = new AddServicesForm();

                DialogResult result = addServicesForm.ShowDialog(new BrutileArcGIS.ArcMapWindow(application));
                //DialogResult result = addServicesForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    TileMap selectedService = addServicesForm.SelectedService;
                    TileMapService provider = addServicesForm.SelectedTileMapService;

                    // Fix the service labs.metacarta.com bug: it doubles the version :-(
                    selectedService.Href = selectedService.Href.Replace(@"1.0.0/1.0.0", @"1.0.0").Trim();


                    /**string capabilitiesHref = selectedTileMapService.Href.Replace(@"1.0.0/1.0.0", @"1.0.0").Trim();
                    string serviceURL = selectedService.Href.Trim();
                    if (serviceURL.EndsWith(@"/"))
                    {
                        serviceURL = serviceURL.Remove(serviceURL.Length - 1);
                    }
                    if (!serviceURL.ToLower().Equals(capabilitiesHref.Substring(0, capabilitiesHref.IndexOf("1.0.0")).ToLower()))
                    {
                        if (true)
                        {
                            selectedService.Href = serviceURL + @"/" + capabilitiesHref.Substring(capabilitiesHref.IndexOf("1.0.0"));
                        }
                    }*/

                    // Normally the layer is a TMS
                    EnumBruTileLayer layerType=EnumBruTileLayer.TMS;

                    // If the type is inverted TMS we have to do something special
                    if (provider.Type == "InvertedTMS")
                    {
                        layerType = EnumBruTileLayer.InvertedTMS;
                    }

                    BruTileLayer brutileLayer = new BruTileLayer(application, layerType, selectedService.Href);
                    brutileLayer.Name = selectedService.Title;
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
    }
}
