using Contracts;
using Entities.Models;

namespace Repository;

internal sealed class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
	public CompanyRepository(RepositoryContext repositoryContext)
		: base(repositoryContext)
	{
	}

	public void CreateCompany(Company company) => CreateCompany(company);

    public void DeleteCompany(Company company) => DeleteCompany(company);

    public IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
		FindAll(trackChanges)
		.OrderBy(c => c.Name)
		.ToList();

    public IEnumerable<Company> GetCompaniesByIds(IEnumerable<Guid> ids, bool trackChanges)
    {
		return FindByCondition(c => ids.Contains(c.Id), trackChanges)
			.ToList();
    }

    public Company GetCompany(Guid companyId, bool trackChanges) =>
		FindByCondition(c => c.Id.Equals(companyId), trackChanges)
		.SingleOrDefault();
}
