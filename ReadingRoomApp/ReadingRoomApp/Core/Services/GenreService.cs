using System.Collections.Generic;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Repositories;
using ReadingRoomApp.Core.Interfaces.Services;

namespace ReadingRoomApp.Core.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<List<Genre>> GetAllGenresAsync()
        {
            return await _genreRepository.GetAllAsync();
        }

        public async Task<Genre> GetGenreByIdAsync(int id)
        {
            return await _genreRepository.GetByIdAsync(id);
        }

        public async Task<Genre> AddGenreAsync(Genre genre)
        {
            return await _genreRepository.AddAsync(genre);
        }

        public async Task<Genre> UpdateGenreAsync(Genre genre)
        {
            return await _genreRepository.UpdateAsync(genre);
        }

        public async Task<bool> DeleteGenreAsync(int id)
        {
            return await _genreRepository.DeleteAsync(id);
        }
    }
}