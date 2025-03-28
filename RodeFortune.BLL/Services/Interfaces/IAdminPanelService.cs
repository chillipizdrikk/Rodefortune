using MongoDB.Bson;
using RodeFortune.BLL.Models;
using RodeFortune.DAL.Models;

namespace RodeFortune.BLL.Services.Interfaces
{
    public interface IAdminPanelService
    {
        public Task<Result<bool>> DeleteTarotCardByNameAsync(string tarotCardName);
        public Task<Result<TarotCard>> CreateTarotCardAsync(string name, string arcana, string motto,
           string meaning, bool reversal, byte[]? imageUrl = null);

        public Task<Result<Horoscope>> CreateHoroscopeAsync(string zodiacSign, string motto, string content, DateTime date);

        public Task<Result<Horoscope>> UpdateHoroscopeAsync(ObjectId id, string? zodiacSign = null, string? motto = null, string? content = null,
            DateTime? date = null);

        public Task<Result<bool>> DeleteHoroscopeAsync(ObjectId id);

    }
}
