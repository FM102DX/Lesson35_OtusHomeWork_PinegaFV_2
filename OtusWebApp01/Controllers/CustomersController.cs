﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Otus.Teaching.Concurrency.Import.Core.Repositories;
using Otus.Teaching.Concurrency.Import.Core.Entities;
using Otus.Teaching.Concurrency.Import.Core.ViewModels;
using Otus.Teaching.Concurrency.Import.DataAccess.Data;
using Otus.Teaching.Concurrency.Import.Core.Service;
using Microsoft.AspNetCore.Http;


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

        /*
        [HttpGet("{id}")]
        public Customer Get(int id)
        {
            return this._repo.GetItemById(id);
        }
        
        [HttpGet]
        [Route("getall/")]
        public IEnumerable<Customer> GetAll()
        {
            return _repo.GetAllItems();
        }
        
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ActionResult))]
        [Route("/")]
        public ActionResult AddCustomer([FromBody] Customer customer)
        {
            CommonOperationResult rez = _repo.AddItem(customer);
            return Ok(rez);
        }
        
        */

    }
}
