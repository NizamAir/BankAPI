using AutoMapper;
using BankAPI.Models;
using BankAPI.Shared.DTOs.AccountDTOs;
using BankAPI.Shared.DTOs.TransactionDTOs;

namespace BankAPI.Profiles
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<RegisterNewAccountDto, Account>();

            CreateMap<UpdateAccountDto, Account>();

            CreateMap<Account, GetAccountDto>();


            CreateMap<TransactionRequestDto, Transaction>();
        }
    }
}
