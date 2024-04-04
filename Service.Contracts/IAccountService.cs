using Entities.LinkModels;
using Entities.Models;
using Shared.DataTranferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts;

public interface IAccountService
{
	Task<RegisterNewAccountDto> RegisterNewAccountAsync(RegisterNewAccountForCreationDto newAccount, string Pin, string ConfirmPin, bool trackChanges);
	Task<IEnumerable<GetAccountDto>> GetAllAccountsFromDb(bool trackChanges);
	Task<(LinkResponse linkResponse, MetaData metaData)> GetAllAccountsAsync(LinkParameters linkParameters, /*string AccountNumber,*/ bool trackChanges);
	//Task<GetAccountDto> AuthenticateAsync(string AccountNumber, string Pin, bool trackChanges);
	Task<(GetAccountDto GetAccountDto, Account Account)> GetByAccountNumberAsync(string AccountNumber, bool trackChanges);
	Task<GetAccountDto> GetAccountByIdAsync(int Id, bool trackChanges);
	Task UpdateAccountAsync(UpdateAccountDto account, bool trackChanges, string Pin = null);
	Task DeleteAccountAsync(int Id, bool trackChanges);	


}
