using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using Laan.AddIns.Ssms.VsExtension.Commands;

using Microsoft.VisualStudio.Shell;

using Task = System.Threading.Tasks.Task;

namespace Laan.AddIns.Ssms.VsExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class VsExtensionPackage : AsyncPackage
    {
        /// <summary>
        /// Laan.AddIns.Ssms.VsExtensionPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "74f80447-ad39-44f8-aa0a-a62f2b5fcc8e";

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var actions = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(BaseAction).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .ToList();

//            foreach (var action in actions)
//            {
//                var item = Activator.CreateInstance(action) as BaseAction;
//                Debug.WriteLine(
//@"
//<Button guid=""guidVsExtensionPackageCmdSet"" id=""{0}Id"" priority=""0x0100"" type=""Button"">
//    <Parent guid=""guidVsExtensionPackageCmdSet"" id=""MyMenuGroup"" />
//    <Icon guid=""guidImages"" id=""bmpPic1"" />
//    <Strings>
//        <ButtonText>{0}</ButtonText>
//    </Strings>
//</Button>
//", new object[] { action.Name.Replace("Action", "") });
//            }

//            foreach (var action in actions)
//            {
//                var item = Activator.CreateInstance(action) as BaseAction;
//                Debug.WriteLine(@"<IDSymbol name=""{0}Id"" value=""{1}"" />", action.Name, item.CommandId);
//            }

            foreach (var action in actions)
                await BaseAction.CreateAsync(action, this);

            //await BaseAction.CreateAsync<SqlFormatterAction>(this);
            //await BaseAction.CreateAsync<SqlInsertTemplateAction>(this);
            //await BaseAction.CreateAsync<SqlTemplateOptionAction>(this);
            //await BaseAction.CreateAsync<MoveLineUpAction>(this);
        }
    }
}
