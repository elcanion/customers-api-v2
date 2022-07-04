using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PloomesTest.Models;
using PloomesTest.Data;
using System.Reflection;
using AutoMapper;
using System.Net.Mail;


namespace PloomesTest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly DataContext _context;

        public CustomerController(DataContext context)
        {
            _context = context;
        }

        // GET /customers
        /// <summary>
        /// Lists all registered customers.
        /// </summary>
        /// <returns>All customers</returns>
        /// <response code="200">Returns all customers</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<CustomerDTO>>> Read()
        {
            var result = await _context.Customers
                .Include("Address")
                .ToListAsync();
            return Ok(result);
        }

        // GET /customer/{id}
        /// <summary>
        /// Find a specific customer.
        /// </summary>
        /// <param name="id">Required parameter</param>
        /// <returns>A new customer</returns>
        /// <response code="200">Returns a specific Customer</response>
        /// <response code="400">Customer couldn't be found</response>  
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> Read(int id)
        {
            var customer = await _context.Customers
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (customer is null) return BadRequest("Customer not found.");
            return Ok(customer);
        }
          
        // POST /customer
        /// <summary>
        /// Creates a new customer.
        /// </summary>
        /// <remarks>
        /// Example:
        ///
        ///     POST /customer
        ///     {
        ///        "id": 0,
        ///        "name": "Yuki",
        ///        "email": "yuki@gmail.com",
        ///        "phone": "(61)99999-9999",
        ///        "addressId": 0
        ///     }
        ///
        /// </remarks>
        /// <returns>A new customer</returns>
        /// <response code="201">Customer created successfully</response>
        /// <response code="400">Customer couldn't be created</response>
        /// <response code="500">Probably an insertion error</response>  
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CustomerDTO>>> Create(CustomerDTO toCreate)
        { 
            var configuration = new MapperConfiguration(cfg => {
                cfg.CreateMap<CustomerDTO, Customer>();
            });
            IMapper mapper = configuration.CreateMapper();
            var customer = mapper.Map<CustomerDTO, Customer>(toCreate);

            if (customer.Email.EndsWith("."))
            {
                return BadRequest("Bad format for the email.");
            }
            if (customer.Phone.EndsWith("."))
            {
                return BadRequest("Bad format for the phone.");
            }
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return Created(new Uri($"{Request.Path}/customer.Id", UriKind.Relative), toCreate);
        }

        // PUT /customer
        /// <summary>
        /// Updates a customer.
        /// </summary>
        /// <remarks>
        /// Example:
        ///
        ///     PUT /customer
        ///     {
        ///        "id": 0,
        ///        "name": "Yuki",
        ///        "email": "yuki@gmail.com",
        ///        "phone": "(61)99999-9999",
        ///     }
        ///
        /// </remarks>
        /// <returns>A new customer</returns>
        /// <response code="200">Returns all customers</response>
        /// <response code="400">Couldn't find customer</response>
        /// <response code="500">Probably an update error</response> 
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CustomerDTO>>> Update(CustomerDTO toUpdate)
        {
            
            var customer = await _context.Customers.FindAsync(toUpdate.Id);
            var configuration = new MapperConfiguration(cfg => 
            {
                cfg.CreateMap<CustomerDTO, Customer>();
            });
            IMapper mapper = configuration.CreateMapper();
            var mapped = mapper.Map<CustomerDTO, Customer>(toUpdate);
            
            
            foreach (PropertyInfo property in typeof(Customer).GetProperties().Where(p => p.CanWrite))
            {
                property.SetValue(customer, property.GetValue(mapped, null), null);
            }

            await _context.SaveChangesAsync();
            return Ok(await _context.Customers.Include("Address").ToListAsync());
        }

        // DELETE /customer/{id}
        /// <summary>
        /// Deletes a specific customer.
        /// </summary>
        /// <param name="id">Required parameter</param>
        /// <returns>A new customer</returns>
        /// <response code="200">Returns all customers</response>
        /// <response code="400">Couldn't find customer</response>  
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<CustomerDTO>>> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is null) return BadRequest("Couldn't find customer.");
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return Ok(await _context.Customers.Include("Address").ToListAsync());
        }
        
    }

}