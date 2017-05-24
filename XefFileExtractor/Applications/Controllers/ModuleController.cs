using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using XefFileExtractor.Applications.Services;
using XefFileExtractor.Applications.ViewModels;

namespace XefFileExtractor.Applications.Controllers {
    [Export(typeof(IModuleController)), Export]
    internal class ModuleController : IModuleController {
        private readonly Lazy<ShellService> _shellService;
        private readonly Lazy<StreamManagerController> _streamManagerController;
        private readonly Lazy<ControlController> _controlController;
        private readonly Lazy<ShellViewModel> _shellViewModel;

        [ImportingConstructor]
        public ModuleController(Lazy<ShellService> shellService, Lazy<ShellViewModel> shellViewModel,
            Lazy<StreamManagerController> streamManagerController,
            Lazy<ControlController> controlController) {
            _shellService = shellService;
            _streamManagerController = streamManagerController;
            _controlController = controlController;
            _shellViewModel = shellViewModel;
        }

        private StreamManagerController StreamManagerController => _streamManagerController.Value;

        private ControlController ControlController => _controlController.Value;

        private ShellService ShellService => _shellService.Value;

        private ShellViewModel ShellViewModel => _shellViewModel.Value;

        public void Initialize() {
            // Load settings and intialize other controllers
            StreamManagerController.Initialize();
            ControlController.Initialize();
        }

        public void Run() {
            ShellViewModel.Show();
        }

        public void Shutdown() {
            // Shut down controllers and save settings
        }
    }
}
