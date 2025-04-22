using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Repositories;
using ReadingRoomApp.Infrastructure.Helpers;
using ReadingRoomApp.Common.Constants;

namespace ReadingRoomApp.Infrastructure.Data.Repositories
{
    public class FileGenreRepository : IGenreRepository
    {
        private List<Genre> _genres;

        public FileGenreRepository()
        {
            LoadGenres();
        }

        private void LoadGenres()
        {
            _genres = JsonHelper.LoadFromJsonFile<Genre>(AppConstants.GENRES_FILE_PATH);
        }

        private void SaveGenres()
        {
            JsonHelper.SaveToJsonFile(_genres, AppConstants.GENRES_FILE_PATH);
        }

        public async Task<List<Genre>> GetAllAsync()
        {
            return await Task.FromResult(_genres);
        }

        public async Task<Genre> GetByIdAsync(int id)
        {
            return await Task.FromResult(_genres.FirstOrDefault(g => g.Id == id));
        }

        public async Task<Genre> AddAsync(Genre genre)
        {
            genre.Id = _genres.Any() ? _genres.Max(g => g.Id) + 1 : 1;
            _genres.Add(genre);
            SaveGenres();
            return await Task.FromResult(genre);
        }

        public async Task<Genre> UpdateAsync(Genre genre)
        {
            var existingGenre = _genres.FirstOrDefault(g => g.Id == genre.Id);
            if (existingGenre == null)
                return null;

            int index = _genres.IndexOf(existingGenre);
            _genres[index] = genre;
            SaveGenres();
            return await Task.FromResult(genre);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var genre = _genres.FirstOrDefault(g => g.Id == id);
            if (genre == null)
                return false;

            _genres.Remove(genre);
            SaveGenres();
            return await Task.FromResult(true);
        }
    }
}