using Microsoft.EntityFrameworkCore;
using PrismDatabaseApp.Data;
using PrismDatabaseApp.Models;
using System.Threading.Tasks;
using System;
using System.Threading;

public class AlarmService
{
    private readonly DbContextOptions<AppDbContext> _options;

    public AlarmService(DbContextOptions<AppDbContext> options)
    {
        _options = options;
    }

    public async Task SaveAlarmAsync(string message, DateTime timestamp)
    {
        using (var context = new AppDbContext(_options)) // 작업마다 새 DbContext 생성
        {
            var alarm = new AlarmEntity
            {
                Message = message,
                Timestamp = timestamp
            };

            context.Alarms.Add(alarm);
            await context.SaveChangesAsync();
        }
    }
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public async Task SaveAlarmSequentialAsync(string message, DateTime timestamp)
    {
        await _semaphore.WaitAsync();
        try
        {
            using (var context = new AppDbContext(_options))
            {
                var alarm = new AlarmEntity
                {
                    Message = message,
                    Timestamp = timestamp
                };

                context.Alarms.Add(alarm);
                await context.SaveChangesAsync();
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

}
