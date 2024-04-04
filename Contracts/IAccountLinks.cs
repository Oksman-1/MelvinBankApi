using Entities.LinkModels;
using Microsoft.AspNetCore.Http;
using Shared.DataTranferObjects;
namespace Contracts;


public interface IAccountLinks
{
	LinkResponse TryGenerateLinks(IEnumerable<GetAccountDto> accountsDto, string fields, /*string AccountNumber,*/ HttpContext httpContext);
}
