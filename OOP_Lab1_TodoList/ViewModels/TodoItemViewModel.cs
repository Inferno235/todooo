using System;
using TodoList.Helpers;
using TodoList.Models;

namespace TodoList.ViewModels
{
    public class TodoItemViewModel
    {
        public TodoItemViewModel(TodoModel model)
        {
            Model = model;
        }

        public TodoModel Model { get; }

        public Guid Id => Model.Id;
        public string Title => Model.Title;
        public string Description => Model.Description;
        public DateTime CreatedAt => Model.CreatedAt;
        public DateTime? DueDate => Model.DueDate;
        public string PriorityText => DisplayNames.Priority(Model.Priority);
        public string StatusText => DisplayNames.Status(Model.Status);
        public TodoPriority Priority => Model.Priority;
        public TodoStatus Status => Model.Status;
    }
}
