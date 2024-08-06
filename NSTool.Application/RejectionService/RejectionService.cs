using NSTool.Domain.Entities;
using NSTool.Domain.Services.Contract;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTool.Application.RejectionService
{
    public class RejectionService : IRejectionService
    {
        public async Task<List<RejectionData>> ReadAndFilterExcelFile(Stream fileStream)
        {
            var data = new List<RejectionData>();
            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;

                var headers = new Dictionary<string, int>();
                for (int col = 1; col <= colCount; col++)
                {
                    headers[worksheet.Cells[1, col].Text] = col;
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    string bsRejection = null;
                    string creator = null;
                    string itRejection = null;
                    string subArea = null;
                    string ownerTeam = null;
                    string status = null;

                    if (headers.TryGetValue("BS Rejection Reason", out var bsRejectionColumnIndex))
                    {
                        bsRejection = worksheet.Cells[row, bsRejectionColumnIndex].Text;
                    }

                    if (headers.TryGetValue("Creator", out var creatorColumnIndex))
                    {
                        creator = worksheet.Cells[row, creatorColumnIndex].Text;
                    }

                    if (headers.TryGetValue("Sub Area", out var subAreaColumnIndex))
                    {
                        subArea = worksheet.Cells[row, subAreaColumnIndex].Text;
                    }

                    if (headers.TryGetValue("Owner Team", out var ownerTeamColumnIndex))
                    {
                        ownerTeam = worksheet.Cells[row, ownerTeamColumnIndex].Text;
                    }

                    if (headers.TryGetValue("IT Rejection Reason", out var itRejectionColumnIndex))
                    {
                        itRejection = worksheet.Cells[row, itRejectionColumnIndex].Text;
                    }

                    if (headers.TryGetValue("Status", out var statusColumnIndex))
                    {
                        status = worksheet.Cells[row, statusColumnIndex].Text;
                    }

                    if (status.Equals("Rejected", StringComparison.OrdinalIgnoreCase) ||
                        (!string.IsNullOrWhiteSpace(bsRejection) && status.Equals("Closed", StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(itRejection) && status.Equals("Closed", StringComparison.OrdinalIgnoreCase)))
                    {
                        var excelData = new RejectionData
                        {
                            SR = headers.ContainsKey("SR #") ? worksheet.Cells[row, headers["SR #"]].Text : null,
                            Creator = creator,
                            Status = status,
                            SubArea = subArea,
                            OwnerTeam = ownerTeam,
                            BS_Rejection = bsRejection,
                            IT_Rejection = itRejection,
                            Justification = string.Empty
                            // Map other properties as needed
                        };

                        data.Add(excelData);
                    }
                }
            }
            return data;

        }
    }
}
