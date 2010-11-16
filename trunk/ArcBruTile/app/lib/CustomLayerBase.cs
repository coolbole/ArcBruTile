
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace BruTileArcGIS
{
    /// <summary>
    /// Base layer for custom layers
    /// </summary>
    public partial class CustomLayerBase : Control, ILayer, ILegendInfo, IGeoDataset, IPersistVariant, ILayerGeneralProperties, ILayerExtensions, IDisposable
    {
        /// <summary>
        /// The log4net logger
        /// </summary>
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Class members
        /// <summary>
        /// Keep the layer's extent. Returned by the ILayer::Extent property
        /// </summary>
        /// <remarks>The extent should be spatial-referenced to the DateFrame's spatial reference.
        /// </remarks>
        protected IEnvelope extent = null;

        /// <summary>
        /// Store the layer's underlying data spatial reference. Returned by IGeoDataset::SpatialReference.
        /// </summary>
        /// <remarks>This spatial reference should not be reprojected. In your inheriting 
        /// class you will need to have another parameter that will keep the DataFrame's spatial reference
        /// that would use to reproject the geometries and the extent of the layer.</remarks>
        protected ISpatialReference spatialRef = null;

        /// <summary>
        /// The spatial reference of the map display
        /// </summary>
        protected ISpatialReference mapSpatialRef;

        /// <summary>
        /// Layer's name. Returned by ILayer::Name property
        /// </summary>
        protected string name;

        /// <summary>
        /// Flag which determines whether the layers is visible. Returned by ILayer::Visible
        /// </summary>
        /// <remarks>You should use this member in your inherited class in the Draw method.</remarks>
        protected bool visible;

        /// <summary>
        /// determines whether the layers is cached
        /// </summary>
        protected bool isCached;

        /// <summary>
        /// Flag thich determine whether the layer is valid (connected to its data source, has valid information etc.).
        /// Returned by ILAyer::Valid.
        /// </summary>
        /// <remarks>You can use this flag to determine for example whether the layer can be available or not.</remarks>
        protected bool valid = true;

        /// <summary>
        /// Keep the maximum scale value at which the layer will display
        /// </summary>
        protected double maximumScale;

        /// <summary>
        /// Keep the minimum scale value at which the layer will display
        /// </summary>
        protected double minimumScale;

        /// <summary>
        /// determines whether the layers is supposed to show its MapTips
        /// </summary>
        protected bool showTips;

        /// <summary>
        /// the layer dirty flag which determine whether its display list need to be recreate
        /// </summary>
        protected bool isImmediateDirty = false;

        /// <summary>
        /// the layer dirty flag which determine whether its display list need to be recreate
        /// </summary>
        protected bool isCompiledDirty = false;

        /// <summary>
        /// The rate in which the DynamicDisplay recompile its display lists
        /// </summary>
        protected int recompileRate = -1;

        /// <summary>
        /// The layer's UID
        /// </summary>
        protected UID uid;

        /// <summary>
        /// An arraylist to store the layer's extensions.
        /// </summary>
        protected ArrayList extensions = null;

        ///// <summary>
        ///// The Dynamic Display System class
        ///// </summary>
        //protected DDSystem ddSystem = null;

        /// <summary>
        /// The data table holding the data read from the RSS
        /// </summary>
        protected DataTable table = null;

        #endregion

        #region class constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomLayerBase()
        {
            name = string.Empty;
            visible = true;
            isCached = false;
            maximumScale = 0;
            minimumScale = 0;

            table = new DataTable("RECORDS");
            table.Columns.Add("ID", typeof(long));

            extent = new EnvelopeClass();
            extent.SetEmpty();

            uid = new UIDClass();

            extensions = new ArrayList();

            //this.ddSystem = DDSystem.GetInstance();

            //make sure that the control got created and that it has a valid handle
            this.CreateHandle();
            this.CreateControl();
        }
        #endregion

        #region IGeoDataset Members

        /// <summary>
        ///The layers geodataset extent which is a union of the extents of all
        /// the items of the layer
        /// </summary>
        /// <remarks>In your inheriting class, consider the following code to calculate the layer's extent:
        /// <code>
        /// public override IEnvelope Extent
        ///{
        ///  get
        ///  {
        ///    m_extent  = GetLayerExtent();
        ///    if (null == m_extent )
        ///      return null;
        ///
        ///    IEnvelope env = ((IClone)m_extent ).Clone() as IEnvelope;
        ///    if(null != m_layerSpatialRef)
        ///       env.Project(m_layerSpatialRef);
        /// 
        ///    return env;
        ///  }
        ///}
        ///    private IEnvelope GetLayerExtent()
        ///{
        ///  if (null == base.m_spRef)
        ///  {
        ///    base.m_spRef = CreateGeographicSpatialReference();
        ///  }
        ///
        ///  IEnvelope env = new EnvelopeClass();
        ///  env.SpatialReference = base.m_spRef;
        ///  IPoint point = new PointClass();
        ///  point.SpatialReference = m_spRef;
        ///  foreach (DataRow r in m_table.Rows)
        ///  {
        ///    point.Y = Convert.ToDouble(r["Y"]);
        ///    point.X = Convert.ToDouble(r["X"]);
        ///
        ///    env.Union(point.Envelope);
        ///  }
        /// 
        ///  return env;
        ///} 
        /// </code>
        /// </remarks>    
        virtual public IEnvelope Extent
        {
            get
            {
                return extent;
            }
        }

        /// <summary>
        /// The spatial reference of the underlying data.
        /// </summary>
        /// <remarks>The property must return the underlying data spatial reference and
        /// must not reporoject it into the layer's spatial reference </remarks>

        virtual public ISpatialReference SpatialReference
        {
            get
            {
                return spatialRef;
            }
        }

        #endregion

        #region IPersistVariant Members

        /// <summary>
        /// The ID of the object.
        /// </summary>
        virtual public UID ID
        {
            get
            {
                // TODO:  Add clsCustomLayer.ID getter implementation
                return null;
            }
        }

        /// <summary>
        /// Loads the object properties from the stream.
        /// </summary>
        /// <param name="Stream"></param>
        /// <remarks>The Load method must read the data from the stream in the same order the data was 
        /// written to the stream in the Save method. 
        /// Streams are sequential; you mut ensure that your data is saved and loaded in the correct order, 
        /// so that the correct data is written to the correct member.
        /// </remarks>
        virtual public void Load(IVariantStream Stream)
        {
            extensions = (ArrayList)Stream.Read();
        }

        /// <summary>
        /// Saves the object properties to the stream.
        /// </summary>
        /// <param name="Stream"></param>
        virtual public void Save(IVariantStream Stream)
        {
            Stream.Write(extensions);
        }

        #endregion

        #region ILayer Members

        #region Properties

        /// <summary>
        /// Indicates if the layer shows map tips.
        /// </summary>
        /// <remarks>Indicates whether or not map tips are shown for the layer. 
        /// If set to True, then map tips will be shown for the layer. 
        /// You can determine the text that will be shown via TipText. 
        ///</remarks>
        virtual public bool ShowTips
        {
            get
            {
                return showTips;
            }
            set
            {
                showTips = value;
            }
        }

        /// <summary>
        /// The default area of interest for the layer. Returns the spatial-referenced extent of the layer.
        /// </summary>
        virtual public IEnvelope AreaOfInterest
        {
            get
            {
                return extent;
            }
        }

        /// <summary>
        /// Indicates if the layer is currently visible.
        /// </summary>
        virtual new public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }


        /// <summary>
        /// Indicates if the layer needs its own display cache.
        /// </summary>
        /// <remarks>This property indicates whether or not the layer requires its own display cache. 
        /// If this property is True, then the Map will use a separate display cache for the layer so 
        /// that it can be refreshed indpendently of other layers.</remarks>
        virtual public bool Cached
        {
            get
            {
                return isCached;
            }
            set
            {
                isCached = value;
            }
        }

        /// <summary>
        /// Minimum scale (representative fraction) at which the layer will display.
        /// </summary>
        /// <remarks>Specifies the minimum scale at which the layer will be displayed. 
        /// This means that if you zoom out beyond this scale, the layer will not display. 
        /// For example, specify 1000 to have the layer not display when zoomed out beyond 1:1000.</remarks>
        virtual public double MinimumScale
        {
            get
            {
                return minimumScale;
            }
            set
            {
                minimumScale = value;
            }
        }

        /// <summary>
        /// Indicates if the layer is currently valid.
        /// </summary>
        /// <remarks>The valid property indicates if the layer is currently valid.
        /// Layers that reference feature classes are valid when they hold a reference to a valid feature class.
        /// The property does not however validate the integrity of the feature classes reference to the database.
        /// Therefore, in rare situations if a datasource is removed after a layer is initialized, 
        /// the layer will report itself as valid but query attempts to the data source will error due to the lack 
        /// of underlying data.</remarks>
        virtual public bool Valid
        {
            get
            {
                return valid;
            }
        }

        /// <summary>
        /// The Layer name.
        /// </summary>
        virtual new public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Maximum scale (representative fraction) at which the layer will display.
        /// </summary>
        /// <remarks>Specifies the maximum scale at which the layer will be displayed. 
        /// This means that if you zoom in beyond this scale, the layer will not display. 
        /// For example, specify 500 to have the layer not display when zoomed in beyond 1:500.</remarks>
        virtual public double MaximumScale
        {
            get
            {
                return maximumScale;
            }
            set
            {
                maximumScale = value;
            }
        }

        /// <summary>
        /// Supported draw phases.
        /// </summary>
        /// <remarks>Indicates the draw phases supported by the layer (esriDPGeography, esriDPAnnotation, 
        /// esriDPSelection, or any combination of the three). 
        /// The supported draw phases are defined by esriDrawPhase. 
        /// When multiple draw phases are supported, the sum of the constants is used. 
        /// For example, if SupportedDrawPhases = 3 then the layer supports drawing in the geography and annotation phases.</remarks>
        public int SupportedDrawPhases
        {
            get
            {
                return (int)esriDrawPhase.esriDPGeography;
            }
        }

        /// <summary>
        /// Spatial reference for the layer.
        /// </summary>
        ///<remarks>This property is only used for map display, setting this property does not 
        ///change the spatial reference of the layer's underlying data. 
        ///The ArcGIS framework uses this property to pass the spatial reference from the map 
        ///to the layer in order to support on-the-fly projection.</remarks> 
        ISpatialReference ESRI.ArcGIS.Carto.ILayer.SpatialReference
        {
            set
            {
                mapSpatialRef = value;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Map tip text at the specified location. 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Tolerance"></param>
        /// <returns>The text string that gets displayed as a map tip if ShowTips = true.</returns>
        virtual public string get_TipText(double X, double Y, double Tolerance)
        {
            return null;
        }

        /// <summary>
        /// Draws the layer to the specified display for the given draw phase.
        /// </summary>
        /// <param name="drawPhase"></param>
        /// <param name="Display"></param>
        /// <param name="trackCancel"></param>
        /// <remarks>This method draws the layer to the Display for the specified DrawPhase. 
        /// Use the TrackCancel object to allow the drawing of the layer to be interrupted by the user.
        /// In order to implement you inheriting class, you must override this method</remarks>
        public virtual void Draw(esriDrawPhase drawPhase, IDisplay Display, ITrackCancel trackCancel)
        {
            return;
        }

        #endregion

        #endregion

        #region ILayerGeneralProperties Members

        /// <summary>
        /// Last maximum scale setting used by layer.
        /// </summary>
        virtual public double LastMaximumScale
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Last minimum scale setting used by layer.
        /// </summary>
        virtual public double LastMinimumScale
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Description for the layer.
        /// </summary>
        virtual public string LayerDescription
        {
            get
            {
                return null;
            }
            set
            {

            }
        }
        #endregion

        #region ILayerExtensions Members

        /// <summary>
        /// Removes the specified extension.
        /// </summary>
        /// <param name="Index"></param>
        public virtual void RemoveExtension(int Index)
        {
            if (Index < 0 || Index > extensions.Count - 1)
                return;

            extensions.RemoveAt(Index);
        }

        /// <summary>
        /// Number of extensions.
        /// </summary>
        public virtual int ExtensionCount
        {
            get
            {
                return extensions.Count;
            }
        }

        /// <summary>
        /// Adds a new extension.
        /// </summary>
        /// <param name="ext"></param>
        public virtual void AddExtension(object ext)
        {
            if (null == ext)
                return;

            extensions.Add(ext);
        }

        /// <summary>
        /// The extension at the specified index. 
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public virtual object get_Extension(int Index)
        {
            if (Index < 0 || Index > extensions.Count - 1)
                return null;

            return extensions[Index];
        }

        #endregion

        #region Data Structure basic functionality

        /// <summary>
        /// Create a new item (does not add it to the layer)
        /// </summary>
        /// <returns></returns>
        virtual public DataRow NewItem()
        {
            return table.NewRow();
        }

        /// <summary>
        /// Add an item to the layer
        /// </summary>
        /// <param name="row"></param>
        virtual public void AddItem(DataRow row)
        {
            table.Rows.Add(row);
        }

        /// <summary>
        /// Add an item to the Layer
        /// </summary>
        /// <param name="values"></param>
        virtual public void AddItem(object[] values)
        {
            table.Rows.Add(values);
        }

        /// <summary>
        /// Query for items in the layer
        /// </summary>
        /// <param name="queryFilter">WHERE clause</param>
        /// <returns></returns>
        virtual public DataRow[] Select(string queryFilter)
        {
            return table.Select(queryFilter);
        }

        /// <summary>
        /// Remove all items in the layer
        /// </summary>
        virtual public void Clear()
        {
            table.Rows.Clear();
        }

        /// <summary>
        /// Remove the item from the layer
        /// </summary>
        /// <param name="row">The row to remove</param>
        virtual public void RemoveItem(DataRow row)
        {
            table.Rows.Remove(row);
        }

        /// <summary>
        /// indexer. Pass an index and return a record
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataRow this[int index]
        {
            get
            {
                return table.Rows[index];
            }
        }

        /// <summary>
        /// return the number of geometries in the layer
        /// </summary>
        public int NumOfRecords
        {
            get
            {
                return table.Rows.Count;
            }
        }
        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Allow users to directly enumerate through the layer's records
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return table.Rows.GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose the layer
        /// </summary>
        public new void Dispose()
        {
            extent = null;
            spatialRef = null;

            base.Dispose();
        }

        #endregion


        #region ILegendInfo Members

        /// <summary>
        /// Number of legend groups contained by the object.
        /// </summary>
        public virtual int LegendGroupCount
        {
            get
            {
                return 0;
            }

        }

        /// <summary>
        /// Optional. Defines legend formatting for layer rendered with this object.
        /// </summary>
        public virtual ILegendItem LegendItem
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Indicates if symbols are graduated.
        /// </summary>
        public virtual bool SymbolsAreGraduated
        {
            get
            {
                return false;
            }
            set
            {
                return;
            }
        }

        /// <summary>
        /// Legend group at the specified index.
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public virtual ILegendGroup get_LegendGroup(int Index)
        {
            return null;
        }

        #endregion

    }
}