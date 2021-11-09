using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Otus.Teaching.Concurrency.Import.Core.Repositories;
using Otus.Teaching.Concurrency.Import.Core.Entities;


namespace OtusREST.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CustomersController : Controller
    {

        IGenericRepository<Customer>  _repo;

        public CustomersController(IGenericRepository<Customer> repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public string Get()
        {
            return "This is Customers info";
        }

        //https://localhost:44329/car
        [HttpGet]
        public IEnumerable<Customer> GetAllObjects()
        {
            return _repo.GetAllItems();
        }

        //https://localhost:44329/car/1
        [HttpGet("{id}")]
        public Customer Get(int id)
        {
            Customer customer = _repo.GetItemById(id);
            return customer;
        }

        // POST <Controller>
        [HttpPost]
        public ActionResult Post([FromBody] Customer customer)
        {
            _repo.AddItemAsync(customer);

            return CreatedAtAction(nameof(Post), new { id = customer.Id });
        }

    }
}
