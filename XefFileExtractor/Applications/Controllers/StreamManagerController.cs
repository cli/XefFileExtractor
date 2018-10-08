using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using XefFileExtractor.Applications.Services;
using XefFileExtractor.Applications.ViewModels;
using XefFileExtractor.Domain;

namespace XefFileExtractor.Applications.Controllers {
    [Export]
    internal class StreamManagerController {
        private readonly IShellService _shellService;
        private readonly SelectionService _selectionService;
        private readonly Lazy<StreamManagerViewModel> _streamManagerViewModel;
        private readonly ObservableCollection<KinectStream> _kinectStreams;
        private readonly DelegateCommand _openCommand;
        private readonly DelegateCommand _exitCommand;
        private KinectFile _kinectFile;

        [ImportingConstructor]
        public StreamManagerController(IShellService shellService, 
            SelectionService selectionService,
            Lazy<StreamManagerViewModel> streamManagerViewModel) {
            _shellService = shellService;
            _selectionService = selectionService;
            _streamManagerViewModel = streamManagerViewModel;
            _kinectStreams = new ObservableCollection<KinectStream>();
            _openCommand = new DelegateCommand(OpenFile);
            _exitCommand = new DelegateCommand(CloseProgram);
        }

        private StreamManagerViewModel ManagerViewModel => _streamManagerViewModel.Value;

        public void Initialize() {
            _selectionService.Initialize(_kinectStreams);

            ManagerViewModel.OpenCommand = _openCommand;
            ManagerViewModel.ExitCommand = _exitCommand;

            _shellService.StreamView = ManagerViewModel.View;
        }

        public void Run() { }

        public void Shutdown() { }

        private void OpenFile() {
            string filePath = Utils.OpenXefFile();
            UpdateStreamList(filePath);
        }

        private void CloseProgram() { }

        private void UpdateStreamList(string filePath) {
            _kinectFile = new KinectFile(filePath);

            _kinectStreams.Clear();

            foreach (var kinectFileStream in _kinectFile.Streams) {
                _kinectStreams.Add(kinectFileStream);
            }
        }
    }
}