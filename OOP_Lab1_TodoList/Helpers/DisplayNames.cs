using System;
using TodoList.Models;

namespace TodoList.Helpers
{
    public static class DisplayNames
    {
        public static string Status(TodoStatus status)
        {
            switch (status)
            {
                case TodoStatus.NotStarted: return "Не начата";
                case TodoStatus.InProgress: return "В процессе";
                case TodoStatus.Completed: return "Выполнена";
                default: return status.ToString();
            }
        }

        public static string Priority(TodoPriority priority)
        {
            switch (priority)
            {
                case TodoPriority.Low: return "Низкий";
                case TodoPriority.Medium: return "Средний";
                case TodoPriority.High: return "Высокий";
                default: return priority.ToString();
            }
        }
    }
}
