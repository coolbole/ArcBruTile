using System.ComponentModel;
using System.Configuration.Install;
using System.Runtime.InteropServices;


namespace BruTileArcGIS
{
    [RunInstaller(true)]
    public partial class ArcBruTileInstaller : Installer
    {
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
            base.Install(stateSaver);
            RegistrationServices regSrv = new RegistrationServices();
            regSrv.RegisterAssembly(base.GetType().Assembly,
              AssemblyRegistrationFlags.SetCodeBase);
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
            base.Uninstall(savedState);
            RegistrationServices regSrv = new RegistrationServices();
            regSrv.UnregisterAssembly(base.GetType().Assembly);
        }

    }
}
