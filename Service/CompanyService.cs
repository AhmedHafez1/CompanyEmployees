using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Contracts;
using Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    internal sealed class CompanyService : ICompanyService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;

        public CompanyService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges)
        {
            try
            {
                var companies = _repository.Company
                    .GetAllCompanies(trackChanges)
                    .Select(c => new CompanyDto(c.Id, c.Name ?? "", string.Join(' ', c.Address, c.Country)))
                    .ToList();
                return companies;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong in the " +
                    $"{nameof(GetAllCompanies)} service method {ex}");
                throw;
            }
        }
    }
}
