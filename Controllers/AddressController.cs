using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PloomesTest.Models;
using PloomesTest.Data;
using System.Reflection;
using AutoMapper;

namespace PloomesTest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly DataContext _context;

        public AddressController(DataContext context)
        {
            _context = context; 
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<AddressDTO>>> Read()
        {
            var result = await _context.Addresses.AsNoTracking().ToListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AddressDTO>> Read(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address is null) return BadRequest("Address not found.");
            return Ok(address);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AddressDTO>>> Create(AddressDTO toCreate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var configuration = new MapperConfiguration(cfg => {
                cfg.CreateMap<AddressDTO, Address>();
            });
            IMapper mapper = configuration.CreateMapper();
            var address = mapper.Map<AddressDTO, Address>(toCreate);
            
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
            return Created(new Uri($"{Request.Path}/address.Id", UriKind.Relative), address);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AddressDTO>>> Update(AddressDTO toUpdate)
        {
            
            var address = await _context.Addresses.FindAsync(toUpdate.Id);
            if (address is null) return BadRequest("Couldn't find address.");

            var configuration = new MapperConfiguration(cfg => {
                cfg.CreateMap<AddressDTO, Address>().ForSourceMember(x => x.Id, option => option.DoNotValidate());
            });
            IMapper mapper = configuration.CreateMapper();
            var mapped = mapper.Map<AddressDTO, Address>(toUpdate);
            
            foreach (PropertyInfo property in typeof(Address).GetProperties().Where(p => p.CanWrite))
            {
                property.SetValue(address, property.GetValue(mapped, null), null);
            }

            await _context.SaveChangesAsync();
            return Ok(await _context.Addresses.ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<AddressDTO>>> Delete(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address is null) return BadRequest("Couldn't find address.");
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return Ok(await _context.Addresses.ToListAsync());
        }
    }
}