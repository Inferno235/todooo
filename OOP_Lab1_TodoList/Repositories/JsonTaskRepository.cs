using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TodoList.Models;

namespace TodoList.Repositories
{
    public class JsonTaskRepository : ITaskRepository
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        public void Save(IReadOnlyList<TodoModel> tasks, string filePath)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Путь к файлу не указан.", nameof(filePath));

            var dtoList = new List<TodoDto>();
            foreach (var task in tasks)
                dtoList.Add(TodoDto.FromModel(task));

            var json = JsonConvert.SerializeObject(dtoList, Settings);
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(filePath, json);
        }

        public IList<TodoModel> Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Путь к файлу не указан.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден.", filePath);

            var json = File.ReadAllText(filePath);
            var dtoList = JsonConvert.DeserializeObject<List<TodoDto>>(json) ?? new List<TodoDto>();

            var result = new List<TodoModel>();
            foreach (var dto in dtoList)
                result.Add(dto.ToModel());

            return result;
        }

        private sealed class TodoDto
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime? DueDate { get; set; }
            public TodoPriority Priority { get; set; }
            public TodoStatus Status { get; set; }

            public static TodoDto FromModel(TodoModel model)
            {
                return new TodoDto
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    CreatedAt = model.CreatedAt,
                    DueDate = model.DueDate,
                    Priority = model.Priority,
                    Status = model.Status
                };
            }

            public TodoModel ToModel()
            {
                return new TodoModel(Id, Title, Description, CreatedAt, DueDate, Priority, Status);
            }
        }
    }
}
