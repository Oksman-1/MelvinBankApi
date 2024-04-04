using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestFeatures;

public class AccountsParameters : RequestParameters
{
	public AccountsParameters() => OrderBy = "name";


	public decimal MinCurrentAccountBalance { get; set; } = 10000.0M;
	public decimal MaxCurrentAccountBalance { get; set; } = decimal.MaxValue;
	//public double HotelRating { get; set; }
	public string? SearchTerm { get; set; }

	public bool ValidCurrentAccountRange => MaxCurrentAccountBalance > MinCurrentAccountBalance;
}
