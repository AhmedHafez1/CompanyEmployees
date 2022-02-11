using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service;

internal sealed class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, EmployeeParameters parameters, bool trackChanges)
    {
        if (!parameters.ValidAgeRange)
            throw new MaxAgeRangeBadRequestException();

        await CheckIfCompanyExists(companyId, trackChanges);

        var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, parameters, trackChanges);
        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

        return employeesDto;
    }

    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);

        Employee? employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);

        var employee = _mapper.Map<EmployeeDto>(employeeDb);
        return employee;
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);

        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
        employeeEntity.CompanyId = companyId;

        _repository.Employee.CreateEmployee(employeeEntity);
        await _repository.SaveAsync();

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

        return employeeToReturn;
    }

    public async Task DeleteEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);

        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, trackChanges);

        _repository.Employee.DeleteEmployee(employee);
        await _repository.SaveAsync();
    }

    public async Task UpdateEmployeeAsync(Guid companyId, Guid employeeId, EmployeeForUpdateDto employeeForUpdate, bool companyTrackChanges, bool employeeTrackChanges)
    {
        await CheckIfCompanyExists(companyId, companyTrackChanges);

        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, employeeTrackChanges);

        _mapper.Map(employeeForUpdate, employee);
        await _repository.SaveAsync();
    }

    public async Task<(Employee employeeEntity, EmployeeForUpdateDto employeeToPatch)> GetEmplyeeFroPatchUpdateAsync(Guid companyId, Guid employeeId, bool companyTrackChanges, bool employeeTrackChanges)
    {
        await CheckIfCompanyExists(companyId, companyTrackChanges);

        var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, employeeTrackChanges);

        var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);

        return (employeeEntity, employeeToPatch);

    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.Map(employeeToPatch, employeeEntity);
        await _repository.SaveAsync();
    }

    private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);
    }

    private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
    {
        var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
        if (employeeDb is null)
            throw new EmployeeNotFoundException(id);
        return employeeDb;
    }
}
