using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace ReadingRoomApp.Presentation.Services
{
    public interface IResourceService
    {
        BitmapImage GetImage(string imageName);
    }

    public class ResourceService : IResourceService
    {
        private readonly string _resourcesPath;

        public ResourceService(string resourcesPath = null)
        {
            _resourcesPath = resourcesPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
        }

        public BitmapImage GetImage(string imageName)
        {
            string imagePath = Path.Combine(_resourcesPath, "Icons", imageName);

            if (!File.Exists(imagePath))
            {
                App.Logger.LogWarning($"Изображение не найдено: {imagePath}");
                return null;
            }

            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(imagePath);
                image.EndInit();
                return image;
            }
            catch (Exception ex)
            {
                App.Logger.LogError($"Ошибка при загрузке изображения {imagePath}: {ex.Message}");
                return null;
            }
        }
    }
}