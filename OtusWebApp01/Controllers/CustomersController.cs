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
            return "Hi! This is customers controller!";
        }




    }
}
