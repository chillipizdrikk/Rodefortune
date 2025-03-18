using RodeFortune.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RodeFortune.BLL.Services.Interfaces
{
    public interface IDivinationService
    {
            public Task<(TarotCard Card, bool IsReversed)> GetYesNoReadingAsync();
            public Task<List<(TarotCard Card, bool IsReversed, string Position)>> GetPastPresentFutureReadingAsync();
            public Task<(TarotCard Card, bool IsReversed, bool IsNew)> GetCardOfTheDayAsync(string userId);
            public Task<List<TarotCard>> GetCardsAsync(string searchTerm = null, string arcana = null);
 
    }
}
