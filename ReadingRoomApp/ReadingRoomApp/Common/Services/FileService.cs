using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadingRoomApp.Common.Services
{
    public class FileService : IFileService
    {
        public async Task<string> ReadTextAsync(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                App.Logger.LogError($"Ошибка при чтении файла {filePath}: {ex.Message}");
                throw;
            }
        }

        public async Task WriteTextAsync(string filePath, string content)
        {
            try
            {
                // Создаем директорию, если не существует
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var writer = new StreamWriter(filePath, false))
                {
                    await writer.WriteAsync(content);
                }
            }
            catch (Exception ex)
            {
                App.Logger.LogError($"Ошибка при записи в файл {filePath}: {ex.Message}");
                throw;
            }
        }

        public Task<bool> FileExistsAsync(string filePath)
        {
            return Task.FromResult(File.Exists(filePath));
        }

        public Task<IEnumerable<string>> GetFilesAsync(string directoryPath, string searchPattern = "*.*")
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    return Task.FromResult<IEnumerable<string>>(new List<string>());
                }

                return Task.FromResult<IEnumerable<string>>(Directory.GetFiles(directoryPath, searchPattern));
            }
            catch (Exception ex)
            {
                App.Logger.LogError($"Ошибка при получении списка файлов из {directoryPath}: {ex.Message}");
                throw;
            }
        }

        public Task<string> SelectFileAsync(string title, string filter)
        {
            return Task.Run(() =>
            {
                var dialog = new OpenFileDialog
                {
                    Title = title,
                    Filter = filter,
                    Multiselect = false
                };

                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            });
        }

        public Task<string> SaveFileAsync(string title, string defaultFileName, string filter)
        {
            return Task.Run(() =>
            {
                var dialog = new SaveFileDialog
                {
                    Title = title,
                    FileName = defaultFileName,
                    Filter = filter
                };

                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            });
        }
    }
}