namespace FileCleanupServices
{
    public class FileCleanupService : BackgroundService
    {
        private string directoryPath = @"C:\\PDFGenetator";
        private FileSystemWatcher _fileWatcher;
        private Timer timer;

        private readonly ILogger<FileCleanupService> _logger;

        public FileCleanupService(ILogger<FileCleanupService> logger)
        {
            _logger = logger;
        }


        protected override  async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Configura o FileSystemWatcher para monitorar o diretório
            _fileWatcher = new FileSystemWatcher(directoryPath);
            _fileWatcher.Created += FileWatcher_Created;
            _fileWatcher.EnableRaisingEvents= true;

            while (!stoppingToken.IsCancellationRequested) 
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                CleanupFiles();
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