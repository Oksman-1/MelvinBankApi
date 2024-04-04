using Entities.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelvinBankApi.Presentation;

[Route("api")]
[ApiController]
public class RootController : ControllerBase
{
	private readonly LinkGenerator _linkGenerator;

	public RootController(LinkGenerator linkGenerator) => _linkGenerator = linkGenerator;

	[HttpGet(Name = "GetRoot")]
	public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
	{
		if (mediaType.Contains("application/vnd.melvinbank.apiroot"))
		{
			var list = new List<Link>
			{
				new Link
				{
					Href = _linkGenerator.GetUriByName(HttpContext, nameof(GetRoot), new {}),
					Rel = "self",
					Method = "GET"
				},
				new Link
				{
					Href = _linkGenerator.GetUriByName(HttpContext, "GetAllAccounts", new {}),
					Rel = "accounts",
					Method = "GET"
				},
				new Link
				{
					Href = _linkGenerator.GetUriByName(HttpContext, "RegisterNewAccount", new {}),
					Rel = "create_account",
					Method = "POST"
				},
				new Link
				{
					Href = _linkGenerator.GetUriByName(HttpContext, "Authenticate", new {}),
					Rel = "authenticate_account",
					Method = "POST"
				},
				new Link
				{
					Href = _linkGenerator.GetUriByName(HttpContext, "UpdateAccount", new {}),
					Rel = "update_account",
					Method = "PUT"
				}
			};

			return Ok(list);
		}

		return NoContent();
	}
}

