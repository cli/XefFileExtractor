using System.Waf.Applications;

namespace XefFileExtractor.Applications.Views
{
    internal interface IShellView : IView
    {
        void Show();

        void Close();
    }
}
