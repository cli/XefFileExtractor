using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Waf.Applications;
using System.Windows;
using XefFileExtractor.Applications.ViewModels;
using XefFileExtractor.Applications.Views;


namespace XefFileExtractor.Presentation.Views {
    [Export(typeof(IShellView))]
    public partial class ShellWindow : Window, IShellView {
        private readonly Lazy<ShellViewModel> _viewModel;
        public ShellWindow() {
            InitializeComponent();
            _viewModel = new Lazy<ShellViewModel>(() => ViewHelper.GetViewModel<ShellViewModel>(this));
        }

        public double VirtualScreenWidth => SystemParameters.VirtualScreenWidth;

        public double VirtualScreenHeight => SystemParameters.VirtualScreenHeight;

        public bool IsMaximized {
            get { return WindowState == WindowState.Maximized; }
            set {
                if (value) {
                    WindowState = WindowState.Maximized;
                } else if (WindowState == WindowState.Maximized) {
                    WindowState = WindowState.Normal;
                }
            }
        }

        private ShellViewModel ViewModel => _viewModel.Value;
    }
}
