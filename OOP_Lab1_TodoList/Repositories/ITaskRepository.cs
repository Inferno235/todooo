using System.Collections.Generic;
using TodoList.Models;

namespace TodoList.Repositories
{
    public interface ITaskRepository
    {
        void Save(IReadOnlyList<TodoModel> tasks, string filePath);
        IList<TodoModel> Load(string filePath);
    }
}
