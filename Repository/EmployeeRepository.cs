using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;

namespace Repository;

internal sealed class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters parameters , bool trackChanges) =>
        await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
        .FilterEmployees(parameters.MinAge, parameters.MaxAge)
        .Search(parameters.SearchTerm)
        .OrderBy(e => e.Name)
        .Sort(parameters.OrderBy)
        .Skip((parameters.PageNumber - 1) * parameters.PageSize)
        .Take(parameters.PageSize)
        .ToListAsync();

    public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
        await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
        .SingleOrDefaultAsync();

    public void CreateEmployee(Employee employee)
    {
        Create(employee);
    }

    public void DeleteEmployee(Employee employee) => Delete(employee);
}
