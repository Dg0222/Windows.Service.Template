using System.Linq.Expressions;
using Windows.Service.Template.Domain.Entities;
using Windows.Service.Template.Domain.Enums;
using Windows.Service.Template.Persistence;
using Windows.Service.Template.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Color = Windows.Service.Template.Domain.ValueObjects.Color;

namespace Windows.Service.Template
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;


        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Test Windows Service started running at: {time}", DateTimeOffset.Now);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"There was an error: {ex.InnerException?.Message}");
            }
        }
    }
}