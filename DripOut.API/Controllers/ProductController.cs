﻿using AutoMapper;
using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Products;
using DripOut.Application.Helpers;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Mappers;
using DripOut.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace DripOut.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] QueryModel queryModel)
        {
            var productsPage = await _unitOfWork.Products.GetAllAsync(queryModel);
            var productsDTO = productsPage.MapToProductDTO();
            if (productsPage.Items == null)
                return NotFound("No Products Found");
            return Ok(productsDTO);
        }
            

        [HttpGet("{id}")]
        public async Task<IActionResult> FindAsync(int id)
        {
            var product = await _unitOfWork.Products.FindAsync(p=>p.Id == id,
                p => p.Variants!,
                p => p.Images!,
                p => p.Reviews!);
            if (product == null)
                return NotFound("No Such Id");
            return Ok(product);
        }
        

        [HttpPost]
        public async Task<IActionResult> AddAsync(ProductInputDTO productDTO)
        {
            if (productDTO == null)
                return BadRequest();
            var product = productDTO.MapToProduct();
            await _unitOfWork.Products.AddAsync(product);
            return Created();
        }

        [HttpPost("Size")]
        public async Task<IActionResult> AddSizeAsync(VariantInputDTO variantDTO)
        {
            var variant = variantDTO.MapToProductVariant();
            var product = await _unitOfWork.Products.FindAsync(p => p.Id == variant.ProductId);
            if (product == null)
                return NotFound("No Such Id");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            product.Amount += variant.StockQuantity;
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.Variants.AddAsync(variant);
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, ProductInputDTO inputProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var product = await _unitOfWork.Products.FindAsync(id);
            if (product is null)
                return NotFound();
            product.UpdateProduct(inputProduct);
            await _unitOfWork.Products.UpdateAsync(product)!;
            return Created();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Products.FindAsync(id)!;
            if (product == null)
                return NotFound();
            await _unitOfWork.Products.DeleteAsync(product);
            return NoContent();
        }
    }
}
