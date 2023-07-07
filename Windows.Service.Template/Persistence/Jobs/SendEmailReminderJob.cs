using Windows.Service.Template.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Windows.Service.Template.Domain.Entities;
using Windows.Service.Template.Domain.Enums;
using Windows.Service.Template.Domain.ValueObjects;

namespace Windows.Service.Template.Persistence.Jobs
{
    public class SendEmailReminderJob : IJob
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<SendEmailReminderJob> _logger;

        public SendEmailReminderJob(IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {

                var contextOptions = new DbContextOptionsBuilder<TestDbContext>()
                    .UseMySql(_configuration.GetConnectionString("DefaultConnection") ?? string.Empty,
                        new MySqlServerVersion(new Version(
                            int.Parse(_configuration.GetSection("MySQLVersion").GetSection("Major").Value ??
                                      string.Empty),
                            int.Parse(_configuration.GetSection("MySQLVersion").GetSection("Minor").Value ??
                                      string.Empty),
                            int.Parse(_configuration.GetSection("MySQLVersion").GetSection("Build").Value ??
                                      string.Empty)
                        ))).Options;

                using var _context = new TestDbContext(contextOptions);

                var list = new TodoList
                {
                    Title = "List From Windows Service",
                    Color = Color.Red
                };

                await _context.TodoLists.AddAsync(list);


                await _context.TodoItems.AddAsync(new TodoItem
                {
                    Title = "Item From Windows Service",
                    Note = "This is a test",
                    Priority = PriorityLevel.Low,
                    Reminder = DateTime.Now + TimeSpan.FromHours(1),
                    Done = false,
                    List = list,

                });

                await _context.SaveChangesAsync();

                var message =
                    $"You have {_context.TodoItems.Count(x => x.Reminder >= DateTime.Now)} items to be completed today: {DateTime.Now}";

                _emailService.SendEmailAsync("test@localhost.com", "Test", message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"There was an error when trying to send the reminder email: {ex.Message}");
            }
        }
    }
}
