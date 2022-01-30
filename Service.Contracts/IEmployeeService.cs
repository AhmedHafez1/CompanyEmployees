using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;

namespace Service.Contracts;

public interface IEmployeeService
{
    IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges);
    EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges);
    EmployeeDto CreateEmployee(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges);
    void DeleteEmployee(Guid companyId, Guid employeeId, bool trackChanges);

    void UpdateEmployee(Guid companyId, Guid employeeId, EmployeeForUpdateDto employeeForUpdate, bool companyTrackChanges, bool employeeTrackChanges);
}
