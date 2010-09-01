using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;

namespace BruTileArcGIS
{
    [Guid("CAB70B8E-A0CD-40F4-9295-E56D85A2A2B2")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("BruTilePropertyPageUserControl")]
    public partial class BruTilePropertyPageUserControl : UserControl, IComPropertyPage
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
            LayerPropertyPages.Register(regKey);
            SxLayerPropertyPages.Register(regKey);
            GMxLayerPropertyPages.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            LayerPropertyPages.Unregister(regKey);
            SxLayerPropertyPages.Unregister(regKey);
            GMxLayerPropertyPages.Unregister(regKey);
        }

        #endregion
        #endregion

        private bool isPageDirty = false;
        private string m_pageTitle;
        private int m_priority;
        private IComPropertyPageSite m_pageSite = null;
        private BruTileLayer bruTileLayer = null;
        private IActiveView m_activeView = null;

        public BruTilePropertyPageUserControl()
        {
            InitializeComponent();

            m_pageTitle = "Tiles";

        }

        /// <summary>
        /// Helper function to set dirty flag whenever changes are made to the page
        /// </summary>
        private void SetPageDirty(bool dirty)
        {
            if (isPageDirty != dirty)
            {
                isPageDirty = dirty;
                if (m_pageSite != null)
                    m_pageSite.PageChanged();
            }
        }


        #region IComPropertyPage Members

        public int Activate()
        {
            return this.Handle.ToInt32();
        }

        public bool Applies(ESRI.ArcGIS.esriSystem.ISet objects)
        {
            if (objects == null || objects.Count == 0)
                return false;

            bool isEditable = false;
            objects.Reset();
            object testObject;
            while ((testObject = objects.Next()) != null)
            {
                if (testObject is BruTileLayer)
                {
                    isEditable = true;
                    break;
                }
            }

            return isEditable;

        }

        public void Apply()
        {
        }

        public void Cancel()
        {
            if (isPageDirty)
            {
                //Reset UI or any temporary changes made to layer
                //radioButtonShow.Checked = m_targetLayer.Visible;

                SetPageDirty(false);
            }
        }

        public void Deactivate()
        {
            bruTileLayer = null;
            m_activeView = null;
            this.Dispose(true);
        }

        public string HelpFile
        {
            get { return String.Empty; }
        }

        public bool IsPageDirty
        {

            get { return isPageDirty;}
        }

        public IComPropertyPageSite PageSite
        {
            set { m_pageSite = value; }
        }

        public int Priority
        {
            get
            {
                return m_priority;
            }
            set
            {
                m_priority = value;
            }
        }

        public void SetObjects(ESRI.ArcGIS.esriSystem.ISet objects)
        {
            if (objects == null || objects.Count == 0)
                return;

            m_activeView = null;
            bruTileLayer = null;

            objects.Reset();
            object testObject;
            while ((testObject = objects.Next()) != null)
            {
                if (testObject is BruTileLayer)
                    bruTileLayer = testObject as BruTileLayer;
                else if (testObject is IActiveView)
                    m_activeView = testObject as IActiveView;
                //else
                //{
                //IApplication app = testObject as IApplication  //Use if needed
                //}
            }

        }

        public string Title
        {
            get
            {
                return m_pageTitle;
            }
            set
            {
                m_pageTitle = value;
            }
        }

        public int get_HelpContextID(int controlID)
        {
            return 0;
        }

        #endregion

        private void rdbOsm_CheckedChanged(object sender, EventArgs e)
        {
            SetPageDirty(true);
        }

        private void rdbBing_CheckedChanged(object sender, EventArgs e)
        {
            SetPageDirty(true);
        }

        private void rdbEsri_CheckedChanged(object sender, EventArgs e)
        {
            SetPageDirty(true);

        }

        private void rdbTMS_CheckedChanged(object sender, EventArgs e)
        {
            SetPageDirty(true);

        }

        private void rdbGeodan_CheckedChanged(object sender, EventArgs e)
        {
            SetPageDirty(true);
        }

        private void BruTilePropertyPageUserControl_Load(object sender, EventArgs e)
        {

        }

    }
}
