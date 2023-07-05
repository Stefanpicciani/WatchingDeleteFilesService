namespace FileCleanupServices
{
    public class FileCleanupService : BackgroundService
    {
        private string directoryPath = "/var/www/mafirolapi/Assets/PDFGeneratedFiles";
        private FileSystemWatcher _fileWatcher;

        private readonly ILogger<FileCleanupService> _logger;

        public FileCleanupService(ILogger<FileCleanupService> logger)
        {
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _fileWatcher = new FileSystemWatcher(directoryPath);
            //_fileWatcher.Created += FileWatcher_Created;
            _fileWatcher.EnableRaisingEvents = true;

            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime now = DateTime.Now;
                DateTime nextCheckTime = new DateTime(now.Year, now.Month, now.Day, 10, 50, 0);


                if (now >= nextCheckTime)
                {
                    CleanupFiles();

                    nextCheckTime = nextCheckTime.AddDays(1);
                    TimeSpan timeUntilNextCheck = nextCheckTime - DateTime.Now;
                    _logger.LogInformation("Próxima verificação em: {timeUntilNextCheck}", timeUntilNextCheck);
                    await Task.Delay(timeUntilNextCheck, stoppingToken);                 
                }
                else
                {
                    TimeSpan timeUntilNextCheck = nextCheckTime - now;

                    _logger.LogInformation("Próxima verificação em: {timeUntilNextCheck}", timeUntilNextCheck);
                    await Task.Delay(timeUntilNextCheck, stoppingToken);

                }


            }
        }

        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            CleanupFiles();
        }

        private void CleanupFiles()
        {
            try
            {
                string[] files = Directory.GetFiles(directoryPath);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Erro ao excluir arquivos no diretório: {DirectoryPath}", directoryPath);
            }
        }
    }
}