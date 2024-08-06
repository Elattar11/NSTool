using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSTool.Domain.Entities;
using NSTool.Domain.Services.Contract;

namespace NSTool.APIs.Controllers
{
    
    public class RejectionController : BaseApiController
    {
        private readonly IRejectionService _rejectionService;
        private readonly IFirebaseService _firebaseService;

        public RejectionController(IRejectionService rejectionService ,IFirebaseService firebaseService)
        {
            _rejectionService = rejectionService;
            _firebaseService = firebaseService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var newData = await _rejectionService.ReadAndFilterExcelFile(stream);

                // Retrieve existing data from Firebase with numeric keys
                var existingData = await _firebaseService.GetAllDataWithKeysAsync<RejectionData>("data-path");

                // Create a set of existing unique identifiers (e.g., SR) for quick lookup
                var existingKeys = existingData.Values.Select(d => d.SR).ToHashSet();

                // Filter out new data that already exists
                var uniqueNewData = newData.Where(item => !existingKeys.Contains(item.SR)).ToList();

                // Add new unique data with unique keys
                int nextKey = existingData.Keys.Select(k => int.Parse(k)).DefaultIfEmpty(-1).Max() + 1;
                foreach (var item in uniqueNewData)
                {
                    var key = nextKey.ToString();
                    await _firebaseService.InsertDataWithKeyAsync("data-path", key, item);
                    nextKey++;
                }

                // Return the updated data
                var updatedData = await _firebaseService.GetAllDataWithKeysAsync<RejectionData>("data-path");

                return Ok(new
                {
                    Added = uniqueNewData.Count,
                    Total = updatedData.Count,
                    Data = updatedData.Values.ToList()
                });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllRejectionData()
        {
            var data = await _firebaseService.GetAllDataAsync<RejectionData>("data-path");
            if (data == null || !data.Any())
            {
                return NotFound("No data found.");
            }

            return Ok(data);
        }

        [HttpPost("updateJustification")]
        public async Task<IActionResult> UpdateJustification([FromBody] UpdateJustificationRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.SR) || string.IsNullOrEmpty(request.NewJustification))
            {
                return BadRequest("Invalid request.");
            }

            // Retrieve existing data from Firebase with numeric keys
            var existingData = await _firebaseService.GetAllDataWithKeysAsync<RejectionData>("data-path");

            // Find the item with the matching SR
            var itemToUpdate = existingData.Values.FirstOrDefault(d => d.SR == request.SR);

            if (itemToUpdate == null)
            {
                return NotFound("Item not found.");
            }

            // Update the Justification field
            itemToUpdate.Justification = request.NewJustification;

            // Save the updated item back to Firebase
            await _firebaseService.InsertDataWithKeyAsync("data-path", existingData.FirstOrDefault(x => x.Value == itemToUpdate).Key, itemToUpdate);

            return Ok("Justification updated successfully.");
        }

        [HttpGet("getBySR/{sr}")]
        public async Task<IActionResult> GetBySR(string sr)
        {
            if (string.IsNullOrEmpty(sr))
            {
                return BadRequest("Invalid SR.");
            }

            // Retrieve existing data from Firebase with numeric keys
            var existingData = await _firebaseService.GetAllDataWithKeysAsync<RejectionData>("data-path");

            // Find the item with the matching SR
            var item = existingData.Values.FirstOrDefault(d => d.SR == sr);

            if (item == null)
            {
                return NotFound("Item not found.");
            }

            return Ok(item);
        }

        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAllData()
        {
            try
            {
                // Delete all data from the specified path in Firebase
                await _firebaseService.DeleteDataAsync("data-path");

                return Ok("All data deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
