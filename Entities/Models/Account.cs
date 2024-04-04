using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models;

public enum AccountType
{
	Savings,
	Current,
	Corporate,
	Government
}

[Table("Accounts")]
public class Account
{
	[Key]
	public int Id { get; set; }

	//Personal Info
	[Required(ErrorMessage = "FirstName is a required field.")]
	public string? FirstName { get; set; }

	[Required(ErrorMessage = "LastName is a required field.")]
	public string? LastName { get; set; }
	public string? AccountName { get; set; }

	[MaxLength(11, ErrorMessage = "Maximum Length for the PhoneNumber is 11 Characters")]
	public string? PhoneNumber { get; set; }

	[Required(ErrorMessage = "Email is a required field.")]
	public string? Email { get; set;}

	//Account Info
	public decimal CurrentAccountBalance { get; set; }
	public AccountType AccountType { get; set;}//This is the account type (Savings or Current).
	public string? AccountNumberGenerated { get; set; }	

	//To store the Hash and Salt of the Account Transaction Pin
	public byte[]? PinHash { get; set; }
	public byte[]? PinSalt { get; set; }
	public DateTime DateCreated { get; set; }
	public DateTime DateLastUpdated { get; set; }

	//To Generate Account Number in Constructor
    public Account()
    {
		AccountNumberGenerated = Convert.ToString((long)Math.Floor(rand.NextDouble() * 9_000_000_000L + 1_000_000_000L));
		//Set Account name
		AccountName = $"{FirstName} {LastName}";

	}

	//Create a Random Object
	Random rand = new Random();

}


