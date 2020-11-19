using BlazorFuncWeb.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorFuncWeb.Func
{

    public class TodoItemDbContext : DbContext
    {
        public TodoItemDbContext(DbContextOptions<TodoItemDbContext> options)
            : base(options)
        { }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
