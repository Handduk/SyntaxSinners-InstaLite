using Entity;

namespace Infrastructure.Repositories
{
    public interface ITodoRepository
    {
        Task<Todo> AddTodo(Todo todo);
        Task<Todo> DeleteTodoById(int id);
        Task<Todo> GetTodo(int id);
        Task<List<Todo>> GetTodos();
        Task<Todo> UpdateTodo(int id, Todo todo);
    }
}
