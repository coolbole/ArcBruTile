using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Framework;

namespace BrutileArcGIS
{
    /// <summary>
    /// see blog.davebouwman.net[slash]default,date,2007-10-04.aspx
    /// Simple method to pass the ArcMap application window
    /// as windowhandle to a form at the show() or showdialog() methods:
    /// form.Show(new ArcMapWindow(application));
    /// </summary>
    public class ArcMapWindow : System.Windows.Forms.IWin32Window
    {
        private IApplication m_app;

        public ArcMapWindow(IApplication application)
        {
            m_app = application;
        }

        public System.IntPtr Handle
        {
            get { return new IntPtr(m_app.hWnd); }
        }
    }
}