using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
	protected MelvinBankContext _melvinBankContext;

	protected RepositoryBase(MelvinBankContext melvinBankContext)
	{
		_melvinBankContext = melvinBankContext;
	}

	public void Create(T entity) => _melvinBankContext.Set<T>().Add(entity);

	public EntityState DbEntityState(T entity) => _melvinBankContext.Entry(entity).State;

	public EntityState DbEntityState(Account entity) => _melvinBankContext.Entry(entity).State;	

	public void Delete(T entity) => _melvinBankContext.Set<T>().Remove(entity);

	public IQueryable<T> FindAll(bool trackChanges) =>
		!trackChanges ? _melvinBankContext.Set<T>().AsNoTracking()
		: _melvinBankContext.Set<T>();

	public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges) => !trackChanges ? _melvinBankContext.Set<T>()
		.Where(expression)
		.AsNoTracking() : _melvinBankContext.Set<T>()
		.Where(expression);

	public void Update(T entity) => _melvinBankContext.Set<T>().Update(entity);

}
