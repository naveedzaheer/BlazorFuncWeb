using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using BlazorFuncWeb.Common;

namespace BlazorFuncWeb.Func
{
    public class TodoItemFunction
    {
        private readonly TodoItemDbContext _context;
        public TodoItemFunction(TodoItemDbContext context)
        {
            _context = context;
        }

        [FunctionName("GetTodos")]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todoitem")] HttpRequest req,
            ILogger log)
        {
            var items = await _context.TodoItems.ToListAsync();

            return new OkObjectResult(items);
        }

        [FunctionName("GetTodoById")]
        public async Task<IActionResult> GetById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todoitem/{id}")] HttpRequest req,
            string id, ILogger log)
        {
            var item = await _context.TodoItems.FirstOrDefaultAsync(a => a.Id == int.Parse(id));

            return new OkObjectResult(item);
        }

        [FunctionName("DeleteTodoById")]
        public async Task<IActionResult> DeleteById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todoitem/{id}")] HttpRequest req,
            string id, ILogger log)
        {
            var item = await _context.TodoItems.FirstOrDefaultAsync(a => a.Id == int.Parse(id));
            if (item != null)
            {
                _context.TodoItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                return new NotFoundResult();
            }
            return new OkResult();
        }

        [FunctionName("CreateTodo")]
        public async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todoitem")] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TodoItem todoItem = JsonConvert.DeserializeObject<TodoItem>(requestBody);
            var dbEntity = _context.Add(todoItem);
            await _context.SaveChangesAsync();
            return new OkObjectResult(dbEntity.Entity);
        }

        [FunctionName("UpdateTodo")]
        public async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todoitem")] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TodoItem todoItem = JsonConvert.DeserializeObject<TodoItem>(requestBody);
            var item = await _context.TodoItems.FirstOrDefaultAsync(a => a.Id == todoItem.Id);
            if (item == null)
            {
                return new NotFoundObjectResult(item);
            }

            item.Name = todoItem.Name;
            item.Description = todoItem.Description;
            item.Status = todoItem.Status;
            await _context.SaveChangesAsync();
            return new OkObjectResult(todoItem);
        }
    }
}
