// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.CartoUI;

namespace BruTileArcGIS
{
    static class Util
    {

        /// <summary>
        /// Sets the property page.
        /// </summary>
        /// <param name="layer">The layer.</param>
        public static void SetBruTilePropertyPage(IApplication application, BruTileLayer layer)
        {

            IBasicDocument basicDocument = application.Document as IBasicDocument;

            ISet propertyObjects = new SetClass();
            propertyObjects.Add(basicDocument.ActiveView);
            propertyObjects.Add(layer); //or check ContextItem is a layer?
            propertyObjects.Add(application); //optional?

            IComPropertySheet propertySheet = new ComPropertySheetClass();
            propertySheet.Title = "BruTile Property sheet";
            propertySheet.HideHelpButton = true;
            propertySheet.ClearCategoryIDs();
            propertySheet.AddCategoryID(new UIDClass()); //a dummy empty UID
            propertySheet.AddPage(new BruTilePropertyPageUserControl()); //my custom page
            
            FeatureLayerDisplayPropertyPageClass displayPropertyPage = new FeatureLayerDisplayPropertyPageClass();
            propertySheet.AddCategoryID(new UIDClass()); //a dummy empty UID
            propertySheet.AddPage(displayPropertyPage);

            
            //Pass in layer, active view and the application
            propertyObjects.Add(layer);
            propertyObjects.Reset();
            //propertySheet.EditProperties(propertyObjects, application.hWnd);
        }

        public static string GetAppDir()
        {
            return System.IO.Path.GetDirectoryName(
              System.Reflection.Assembly.GetEntryAssembly().GetModules()[0].FullyQualifiedName);
        }

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2f) + Math.Pow(y1 - y2, 2f));
        }

        public static string AppName
        {
            get { return System.Reflection.Assembly.GetEntryAssembly().GetName().Name.ToString(); }
        }

        public static string DefaultCacheDir
        {
            get { return "c:\\TileCache"; }
        }

        //public static Extent ToExtent(Rect rect)
        //{
        //    return new Extent(rect.Left, rect.Top, rect.Right, rect.Bottom);
        //}

    }
}
