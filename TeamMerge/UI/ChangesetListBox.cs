using Domain.Entities;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
                    foreach (var removedItem in e.RemovedItems.Cast<Changeset>())
                    {
                        SelectedItemsList.Remove(removedItem);
                    }

                    foreach (var addItem in e.AddedItems.Cast<Changeset>())
                    {
                        SelectedItemsList.Add(addItem);
                    }
                }                
            }
        }

        public ObservableCollection<Changeset> SelectedItemsList
        {
            get { return (ObservableCollection<Changeset>)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
           DependencyProperty.Register(nameof(SelectedItemsList), typeof(ObservableCollection<Changeset>), typeof(ChangesetListBox), new PropertyMetadata(null, null, CoerceVallBack));

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