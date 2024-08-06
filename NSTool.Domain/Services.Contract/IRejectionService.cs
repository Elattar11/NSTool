using NSTool.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTool.Domain.Services.Contract
{
    public interface IRejectionService
    {
        Task<List<RejectionData>> ReadAndFilterExcelFile(Stream fileStream);
    }
}
