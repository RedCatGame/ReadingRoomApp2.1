using System.Windows;
using System.Windows.Controls;

namespace ReadingRoomApp.Presentation.Controls
{
    public partial class LoadingOverlay : UserControl
    {
        public static readonly DependencyProperty LoadingTextProperty =
            DependencyProperty.Register("LoadingText", typeof(string), typeof(LoadingOverlay),
                new PropertyMetadata("Загрузка..."));

        public string LoadingText
        {
            get { return (string)GetValue(LoadingTextProperty); }
            set { SetValue(LoadingTextProperty, value); }
        }

        public LoadingOverlay()
        {
            InitializeComponent();
        }
    }
}