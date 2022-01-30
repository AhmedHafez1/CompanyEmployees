using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

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

	public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
	{
		var company = _repository.Company.GetCompany(companyId, trackChanges);
		if (company is null)
			throw new CompanyNotFoundException(companyId);

		var employeesFromDb = _repository.Employee.GetEmployees(companyId, trackChanges);
		var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

		return employeesDto;
	}

	public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
	{
		var company = _repository.Company.GetCompany(companyId, trackChanges);
		if (company is null)
			throw new CompanyNotFoundException(companyId);

		var employeeDb = _repository.Employee.GetEmployee(companyId, id, trackChanges);
		if (employeeDb is null)
			throw new EmployeeNotFoundException(id);

		var employee = _mapper.Map<EmployeeDto>(employeeDb);
		return employee;
	}

    public EmployeeDto CreateEmployee(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);

		if (company is null) throw new CompanyNotFoundException(companyId);

		var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
		employeeEntity.CompanyId = companyId;

		_repository.Employee.CreateEmployee(employeeEntity);
		_repository.Save();

		var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

		return employeeToReturn;
    }

    public void DeleteEmployee(Guid companyId, Guid employeeId, bool trackChanges)
    {
		var company = _repository.Company.GetCompany(companyId, trackChanges);

		if (company is null)
			throw new CompanyNotFoundException(companyId);

		var employee = _repository.Employee.GetEmployee(companyId, employeeId, trackChanges);
		if (employee is null)
			throw new EmployeeNotFoundException(employeeId);

		_repository.Employee.DeleteEmployee(employee);
		_repository.Save();
    }

    public void UpdateEmployee(Guid companyId, Guid employeeId, EmployeeForUpdateDto employeeForUpdate, bool companyTrackChanges, bool employeeTrackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, companyTrackChanges);
		if(company is null) throw new CompanyNotFoundException(companyId);

		var employee = _repository.Employee.GetEmployee(companyId, employeeId, employeeTrackChanges);
		if(employee is null) throw new EmployeeNotFoundException(employeeId);

		_mapper.Map(employeeForUpdate, employee);
		_repository.Save();
    }

    public (Employee employeeEntity, EmployeeForUpdateDto employeeToPatch) GetEmplyeeFroPatchUpdate(Guid companyId, Guid employeeId, bool companyTrackChanges, bool employeeTrackChanges)
    {
		var company = _repository.Company.GetCompany(companyId, companyTrackChanges);
		if (company is null) throw new CompanyNotFoundException(companyId);

		var employeeEntity = _repository.Employee.GetEmployee(companyId, employeeId, employeeTrackChanges);
		if (employeeEntity is null) throw new EmployeeNotFoundException(employeeId);

		var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);

		return (employeeEntity, employeeToPatch);

	}

    public void SaveChangesForPatch(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.Map(employeeToPatch, employeeEntity);
		_repository.Save();
    }
}
