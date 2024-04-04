using AutoMapper;
using Entities.Models;
using Shared.DataTranferObjects;

namespace MelvinBankApi;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<Account, RegisterNewAccountDto>();

		CreateMap<Account, AuthenticateDto>();

		CreateMap<AuthenticateForCreationDto, Account>();

		CreateMap<RegisterNewAccountForCreationDto, Account>();

		CreateMap<UpdateAccountDto, Account>();

		CreateMap<Account, GetAccountDto>().ReverseMap()
			.ForMember("AccountName",
				opt => opt.MapFrom(x => string.Join(' ', x.FirstName, x.LastName)));



		//.ForMember(f => f.AccountName,
		//opt => opt.MapFrom(m => string.Join(' ',
		//m.FirstName, m.LastName)));

		CreateMap<TransactionRequestDto, Transaction>();

		CreateMap<UserForRegistrationDto, User>();

	}

}
