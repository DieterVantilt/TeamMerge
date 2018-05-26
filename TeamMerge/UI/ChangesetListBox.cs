using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TeamMerge.Services.Models;

namespace TeamMerge.UI
{
    public class ChangesetListBox 
        : ListBox
    {
        public ChangesetListBox()
        {
            SelectionChanged += AutoMergeListBox_SelectionChanged;
        }

        private void AutoMergeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItemsList != null)
            {
                foreach (var removedItem in e.RemovedItems.Cast<ChangesetModel>())
                {
                    SelectedItemsList.Remove(removedItem);
                }

                foreach (var addItem in e.AddedItems.Cast<ChangesetModel>())
                {
                    SelectedItemsList.Add(addItem);
                }
            }
        }

        public ObservableCollection<ChangesetModel> SelectedItemsList
        {
            get { return (ObservableCollection<ChangesetModel>)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
           DependencyProperty.Register("SelectedItemsList", typeof(ObservableCollection<ChangesetModel>), typeof(ChangesetListBox), new PropertyMetadata(null));
    }
}