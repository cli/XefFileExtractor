using System.Waf.Applications;

namespace XefFileExtractor.Applications.Views {
    public interface IControlView : IView {
        int GetProgress();

        void SetProgress(int progress);
    }
}