using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Runtime.InteropServices;
using System;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Diagnostics;


namespace BruTileArcGIS
{
    [RunInstaller(true)]
    public partial class ArcBruTileInstaller : Installer
    {
        private static readonly log4net.ILog logger = LogManager.GetLogger("ArcBruTileSystemLogger");

        public ArcBruTileInstaller()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Voer de installatie uit.
        /// </summary>
        /// <param name="stateSaver">An <see cref="T:System.Collections.IDictionary"/> used to save information needed to perform a commit, rollback, or uninstall operation.</param>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="stateSaver"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Exception">
        /// An exception occurred in the <see cref="E:System.Configuration.Install.Installer.BeforeInstall"/> event handler of one of the installers in the collection.
        /// -or-
        /// An exception occurred in the <see cref="E:System.Configuration.Install.Installer.AfterInstall"/> event handler of one of the installers in the collection.
        /// </exception>
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.OnAfterInstall(stateSaver);

            string esriRegAsmFilename = Path.Combine(
                          Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                          "ArcGIS\\bin\\ESRIRegAsm.exe");
            Process esriRegAsm = new Process();
            esriRegAsm.StartInfo.FileName = esriRegAsmFilename;
            string cmd = string.Format("\"{0}\" /p:Desktop", base.GetType().Assembly.Location);
            esriRegAsm.StartInfo.Arguments = cmd;
            logger.Debug("Register for ArcGIS 10: " + cmd);

            esriRegAsm.Start();
            logger.Debug("Register for ArcGIS 10 finished.");
            
        }

        /// <summary>
        /// Verwijdert de installatie.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Collections.IDictionary"/> that contains the state of the computer after the installation was complete.</param>
        /// <exception cref="T:System.ArgumentException">
        /// The saved-state <see cref="T:System.Collections.IDictionary"/> might have been corrupted.
        /// </exception>
        /// <exception cref="T:System.Configuration.Install.InstallException">
        /// An exception occurred while uninstalling. This exception is ignored and the uninstall continues. However, the application might not be fully uninstalled after the uninstallation completes.
        /// </exception>
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);

            XmlConfigurator.Configure(new FileInfo(base.GetType().Assembly.Location + ".config"));

            logger.Debug("Uninstall ArcBruTile");
            // Try to clean up stuff
            try
            {
                string cacheFolder = CacheSettings.GetCacheFolder();
                logger.Debug("Trying to delete tile folder: " + cacheFolder );
                System.IO.Directory.Delete(cacheFolder,true);
                logger.Debug("Tile directory is deleted");
            }
            catch (Exception ex)
            {
                logger.Debug("Delete folder failed, error: " + ex.ToString());
            }

           string esriRegAsmFilename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                "ArcGIS\\bin\\ESRIRegAsm.exe");
            Process esriRegAsm = new Process();
            esriRegAsm.StartInfo.FileName = esriRegAsmFilename;
            string cmd=string.Format("\"{0}\" /p:Desktop /u", base.GetType().Assembly.Location);
            esriRegAsm.StartInfo.Arguments = cmd;
            logger.Debug("Unregister for ArcGIS 10: " + cmd);
            esriRegAsm.Start();
            logger.Debug("Unregister for ArcGIS 10 finished.");
        }

    }
}
