using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Waf;
using System.Waf.Applications;
using System.Windows;

namespace XefFileExtractor {
    public partial class App : Application {
        private AggregateCatalog catalog;
        private CompositionContainer container;
        private IEnumerable<IModuleController> moduleControllers;


        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            catalog = new AggregateCatalog();
            // Add the WpfApplicationFramework assembly to the catalog
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(WafConfiguration).Assembly));
            //catalog.Catalogs.Add(new AssemblyCatalog(typeof(System.Waf.Foundation.Model).Assembly));
            //catalog.Catalogs.Add(new AssemblyCatalog(typeof(ShellViewModel).Assembly));
            // Add the WafApplication assembly to the catalog
            //catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(App).Assembly));

            container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(container);
            container.Compose(batch);

            // Initialize and run all module controllers
            moduleControllers = container.GetExportedValues<IModuleController>();
            foreach (var moduleController in moduleControllers) {
                moduleController.Initialize();
            }
            foreach (var moduleController in moduleControllers) {
                moduleController.Run();
            }
        }

        protected override void OnExit(ExitEventArgs e) {
            // Shutdown the module controllers in reverse order
            foreach (var moduleController in moduleControllers.Reverse()) {
                moduleController.Shutdown();
            }
            container.Dispose();
            catalog.Dispose();

            base.OnExit(e);
        }
    }
}
