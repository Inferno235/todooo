using System;

namespace TodoList.Models
{
    public class TodoModel
    {
        public Guid Id { get; }

        private string _title = string.Empty;
        private string _description = string.Empty;
        private DateTime _createdAt;
        private DateTime? _dueDate;
        private TodoPriority _priority;
        private TodoStatus _status;

        public TodoModel()
        {
            Id = Guid.NewGuid();
            _createdAt = DateTime.Now;
            _priority = TodoPriority.Medium;
            _status = TodoStatus.NotStarted;
        }

        public TodoModel(
            Guid id,
            string title,
            string description,
            DateTime createdAt,
            DateTime? dueDate,
            TodoPriority priority,
            TodoStatus status)
        {
            Id = id;
            Title = title;
            Description = description;
            _createdAt = createdAt;
            DueDate = dueDate;
            Priority = priority;
            Status = status;
        }

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Название задачи не может быть пустым.", nameof(value));
                _title = value.Trim();
            }
        }

        public string Description
        {
            get => _description;
            set => _description = value?.Trim() ?? string.Empty;
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (value > DateTime.Now.AddMinutes(1))
                    throw new ArgumentException("Дата создания не может быть в будущем.", nameof(value));
                _createdAt = value;
            }
        }

        public DateTime? DueDate
        {
            get => _dueDate;
            set
            {
                if (value.HasValue && value.Value.Date < _createdAt.Date)
                    throw new ArgumentException("Срок выполнения не может быть раньше даты создания.", nameof(value));
                _dueDate = value;
            }
        }

        public TodoPriority Priority
        {
            get => _priority;
            set
            {
                if (!Enum.IsDefined(typeof(TodoPriority), value))
                    throw new ArgumentOutOfRangeException(nameof(value));
                _priority = value;
            }
        }

        public TodoStatus Status
        {
            get => _status;
            set
            {
                if (!Enum.IsDefined(typeof(TodoStatus), value))
                    throw new ArgumentOutOfRangeException(nameof(value));
                _status = value;
            }
        }

        public TodoModel Clone()
        {
            return new TodoModel(Id, _title, _description, _createdAt, _dueDate, _priority, _status);
        }
    }
}
