using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Controls;
using XefFileExtractor.Applications.ViewModels;
using XefFileExtractor.Applications.Views;

namespace XefFileExtractor.Presentation.Views {
    /// <summary>
    /// Interaction logic for StreamManagerView.xaml
    /// </summary>
    [Export(typeof(IStreamManagerView))]
    public partial class StreamView : UserControl, IStreamManagerView {
        private readonly Lazy<StreamManagerViewModel> _viewModel;

        public StreamView() {
            InitializeComponent();
            _viewModel = new Lazy<StreamManagerViewModel>(() => ViewHelper.GetViewModel<StreamManagerViewModel>(this));
        }

        private StreamManagerViewModel ViewModel => _viewModel.Value;
    }
}