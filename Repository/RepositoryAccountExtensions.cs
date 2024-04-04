using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Repository.Extensions.Utility;

namespace Repository.Extensions;

public static class RepositoryAccountExtensions
{
	public static IQueryable<Account> FilterAccounts(this IQueryable<Account> accounts, decimal MinCurrentAccountBalance, decimal MaxCurrentAccountBalance) =>
		accounts.Where(e => (e.CurrentAccountBalance >= MinCurrentAccountBalance && e.CurrentAccountBalance <= MaxCurrentAccountBalance));

	public static IQueryable<Account> Search(this IQueryable<Account> accounts, string searchTerm)
	{
		if (string.IsNullOrWhiteSpace(searchTerm))
			return accounts;

		var lowerCaseTerm = searchTerm.Trim().ToLower();

		return accounts.Where(e => e.FirstName.ToLower().Contains(lowerCaseTerm));
	}

	public static IQueryable<Account> Sort(this IQueryable<Account> accounts, string orderByQueryString)
	{
		if (string.IsNullOrWhiteSpace(orderByQueryString))
			return accounts.OrderBy(e => e.FirstName);

		var orderQuery = OrderQueryBuilder.CreateOrderQuery<Account>(orderByQueryString);

		if (string.IsNullOrWhiteSpace(orderQuery))
			return accounts.OrderBy(e => e.FirstName);

		return accounts.OrderBy(orderQuery);
	}
}
