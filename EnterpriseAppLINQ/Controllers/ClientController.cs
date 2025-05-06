using EnterpriseAppLINQ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EnterpriseAppLINQ.Data;

namespace EnterpriseAppLINQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para obtener todos los clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients.ToListAsync();
        }

        // Método para obtener un cliente por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        // Método para obtener clientes cuyo nombre contiene un valor específico
        [HttpGet("search/{name}")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClientsByName(string name)
        {
            var clients = await _context.Clients
                .Where(c => c.Name.Contains(name))
                .ToListAsync();

            if (clients == null || !clients.Any())
            {
                return NotFound($"No se encontraron clientes con el nombre que contiene '{name}'");
            }

            return clients;
        }

        // Método para obtener el cliente con mayor número de pedidos
        [HttpGet("mostOrders")]
        public async Task<ActionResult<Client>> GetClientWithMostOrders()
        {
            var clientWithMostOrders = await _context.Orders
                .GroupBy(o => o.ClientId) 
                .Select(g => new
                {
                    ClientId = g.Key,
                    OrderCount = g.Count()
                })
                .OrderByDescending(g => g.OrderCount)
                .FirstOrDefaultAsync();

            if (clientWithMostOrders == null)
            {
                return NotFound("No se encontraron clientes.");
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.ClientId == clientWithMostOrders.ClientId);

            if (client == null)
            {
                return NotFound("Cliente no encontrado.");
            }

            return client;  
        }

        // Método para obtener todos los clientes que han comprado un producto específico
        [HttpGet("purchasedProduct/{productId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetClientsWhoPurchasedProduct(int productId)
        {
            var clients = await _context.OrderDetails
                .Where(od => od.ProductId == productId)  // Filtrar por ProductId
                .Select(od => new 
                {
                    ClientName = od.Order.Client.Name  // Proyectar el nombre del cliente
                })
                .Distinct()  // Asegurarse de que los nombres de los clientes sean únicos
                .ToListAsync();  // Convertir el resultado a una lista

            if (clients == null || !clients.Any())
            {
                return NotFound($"No se encontraron clientes que hayan comprado el producto con ProductId {productId}");
            }

            return clients;
        }

        // Método para actualizar un cliente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            if (id != client.ClientId)
            {
                return BadRequest();
            }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Método para crear un cliente
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClient", new { id = client.ClientId }, client);
        }

        // Método para eliminar un cliente
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
        }
    }
}
