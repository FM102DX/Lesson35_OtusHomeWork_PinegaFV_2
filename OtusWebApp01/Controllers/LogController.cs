using Microsoft.AspNetCore.Mvc;
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
    [Route("/")]
    public class Log : Controller
    {
        
        IGenericRepository<ConsoleToApiMessage> _messagesRepository;
        IGenericRepository<Customer> _customersRepository;

        public Log(IGenericRepository<ConsoleToApiMessage> messagesRepository, IGenericRepository<Customer> customersRepository)
        {
            _messagesRepository = messagesRepository;
            _customersRepository = customersRepository;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            var viewModel = new LogViewModel()
            {
                MyMessagesList = _messagesRepository.GetAllItems(),
                MyCustomersList = _customersRepository.GetAllItems()
            };
            return View("LogIndexView", viewModel);
        }
        /*
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ActionResult))]
        [Route("/")]
        public ActionResult AddMessage([FromBody] ConsoleToApiMessage msg)
        {
            CommonOperationResult rez = _messagesRepository.AddItem(msg);
            return Ok(rez);
        }
        */
    }
}
