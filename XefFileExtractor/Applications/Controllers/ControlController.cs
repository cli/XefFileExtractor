using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Waf.Applications;
using XefFileExtractor.Applications.Services;
using XefFileExtractor.Applications.ViewModels;
using XefFileExtractor.Domain;

namespace XefFileExtractor.Applications.Controllers {
    [Export]
    internal class ControlController {
        private readonly IShellService _shellService;
        private readonly ISelectionService _selectionService;
        private readonly Lazy<ControlViewModel> _controlViewModel;
        private readonly DelegateCommand _extractCommand;
        private readonly BackgroundWorker _worker;
        private int _currentProgress;

        [ImportingConstructor]
        public ControlController(IShellService shellService,
            ISelectionService selectionService,
            Lazy<ControlViewModel> controlViewModel) {
            _shellService = shellService;
            _selectionService = selectionService;
            _controlViewModel = controlViewModel;
            _extractCommand = new DelegateCommand(o => _worker.RunWorkerAsync(), o => !_worker.IsBusy);

            _worker = new BackgroundWorker();
            _worker.DoWork += Extract;
            _worker.WorkerReportsProgress = true;
        }

        private ControlViewModel ControlViewModel => _controlViewModel.Value;

        public DelegateCommand ExtractCommand => _extractCommand;

        public int CurrentProgress {
            get { return _currentProgress; }
            private set {
                if (_currentProgress != value) {
                    _currentProgress = value;
                    ControlViewModel.SetProgress(value);
                }
            }
        }

        public void Initialize() {
            _shellService.ControlView = ControlViewModel.View;

            ControlViewModel.ExtractCommand = _extractCommand;
            ControlViewModel.OutputDirectory = Directory.GetCurrentDirectory();
        }

        public void Run() { }

        public void Shutdown() { }

        private void Extract(object sender, DoWorkEventArgs e) {
            var streams = _selectionService.SelectedStreams.Select(x => x);

            foreach (var stream in streams) {
                CurrentProgress = 0;
                stream.ProgressUpdated += KProgressChanged;
                // TODO: Should I use the view model this way?
                stream.Extract(ControlViewModel.OutputDirectory);
            }
        }

        void KProgressChanged(object sender, EventArgs e) {
            KinectFileProgressChangedEventArgs kargs = (KinectFileProgressChangedEventArgs) e;
            if (kargs.Progress > CurrentProgress) {
                CurrentProgress = kargs.Progress;
            }
        }
    }
}