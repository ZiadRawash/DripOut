using AutoMapper;
using DripOut.Application.DTOs;
using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Models;
using DripOut.Domain.Models.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DripOut.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _prdService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;

        public ProductController(IProductService prdService, SignInManager<AppUser> signInManager
            , IMapper mapper)
        {
            _prdService = prdService;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search = "", int crntPage = 1, int pageSize = 10)
            => Ok(await _prdService.GetAllAsync(search,0, crntPage, pageSize));

        [HttpGet("{id}")]
        public async Task<IActionResult> FindAsync(int id)
        {
            var product = await _prdService.FindAsync(p => p.Id == id, p => p.Category!, p => p.Reviews!);
            foreach (var rev in product.Reviews!)
            {
                rev.User = await _signInManager.UserManager.FindByIdAsync(rev.AppUserId);
            }
            if (product == null)
                return NotFound("No Such Id");

            return Ok(_mapper.Map<ProductsDTO>(product));
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(Product product)
        {
            if (product == null)
                return BadRequest();
            var newProduct = await _prdService.AddAsync(product);
            return CreatedAtAction(nameof(FindAsync), new { id = newProduct.Id }, newProduct);
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Put(int id, Product product)
        //{
        //    if (id != product.Id)
        //        return BadRequest();
        //    var updatedProduct = await _repo.UpdateAsync(product)!;
        //    if (updatedProduct == null)
        //        return NotFound();
        //    return NoContent();
        //}
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var product = await _repo.FindAsync(id)!;
        //    if (product == null)
        //        return NotFound();
        //    await _repo.DeleteAsync(product);
        //    return NoContent();
        //}
    }
}
