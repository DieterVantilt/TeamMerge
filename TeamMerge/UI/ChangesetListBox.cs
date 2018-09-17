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
        public bool DisableAutoMergeListBox { get; set; }

        public ChangesetListBox()
        {
            SelectionChanged += AutoMergeListBox_SelectionChanged;
            SetSelectedItems(SelectedItems);
        }

        private void AutoMergeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItemsList != null)
            {
                if (!DisableAutoMergeListBox)
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
        }

        public ObservableCollection<ChangesetModel> SelectedItemsList
        {
            get { return (ObservableCollection<ChangesetModel>)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
           DependencyProperty.Register(nameof(SelectedItemsList), typeof(ObservableCollection<ChangesetModel>), typeof(ChangesetListBox), new PropertyMetadata(null, null, CoerceVallBack));

        private static object CoerceVallBack(DependencyObject d, object baseValue)
        {
            var changesetListBox = ((ChangesetListBox)d);

            changesetListBox.DisableAutoMergeListBox = true;

            changesetListBox.SetSelectedItems(changesetListBox.SelectedItemsList);

            changesetListBox.DisableAutoMergeListBox = false;

            return baseValue;
        }
    }
}