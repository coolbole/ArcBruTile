using System;
using ESRI.ArcGIS.Framework;

namespace BrutileArcGIS.Lib
{
    /// <summary>
    /// see blog.davebouwman.net[slash]default,date,2007-10-04.aspx
    /// Simple method to pass the ArcMap application window
    /// as windowhandle to a form at the show() or showdialog() methods:
    /// form.Show(new ArcMapWindow(application));
    /// </summary>
    public class ArcMapWindow : System.Windows.Forms.IWin32Window
    {
        private readonly IApplication _app;

        public ArcMapWindow(IApplication application)
        {
            _app = application;
        }

        public IntPtr Handle
        {
            get { return new IntPtr(_app.hWnd); }
        }
    }
}