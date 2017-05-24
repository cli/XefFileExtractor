using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using XefFileExtractor.Applications.Services;
using XefFileExtractor.Applications.Views;

namespace XefFileExtractor.Applications.ViewModels {
    [Export]
    public class ControlViewModel : ViewModel<IControlView> {
        private ICommand _extractCommand;
        private string _outputPath;

        [ImportingConstructor]
        public ControlViewModel(IControlView view, IShellService shellService) : base(view) {
            ShellService = shellService;
        }

        public IShellService ShellService { get; }

        public ICommand ExtractCommand {
            get => _extractCommand;
            set => SetProperty(ref _extractCommand, value);
        }

        public string OutputDirectory {
            get { return _outputPath; }
            set { SetProperty(ref _outputPath, value); }
        }

        public int GetProgress() {
            return ViewCore.GetProgress();
        }

        public void SetProgress(int progress) {
            ViewCore.SetProgress(progress);
        }
    }
}