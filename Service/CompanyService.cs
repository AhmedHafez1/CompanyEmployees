using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class CompanyService : ICompanyService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async  Task<CompanyDto> CreateCompany(CompanyForCreationDto company)
    {
        var companyEntity = _mapper.Map<Company>(company);

        _repository.Company.CreateCompany(companyEntity);
        await _repository.SaveAsync();

        var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

        return companyToReturn;
    }

    public Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company)
    {
        throw new NotImplementedException();
    }

    public async Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollection)
    {
        if (companyCollection is null)
            throw new CompanyCollectionBadRequest();

        var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);

        foreach (var companyEntity in companyEntities)
        {
            _repository.Company.CreateCompany(companyEntity);
        }

        await _repository.SaveAsync();

        var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
        var ids = string.Join(',', companiesToReturn.Select(c => c.Id));
        return (companies: companiesToReturn, ids: ids);

    }

    public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);

        if (company is null)
            throw new CompanyNotFoundException(companyId);

        _repository.Company.DeleteCompany(company);
        await _repository.SaveAsync();
    }

    public async Task<IEnumerable<CompanyDto>> GetAllCompanies(bool trackChanges)
    {
        var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges);

        var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

        return companiesDto;
    }

    public Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CompanyDto>> GetCompaniesByIds(IEnumerable<Guid> ids, bool trackChanges)
    {
        if (ids is null)
            throw new IdParametersBadRequestException();

        var companniesEntities = await _repository.Company.GetCompaniesByIdsAsync(ids, trackChanges);

        if (companniesEntities.Count() != ids.Count())
            throw new CollectionByIdsBadRequestException();

        var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companniesEntities);
        return companiesToReturn;
    }

    public Task<IEnumerable<CompanyDto>> GetCompaniesByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        throw new NotImplementedException();
    }

    public async Task<CompanyDto> GetCompanyAsync(Guid id, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(id, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(id);

        var companyDto = _mapper.Map<CompanyDto>(company);
        return companyDto;
    }

    public async Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null) throw new CompanyNotFoundException(companyId);

        _mapper.Map(companyForUpdate, company);
        await _repository.SaveAsync();
    }
}
