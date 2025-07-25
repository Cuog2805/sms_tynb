using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Service;

namespace SMS_TYNB.BackgoundSercvice
{
    public class FileCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FileCleanupService> _logger;
        private readonly string _uploadRoot;

        public FileCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<FileCleanupService> logger,
            IWebHostEnvironment env)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _uploadRoot = Path.Combine(env.WebRootPath, "upload");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FileCleanupService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = now.Date.AddDays(1);
                //nextRun = now.AddSeconds(10);
                var delay = nextRun - now;

                _logger.LogInformation("Next cleanup scheduled at {NextRun}", nextRun);
                await Task.Delay(delay, stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var configService = scope.ServiceProvider.GetRequiredService<IConfigService>();

                try
                {
                    var cfg = await configService.FindByKey("file_delete_after");
                    if (cfg != null && cfg.IsUsed == 1 && int.TryParse(cfg.Value, out int days))
                    {
                        await DeleteOldFilesAsync(days);
                    }
                    else
                    {
                        _logger.LogInformation("Auto-delete disabled or invalid config.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during file cleanup");
                }
            }
        }

        private Task DeleteOldFilesAsync(int maxAgeDays)
        {
            if (!Directory.Exists(_uploadRoot))
            {
                _logger.LogWarning("Upload directory does not exist: {UploadRoot}", _uploadRoot);
                return Task.CompletedTask;
            }

            var cutoff = DateTime.Now.AddDays(-maxAgeDays);
            //var cutoff = DateTime.Now.AddSeconds(-maxAgeDays);

            var files = Directory.EnumerateFiles(_uploadRoot, "*.*", SearchOption.AllDirectories);
            int deletedCount = 0;

            foreach (var filePath in files)
            {
                try
                {
                    var info = new FileInfo(filePath);
                    if (info.LastWriteTime < cutoff)
                    {
                        info.Delete();
                        deletedCount++;
                        _logger.LogDebug("Deleted file: {File}", filePath);
                    }
                }
                catch (Exception exFile)
                {
                    _logger.LogError(exFile, "Failed to delete file: {File}", filePath);
                }
            }

            _logger.LogInformation("File cleanup complete. {Count} files older than {Days} days were deleted.",
                deletedCount, maxAgeDays);

            return Task.CompletedTask;
        }
    }
}
