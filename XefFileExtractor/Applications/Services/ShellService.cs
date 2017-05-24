using System;
using System.ComponentModel.Composition;
using XefFileExtractor.Applications.Views;

namespace XefFileExtractor.Applications.Services {
    [Export(typeof(IShellService)), Export]
    internal class ShellService : System.Waf.Foundation.Model, IShellService {
        private readonly Lazy<IShellView> _shellView;
        private object _streamView;
        private object _propertyView;
        private object _controlView;

        [ImportingConstructor]
        public ShellService(Lazy<IShellView> shellView) {
            _shellView = shellView;
        }

        public object ShellView => _shellView.Value;

        public object StreamView {
            get { return _streamView; }
            set { SetProperty(ref _streamView, value); }
        }

        public object PropertyView {
            get { return _propertyView; }
            set { SetProperty(ref _propertyView, value); }
        }

        public object ControlView {
            get { return _controlView; }
            set { SetProperty(ref _controlView, value); }
        }
    }
}
