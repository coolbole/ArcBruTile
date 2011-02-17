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
using BrutileArcGIS;
using log4net.Config;
using System.Reflection;
using System.Configuration;
using ESRI.ArcGIS.Geometry;
using System.Diagnostics;
using System.ComponentModel;
using BrutileArcGIS.Properties;
using BruTile;
using System.Collections.Generic;

namespace BruTileArcGIS
{
    /// <summary>
    /// Command voor het tonen van de laser tools about box.
    /// </summary>
    [Guid("855F9DFE-E397-4b33-9E54-EED54EEF150B")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PreCacheBruTileCommand")]
    public sealed class PreCacheBruTileCommand : BaseCommand
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

        private IApplication m_application;

        /// <summary>
        /// Initialiseert een nieuwe instantie van het command.
        /// </summary>
        public PreCacheBruTileCommand()
        {
            base.m_category = "BruTile";
            base.m_caption = "&PreCache BruTile...";
            base.m_message = "PreCache BruTile...";
            base.m_toolTip = base.m_caption;
            base.m_name = "PreCacheBruTileCommand";
            base.m_bitmap = Resources.download;
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

            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }


        private int GetNumberOfTiles(ITileSchema schema, IEnvelope env)
        {
            int result = 0;

            //start the actual precache process
            for (int i = 0; i < schema.Resolutions.Count; i++)
            {
                //IList<TileInfo> tiles = schema.GetTilesInView(env, i);
                //result += tiles.Count;
            }
            return result;
        }



        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                //check if this is an OSM layer and we are at the specified minimum level (see config)
                XmlConfigurator.Configure(new FileInfo(Assembly.GetExecutingAssembly().Location + ".config"));
                Configuration config = ConfigurationHelper.GetConfig();
                string levelStr = config.AppSettings.Settings["precacheStartLevel"].Value;
                levelStr = "14"; // hardcoded to prevent abuse
                int precacheMinimumLevel;
                if (!int.TryParse(levelStr, out precacheMinimumLevel))
                {
                    MessageBox.Show("There is no minimum precache level defined in the config file. Please enter a valid levelnumber in the config file.");
                    return;
                }

                //are we using OSM?
                EnumBruTileLayer enumBruTileLayer = EnumBruTileLayer.OSM;
                IMxDocument mxdoc = (IMxDocument)m_application.Document;
                IMap map = mxdoc.FocusMap;
                bool layerIsOsmAndVisible = false;

                BruTileLayer bruTileLayer=this.GetFirstOsmBruTileLayer(map);
                if (bruTileLayer != null)
                {
                    if (bruTileLayer.Visible)
                    {
                        layerIsOsmAndVisible = (bruTileLayer.CurrentLevel >= precacheMinimumLevel);

                        if (!layerIsOsmAndVisible)
                        {
                            MessageBox.Show(String.Format("Error: The current OpenStreetMap layer level ({0}) is smaller than the minimum precache level ({1}).", bruTileLayer.CurrentLevel, precacheMinimumLevel));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: The OpenStreetMap layer is not visible.");
                    }
                }
                else
                {
                    MessageBox.Show("Error: There is no OpenStreetMap layer loaded.");
                }

                if (layerIsOsmAndVisible)
                {
                    // Show warning...
                    IConfig config1 = ConfigHelper.GetConfig(enumBruTileLayer);
                    ITileSource tileSource = config1.CreateTileSource();
                    ITileSchema schema = tileSource.Schema;
                    int number=this.GetNumberOfTiles(schema, ((IActiveView)map).Extent); 



                    //show form
                    FormPreCache formPreCache = new FormPreCache();
                    if (formPreCache.ShowDialog().Equals(DialogResult.OK))
                    {
                        int[] precacheLevels = formPreCache.preCacheLevels;
                        string precacheAreaName = formPreCache.preCacheAreaName;
                        formPreCache.Close();

                        //get user folder
                        string cacheDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ArcBruTile\\PreCache");
                        cacheDir = System.IO.Path.Combine(cacheDir, precacheAreaName);

                        //start precache procedure here
                        Precache precacher = new Precache(new IntPtr(m_application.hWnd), mxdoc.ActiveView, enumBruTileLayer, map.SpatialReference, cacheDir);
                        precacher.RunPrecacher();
                    }
                    else
                    {
                        formPreCache.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        #endregion


        private BruTileLayer GetFirstOsmBruTileLayer(IMap map)
        {
            BruTileLayer result = null;
            for (int i = 0; i < map.LayerCount; i++)
            {
                ILayer layer = map.get_Layer(i);
                if (layer.GetType() == typeof(BruTileLayer))
                {
                    BruTileLayer btLayer = layer as BruTileLayer;
                    if (btLayer.EnumBruTileLayer == EnumBruTileLayer.OSM)
                    {
                        result = btLayer;
                        break;
                    }
                }
            }
            return result;
        }
    }
}
