using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace DrillSensorsEmulator.Markers
{
    /// <summary>
    /// Логика взаимодействия для DrillMarkerBody.xaml
    /// </summary>
    public partial class DrillMarkerBody : UserControl
    {
        public Popup PopupHint;
        public Label Hint;

        internal DrillMarkerBody()
        {
            InitializeComponent();

            Hint = new Label();
            Hint.Style = (Style)Application.Current.TryFindResource("LabelStyle");
            Hint.FontFamily = (FontFamily)Application.Current.TryFindResource("MontserratBlackItalic");

            PopupHint = new Popup();
            PopupHint.Placement = PlacementMode.Mouse;
            PopupHint.Child = Hint;
        }
    }
}
