using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using XefFileExtractor.Applications.Services;
using XefFileExtractor.Applications.Views;
using XefFileExtractor.Domain;

namespace XefFileExtractor.Applications.ViewModels {
    [Export]
    public class StreamManagerViewModel : ViewModel<IStreamManagerView> {
        private readonly Lazy<ISelectionService> _selectionService;
        private ICommand _openCommand;
        private ICommand _exitCommand;
        private KinectFile _kinectFile;

        [ImportingConstructor]
        public StreamManagerViewModel(IStreamManagerView view,
            Lazy<ISelectionService> selectionService) : base(view) {
            _selectionService = selectionService;
        }

        public ISelectionService SelectionService => _selectionService.Value;

        public ICommand OpenCommand {
            get { return _openCommand; }
            set { SetProperty(ref _openCommand, value); }
        }

        public ICommand ExitCommand {
            get { return _exitCommand; }
            set { SetProperty(ref _exitCommand, value); }
        }

        public KinectFile KinectFile {
            get { return _kinectFile; }
            set { SetProperty(ref _kinectFile, value); }
        }
    }
}