using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using XefFileExtractor.Applications.Services;
using XefFileExtractor.Applications.Views;

namespace XefFileExtractor.Applications.ViewModels {
    [Export]
    public class ShellViewModel : ViewModel<IShellView> {
        [ImportingConstructor]
        public ShellViewModel(IShellView view, IShellService shellService)
            : base(view) {
            ShellService = shellService;
            view.Closed += ViewClosed;
        }

        public string Title => ApplicationInfo.ProductName;

        public IShellService ShellService { get; }

        public void Show() {
            ViewCore.Show();
        }   

        private void Close() {
            ViewCore.Close();
        }

        private void ViewClosed(object sender, EventArgs e) {
            // Update settings
        }
    }
}
