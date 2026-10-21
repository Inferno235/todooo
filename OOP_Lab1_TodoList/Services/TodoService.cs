using System;
using System.Collections.Generic;
using System.Linq;
using TodoList.Models;

namespace TodoList.Services
{
    public class TodoService
    {
        private readonly List<TodoModel> _tasks = new List<TodoModel>();

        public IReadOnlyList<TodoModel> Tasks => _tasks.AsReadOnly();

        public TodoModel AddTask(
            string title,
            string description,
            DateTime? dueDate,
            TodoPriority priority,
            TodoStatus status = TodoStatus.NotStarted)
        {
            var task = new TodoModel
            {
                Title = title,
                Description = description,
                DueDate = dueDate,
                Priority = priority,
                Status = status
            };

            _tasks.Add(task);
            return task;
        }

        public void UpdateTask(
            Guid id,
            string title,
            string description,
            DateTime? dueDate,
            TodoPriority priority,
            TodoStatus status)
        {
            var task = GetTaskOrThrow(id);
            task.Title = title;
            task.Description = description;
            task.DueDate = dueDate;
            task.Priority = priority;
            task.Status = status;
        }

        public void RemoveTask(Guid id)
        {
            var task = GetTaskOrThrow(id);
            _tasks.Remove(task);
        }

        public void ChangeStatus(Guid id, TodoStatus status)
        {
            var task = GetTaskOrThrow(id);
            task.Status = status;
        }

        public void ReplaceAll(IEnumerable<TodoModel> tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));

            _tasks.Clear();
            _tasks.AddRange(tasks.Select(t => t.Clone()));
        }

        public TodoModel? FindById(Guid id)
        {
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        private TodoModel GetTaskOrThrow(Guid id)
        {
            var task = FindById(id);
            if (task == null)
                throw new InvalidOperationException("Задача не найдена.");
            return task;
        }
    }
}
