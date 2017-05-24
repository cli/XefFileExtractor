using System.ComponentModel;

namespace XefFileExtractor.Applications.Services {
    public interface IShellService : INotifyPropertyChanged {
        object ShellView { get; }
        object StreamView { get; set; }
        object PropertyView { get; set; }
        object ControlView { get; set; }
    }
}
