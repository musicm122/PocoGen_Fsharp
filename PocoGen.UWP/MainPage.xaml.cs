using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace PocoGen.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadApplication(new PocoGen.App());
        }
    }
}
