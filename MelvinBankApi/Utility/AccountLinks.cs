using Contracts;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.Net.Http.Headers;
using Shared.DataTranferObjects;


namespace MelvinBankApi.Utility;

public class AccountLinks : IAccountLinks
{
	private readonly LinkGenerator _linkGenerator;

	private readonly IDataShaper<GetAccountDto> _dataShaper;

	public Dictionary<string, MediaTypeHeaderValue> AcceptHeader { get; set; } = new Dictionary<string, MediaTypeHeaderValue>();

	public AccountLinks(LinkGenerator linkGenerator, IDataShaper<GetAccountDto> dataShaper)
	{
		_linkGenerator = linkGenerator;
		_dataShaper = dataShaper;
	}

	public LinkResponse TryGenerateLinks(IEnumerable<GetAccountDto> accountsDto, string fields, /*string AccountNumber, */HttpContext httpContext)
	{
		var shapedAccounts = ShapeData(accountsDto, fields);

		if (ShouldGenerateLinks(httpContext))
			return ReturnLinkdedAccounts(accountsDto, fields, /*AccountNumber,*/ httpContext, shapedAccounts);

		return ReturnShapedAccounts(shapedAccounts);
	}

	private List<Entity> ShapeData(IEnumerable<GetAccountDto> accountsDto, string fields) => _dataShaper.ShapeData(accountsDto, fields)
			.Select(e => e.Entity)
			.ToList();


	private bool ShouldGenerateLinks(HttpContext httpContext)
	{
		var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];

		return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
	}

	private LinkResponse ReturnShapedAccounts(List<Entity> shapedAccounts) => new LinkResponse { ShapedEntities = shapedAccounts };

	private LinkResponse ReturnLinkdedAccounts(IEnumerable<GetAccountDto> accountsDto,
	string fields, /*string AccountNumber,*/ HttpContext httpContext, List<Entity> shapedAccounts)
	{
		var accountDtoList = accountsDto.ToList();

		for (var index = 0; index < accountDtoList.Count(); index++)
		{
			var accountLinks = CreateLinksForAccount(httpContext, /*AccountNumber,*/ accountsDto, accountDtoList[index].Id, fields);
			shapedAccounts[index].Add("Links", accountLinks);
		}

		var accountCollection = new LinkCollectionWrapper<Entity>(shapedAccounts);
		var linkedAccounts = CreateLinksForAccounts(httpContext, accountCollection);

		return new LinkResponse { HasLinks = true, LinkedEntities = linkedAccounts };
	}

	private List<Link> CreateLinksForAccount(HttpContext httpContext,/* string AccountNumber,*/ IEnumerable<GetAccountDto> account, int Id, string fields = "")
	{
		var links = new List<Link>
			{
				new Link(_linkGenerator.GetUriByAction(httpContext, "GetByAccountNumber", values: new { /*AccountNumber,*/ fields }),
				"self",
				"GET"),
				new Link(_linkGenerator.GetUriByAction(httpContext, "DeleteAccount", values: new { Id }),
				"delete_account",
				"DELETE"),
				//new Link(_linkGenerator.GetUriByAction(httpContext, "UpdateAccount", values: new { account }),
				//"update_account",
				//"PUT"),
				//new Link(_linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateHotelForCountry", values: new { companyId, HotelId }),
				//"partially_update_hotel",
				//"PATCH")
			};
		return links;
	}

	private LinkCollectionWrapper<Entity> CreateLinksForAccounts(HttpContext httpContext, LinkCollectionWrapper<Entity> accountsWrapper)
	{
		accountsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, "GetAllAccounts", values: new { }),
				"self",
				"GET"));

		return accountsWrapper;
	}
} 