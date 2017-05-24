using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Controls;
using XefFileExtractor.Applications.ViewModels;
using XefFileExtractor.Applications.Views;

namespace XefFileExtractor.Presentation.Views {
    /// <summary>
    /// Interaction logic for ControlView.xaml
    /// </summary>
    [Export(typeof(IControlView))]
    public partial class ControlView : UserControl, IControlView {
        private readonly Lazy<ControlViewModel> _viewModel;

        [ImportingConstructor]
        public ControlView() {
            InitializeComponent();
            _viewModel = new Lazy<ControlViewModel>(() => ViewHelper.GetViewModel<ControlViewModel>(this));
        }

        private ControlViewModel ViewModel => _viewModel.Value;

        public int GetProgress() {
            return (int) ProgressBar.Value;
        }

        public void SetProgress(int progress) {
            // TODO: Find a more elegant solution. Should I be using the Dispatcher like this?
            Dispatcher.Invoke(() => {
                ProgressBar.Value = progress;
            });
        }
    }
}