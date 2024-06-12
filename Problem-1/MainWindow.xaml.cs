using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CodeExample
{
    // Main window for the todo list
    public partial class MainWindow : Window
    {
        // Hold list of todo items
        public ObservableCollection<TodoListItem> TodoItems { get; set; }
        // Hold list of filtered todo list items
        public ObservableCollection<TodoListItem> DisplayedItems { get; set; }

        // Main window constructor 
        public MainWindow()
        {
            InitializeComponent();
            TodoItems = new ObservableCollection<TodoListItem>();
            DisplayedItems = new ObservableCollection<TodoListItem>(TodoItems);
            masterList.ItemsSource = DisplayedItems;
        }

        // Event handler for the add button
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (masterList.SelectedItem != null)
            {
                // Update item content
                var selectedItem = (TodoListItem)masterList.SelectedItem;
                selectedItem.Description = descriptionField.Text;
                selectedItem.DueDate = dueDatePicker.SelectedDate ?? DateTime.Now;
                selectedItem.IsDone = isDoneCheckbox.IsChecked == true;
            }
            else
            {
                // Add new item to the todo list
                var newItem = new TodoListItem(titleField.Text)
                {
                    Description = descriptionField.Text,
                    DueDate = dueDatePicker.SelectedDate ?? DateTime.Now,
                    IsDone = isDoneCheckbox.IsChecked == true
                };
                TodoItems.Add(newItem);
            }
            // Update displayed items
            UpdateDisplayedItems();
            ClearDetailFields();
        }
        // Event handler for the edit button
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Load details of selected task
            if (masterList.SelectedItem != null)
            {
                var selectedItem = (TodoListItem)masterList.SelectedItem;
                titleField.Text = selectedItem.Title;
                descriptionField.Text = selectedItem.Description;
                dueDatePicker.SelectedDate = selectedItem.DueDate;
                isDoneCheckbox.IsChecked = selectedItem.IsDone;
                // Remove read-only
                descriptionField.IsReadOnly = false;
            }
        }
        // Event handler for the delete button
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (masterList.SelectedItem != null)
            {
                // Updated the list after task deletion
                TodoItems.Remove((TodoListItem)masterList.SelectedItem);
                UpdateDisplayedItems();
            }
        }
        // Event handler for selection change
        private void MasterList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (masterList.SelectedItem != null)
            {
                // Load selected item contents
                var selectedItem = (TodoListItem)masterList.SelectedItem;
                titleField.Text = selectedItem.Title;
                descriptionField.Text = selectedItem.Description;
                dueDatePicker.SelectedDate = selectedItem.DueDate;
                isDoneCheckbox.IsChecked = selectedItem.IsDone;
                // Remove read-only
                descriptionField.IsReadOnly = true;
            }
        }
        // Event handler for the show all checkbox
        private void ShowAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Update displayed items depending on the checkbox
            UpdateDisplayedItems();
        }
        private void UpdateDisplayedItems()
        {
            DisplayedItems.Clear();
            var itemsToShow = showAllCheckBox.IsChecked == true
                ? TodoItems 
                : TodoItems.Where(item => !item.IsDone);
            foreach (var item in itemsToShow)
            {
                DisplayedItems.Add(item);
            }
        }
        // Clear detail fields
        private void ClearDetailFields()
        {
            titleField.Clear();
            descriptionField.Clear();
            dueDatePicker.SelectedDate = null;
            isDoneCheckbox.IsChecked = false;

            descriptionField.IsReadOnly = false;
            masterList.SelectedItem = null;
        }
    }
    // Class for a single todo list item
    public class TodoListItem : INotifyPropertyChanged
    {
        // List item contents
        private string description;
        private DateTime dueDate;
        private bool isDone;

        public TodoListItem(string title)
        {
            Title = title;
        }
        // Title property
        public string Title { get; }
        // Description property
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }
        // Due date property
        public DateTime DueDate
        {
            get => dueDate;
            set
            {
                dueDate = value;
                OnPropertyChanged();
            }
        }
        // Done? checkbox property
        public bool IsDone
        {
            get => isDone;
            set
            {
                isDone = value;
                OnPropertyChanged();
            }
        }
        // Event handler for property changes
        public event PropertyChangedEventHandler PropertyChanged;
        // Trigger the property change 
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
