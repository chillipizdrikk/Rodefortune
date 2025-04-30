using RodeFortune.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RodeFortune.BLL.Services.Interfaces
{
    public interface IConstantDivinationService
    {
        public Task<TarotCard> GetTarotCardByBirthDateAsync(DateTime birthDate);
    }
}
