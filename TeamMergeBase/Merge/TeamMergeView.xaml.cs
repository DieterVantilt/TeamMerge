using System.Windows.Controls;

namespace TeamMergeBase.Merge
{
    /// <summary>
    /// Interaction logic for TeamMergeView.xaml
    /// </summary>
    public partial class TeamMergeView 
        : UserControl
    {
        public TeamMergeView()
        {
            //necessary to provoke dll from loading...
            var trig = new Microsoft.Xaml.Behaviors.EventTrigger(); trig.SourceName = "foo";

            InitializeComponent();
        }
    }
}