using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repository.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Repository;

public class MelvinBankContext : IdentityDbContext<User>
{
	public MelvinBankContext(DbContextOptions options) : base(options)
	{


	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		//modelBuilder.ApplyConfiguration(new CompanyConfiguration());
		//modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
		modelBuilder.ApplyConfiguration(new RoleConfiguration());

	}

	public DbSet<Account>? Accounts { get; set; }
	public DbSet<Entities.Models.Transaction>? Transactions { get; set; }
}
