using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Shared.DataTranferObjects;
using System.Text;

namespace MelvinBankApi;

public class CsvOutputFormatter : TextOutputFormatter
{
	public CsvOutputFormatter()
	{
		SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
		SupportedEncodings.Add(Encoding.UTF8);
		SupportedEncodings.Add(Encoding.Unicode);
	}

	protected override bool CanWriteType(Type? type)
	{
		if (typeof(GetAccountDto).IsAssignableFrom(type) || typeof(IEnumerable<GetAccountDto>).IsAssignableFrom(type))
		{
			return base.CanWriteType(type);
		}

		return false;
	}

	public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
	{
		var response = context.HttpContext.Response;
		var buffer = new StringBuilder();

		if (context.Object is IEnumerable<GetAccountDto>)
		{
			foreach (var account in (IEnumerable<GetAccountDto>)context.Object)
			{
				FormatCsv(buffer, account);
			}
		}
		else
		{
			FormatCsv(buffer, (GetAccountDto)context.Object);
		}

		await response.WriteAsync(buffer.ToString());
	}

	private static void FormatCsv(StringBuilder buffer, GetAccountDto account)
	{
		buffer.AppendLine($"{account.Id}, \"{account.FirstName}, \"{account.AccountName}\"");
	}
}
