using Entities.Models;

namespace Contracts;

public interface ICompanyRepository
{
    IEnumerable<Company> GetAllCompanies(bool trackChanges);
    Company GetCompany(Guid companyId, bool trackChanges);
    void CreateCompany(Company company);
    IEnumerable<Company> GetCompaniesByIds(IEnumerable<Guid> ids, bool trackChanges);
    void DeleteCompany(Company company);
}
