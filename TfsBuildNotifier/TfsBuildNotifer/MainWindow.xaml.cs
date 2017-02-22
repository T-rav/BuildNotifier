using System.Windows;
using TfsBuildNotifier.DataContext;

namespace TfsBuildNotifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BuildMessageText.DataContext = StaticMessageHolder.Message;
        }
    }
}
