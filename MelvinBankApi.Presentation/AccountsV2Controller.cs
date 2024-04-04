using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelvinBankApi.Presentation;

//[ApiVersion("2.0")]
[Route("api/v2/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v2")]
public class AccountsV2Controller : ControllerBase
{
	private readonly IServiceManager _service;

	public AccountsV2Controller(IServiceManager service) => _service = service;

	[HttpGet]
	[Route("get_all_accounts_from_dbV2", Name = "GetAllAccountsV2")]
	public async Task<IActionResult> GetAllAccountsFromDb()
	{
		var accounts = await _service.AccountService.GetAllAccountsFromDb(trackChanges: false);

		//foreach (var account in accounts)
		//{
		//	account.FirstName = account.FirstName + "-"+"V2";	
		//}

		var accountsV2 = accounts.Select(x => $"{ x.FirstName} -V2");

		return Ok(accountsV2);
	}
}
