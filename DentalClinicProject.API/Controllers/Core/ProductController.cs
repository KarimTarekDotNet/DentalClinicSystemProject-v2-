using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinicProject.API.Controllers.Core
{
    public class ProductController : BaseController
    {
        public ProductController(IUnitOfWork work) : base(work)
        {
        }

        [HttpGet("with-ids")]
        public async Task<IActionResult> GetProductsByIds([FromQuery] string ids)
        {
            try
            {
                var idList = ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id, out var x) ? x : (int?)null)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToList();

                if (!idList.Any())
                    return BadRequest(new { success = false, message = "No valid IDs provided." });

                var products = await work.ProductRepository.GetProductsByIdsAsync(idList);

                if (products == null || !products.Any())
                    return NotFound(new { success = false, message = "Products not found." });

                return Ok(new { success = true, data = products });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while getting products.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetProductsWithDetails([FromQuery] PaginationParams param)
        {
            try
            {
                var pagedResult = await work.ProductRepository.GetProductsWithDetailsAsync(param);
                if (!pagedResult.Items.Any())
                    return NotFound(new { success = false, message = "No products found." });
                return Ok(new { success = true, data = pagedResult });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while getting products.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("with-details")]
        public async Task<IActionResult> GetProductWithDetails(int id)
        {
            try
            {
                var product = await work.ProductRepository.GetProductWithDetailsAsync(id);
                if (product == null)
                    return NotFound(new { success = false, message = "Product not found." });
                return Ok(new { success = true, data = product });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while getting the product.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO dto)
        {
            try
            {
                var createdProduct = await work.ProductRepository.CreateProductAsync(dto);
                return Ok(new { success = true, data = createdProduct });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while creating the product.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await work.ProductRepository.DeleteProductAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "Product not found." });
                return Ok(new { success = true, message = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while deleting the product.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDTO dto)
        {
            try
            {
                var updatedProduct = await work.ProductRepository.UpdateProductAsync(dto);
                return Ok(new { success = true, data = updatedProduct });
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(new { success = false, message = knfEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while updating the product.",
                    error = ex.Message
                });
            }
        }
    }
}