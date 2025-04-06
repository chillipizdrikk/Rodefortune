using System.Threading.Tasks;

namespace RodeFortune.BLL.Services.Interfaces
{
    public interface ITarotService
    {
        Task GetTarotCardByBirthDateAsync(DateTime birthDate);
    }
}