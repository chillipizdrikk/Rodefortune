using RodeFortune.BLL.Models;
using RodeFortune.DAL.Models;

namespace RodeFortune.BLL.Services.Interfaces
{
    public interface IAdminPanelService
    {
        public Task<Result<bool>> DeleteTarotCardByNameAsync(string tarotCardName);
        public Task<Result<TarotCard>> CreateTarotCardAsync(string name, string arcana, string motto,
           string meaning, bool reversal, byte[]? imageUrl = null);

    }
}
