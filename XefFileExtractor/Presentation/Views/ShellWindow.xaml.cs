using System.ComponentModel.Composition;
using System.Windows;
using XefFileExtractor.Applications.Views;

namespace XefFileExtractor.Presentation.Views
{
    [Export(typeof(IShellView))]
    public partial class ShellWindow : Window, IShellView
    {
        public ShellWindow()
        {
            InitializeComponent();
        }
    }
}
