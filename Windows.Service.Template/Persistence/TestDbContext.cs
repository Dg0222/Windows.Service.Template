using System.Reflection;
using Windows.Service.Template.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Windows.Service.Template.Persistence;

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options){}

        public DbSet<TodoList> TodoLists { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }

