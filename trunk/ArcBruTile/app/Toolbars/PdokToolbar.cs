using System;
using System.IO;
using System.Reflection;
using BrutileArcGIS.MenuDefs;
using ESRI.ArcGIS.ADF.BaseClasses;
using log4net;
using log4net.Config;

namespace BrutileArcGIS
{
    public sealed class PdokToolbar:BaseToolbar
    {
        private static readonly ILog Logger = LogManager.GetLogger("ArcBruTileSystemLogger");
   
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
