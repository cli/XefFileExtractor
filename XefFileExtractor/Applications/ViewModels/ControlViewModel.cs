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
        private string _statusText;

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
            get => _outputPath;
            set => SetProperty(ref _outputPath, value);
        }

        public string StatusText {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public int GetProgress() {
            return ViewCore.GetProgress();
        }

        public void SetProgress(int progress) {
            ViewCore.SetProgress(progress);
        }
    }
}