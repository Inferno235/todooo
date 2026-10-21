using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using TodoList.Helpers;
using TodoList.Models;
using TodoList.Repositories;
using TodoList.Services;
using TodoList.ViewModels;

namespace TodoList
{
    public partial class MainWindow : Window
    {
        private readonly TodoService _todoService = new TodoService();
        private readonly ITaskRepository _repository = new JsonTaskRepository();
        private Guid? _editingTaskId;

        public MainWindow()
        {
            InitializeComponent();
            InitializeCombos();
            RefreshGrid();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var answer = MessageBox.Show(
                "Загрузить ранее сохранённые задачи из JSON-файла?",
                "Todo List",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (answer == MessageBoxResult.Yes)
                LoadFromFile();
        }

        private void InitializeCombos()
        {
            PriorityCombo.ItemsSource = Enum.GetValues(typeof(TodoPriority))
                .Cast<TodoPriority>()
                .Select(p => new ComboItem<TodoPriority>(DisplayNames.Priority(p), p))
                .ToList();
            PriorityCombo.DisplayMemberPath = "Text";
            PriorityCombo.SelectedValuePath = "Value";
            PriorityCombo.SelectedIndex = 1;

            StatusCombo.ItemsSource = Enum.GetValues(typeof(TodoStatus))
                .Cast<TodoStatus>()
                .Select(s => new ComboItem<TodoStatus>(DisplayNames.Status(s), s))
                .ToList();
            StatusCombo.DisplayMemberPath = "Text";
            StatusCombo.SelectedValuePath = "Value";
            StatusCombo.SelectedIndex = 0;
        }

        private void RefreshGrid()
        {
            TasksGrid.ItemsSource = _todoService.Tasks
                .Select(t => new TodoItemViewModel(t))
                .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
                .ThenByDescending(t => t.Priority)
                .ToList();

            StatusBarText.Text = $"Задач: {_todoService.Tasks.Count}";
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _editingTaskId = null;
            ApplyButton.Content = "Добавить задачу";
            ClearForm();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksGrid.SelectedItem is not TodoItemViewModel selected)
            {
                MessageBox.Show("Выберите задачу для редактирования.", "Todo List", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            LoadFormFromTask(selected.Model);
            _editingTaskId = selected.Id;
            ApplyButton.Content = "Сохранить изменения";
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksGrid.SelectedItem is not TodoItemViewModel selected)
            {
                MessageBox.Show("Выберите задачу для удаления.", "Todo List", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Удалить задачу «{selected.Title}»?",
                "Todo List",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            _todoService.RemoveTask(selected.Id);
            ClearForm();
            RefreshGrid();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var title = TitleBox.Text;
                var description = DescriptionBox.Text;
                var dueDate = DueDatePicker.SelectedDate;
                var priority = GetSelectedPriority();
                var status = GetSelectedStatus();

                if (_editingTaskId.HasValue)
                {
                    _todoService.UpdateTask(_editingTaskId.Value, title, description, dueDate, priority, status);
                }
                else
                {
                    _todoService.AddTask(title, description, dueDate, priority, status);
                }

                _editingTaskId = null;
                ApplyButton.Content = "Добавить задачу";
                ClearForm();
                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearFormButton_Click(object sender, RoutedEventArgs e)
        {
            _editingTaskId = null;
            ApplyButton.Content = "Добавить задачу";
            ClearForm();
        }

        private void TasksGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TasksGrid.SelectedItem is TodoItemViewModel selected)
                LoadFormFromTask(selected.Model);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON (*.json)|*.json",
                FileName = "tasks.json",
                Title = "Сохранить задачи"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                _repository.Save(_todoService.Tasks, dialog.FileName);
                MessageBox.Show("Задачи сохранены.", "Todo List", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadFromFile();
        }

        private void LoadFromFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON (*.json)|*.json",
                Title = "Загрузить задачи"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                var tasks = _repository.Load(dialog.FileName);
                _todoService.ReplaceAll(tasks);
                ClearForm();
                RefreshGrid();
                MessageBox.Show($"Загружено задач: {tasks.Count}", "Todo List", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFormFromTask(TodoModel task)
        {
            TitleBox.Text = task.Title;
            DescriptionBox.Text = task.Description;
            DueDatePicker.SelectedDate = task.DueDate;
            PriorityCombo.SelectedValue = task.Priority;
            StatusCombo.SelectedValue = task.Status;
        }

        private void ClearForm()
        {
            TitleBox.Clear();
            DescriptionBox.Clear();
            DueDatePicker.SelectedDate = null;
            PriorityCombo.SelectedValue = TodoPriority.Medium;
            StatusCombo.SelectedValue = TodoStatus.NotStarted;
            TasksGrid.SelectedItem = null;
        }

        private TodoPriority GetSelectedPriority()
        {
            if (PriorityCombo.SelectedValue is TodoPriority priority)
                return priority;
            throw new InvalidOperationException("Выберите приоритет.");
        }

        private TodoStatus GetSelectedStatus()
        {
            if (StatusCombo.SelectedValue is TodoStatus status)
                return status;
            throw new InvalidOperationException("Выберите статус.");
        }

        private sealed class ComboItem<T>
        {
            public ComboItem(string text, T value)
            {
                Text = text;
                Value = value;
            }

            public string Text { get; }
            public T Value { get; }
        }
    }
}
