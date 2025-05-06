using EnterpriseAppLINQ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EnterpriseAppLINQ.Data;

namespace EnterpriseAppLINQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para obtener todos los productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products
                .Select(p => new 
                {
                    p.ProductId,
                    p.Name,
                    Description = p.Description ?? "Sin descripción",  // Usamos un valor por defecto
                    Price = p.Price != null ? p.Price : 0  // Aseguramos que el precio no sea null
                })
                .ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No se encontraron productos.");
            }

            var result = products.Select(p => new Product
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price
            }).ToList();

            return result;
        }

        // Método para obtener productos cuyo precio es mayor a un valor específico
        [HttpGet("search/price/{value}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByPrice(decimal value)
        {
            var products = await _context.Products
                .Where(p => p.Price > value)  // Filtrar por el precio
                .Select(p => new 
                {
                    p.ProductId,
                    p.Name,
                    Description = p.Description ?? "Sin descripción",  // Usamos un valor por defecto
                    Price = p.Price != null ? p.Price : 0  // Aseguramos que el precio no sea null
                })
                .ToListAsync();  

            if (products == null || !products.Any())
            {
                return NotFound($"No se encontraron productos con un precio mayor a '{value}'");
            }

            var result = products.Select(p => new Product
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price
            }).ToList();

            return result;
        }

        // Método para obtener el producto más caro
        [HttpGet("mostExpensive")]
        public async Task<ActionResult<Product>> GetMostExpensiveProduct()
        {
            var product = await _context.Products
                .OrderByDescending(p => p.Price)  // Ordenar los productos de mayor a menor precio
                .Select(p => new 
                {
                    p.ProductId,
                    p.Name,
                    Description = p.Description ?? "Sin descripción",  // Usamos un valor por defecto
                    Price = p.Price != null ? p.Price : 0  // Aseguramos que el precio no sea null
                })
                .FirstOrDefaultAsync();  

            if (product == null)
            {
                return NotFound("No se encontraron productos.");
            }

            var result = new Product
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            };

            return result;
        }

        // Método para obtener el promedio de precios de los productos
        [HttpGet("averagePrice")]
        public async Task<ActionResult<decimal>> GetAveragePrice()
        {
            var averagePrice = await _context.Products
                .Where(p => p.Price != null)  
                .AverageAsync(p => p.Price);  

            if (averagePrice == 0)
            {
                return NotFound("No se encontraron productos.");
            }

            return Ok(averagePrice);  
        }

        // Método para obtener todos los productos que no tienen descripción
        [HttpGet("noDescription")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsNoDescription()
        {
            var products = await _context.Products
                .Where(p => string.IsNullOrEmpty(p.Description))  // Filtrar productos cuya descripción sea nula o vacía
                .ToListAsync();  // Convertir el resultado a una lista

            if (products == null || !products.Any())
            {
                return NotFound("No se encontraron productos sin descripción.");
            }

            return products;
        }
    }
}   
