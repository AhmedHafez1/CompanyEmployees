using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryManager
    {
        public IEmployeeRepository Employee { get; }
        public ICompanyRepository Company { get; }

        void Save();
    }
}
