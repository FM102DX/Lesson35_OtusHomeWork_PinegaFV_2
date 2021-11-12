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
            return "This is customers controller!";
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ActionResult))]
        public ActionResult<Customer> Get(int id)
        {
            Customer customer = this._repo.GetItemByIdOrNull(id);

            if (customer==null)
            {
                return NotFound();
            }
            else
            {
                return Ok(customer);
            }
            
        }
        
        [HttpGet]
        [Route("getall/")]
        public ActionResult<IEnumerable<Customer>> GetAll()
        {
            return Ok(_repo.GetAllItems());
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ActionResult))]
        public ActionResult<CommonOperationResult> AddCustomer([FromBody] Customer customer)
        {
            if (!_repo.Exists(customer.Id))
            {
                return Conflict();
            }

            CommonOperationResult rez = _repo.AddItem(customer);
            return Ok(rez);
        }

        [HttpPut]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ActionResult))]

        public ActionResult<CommonOperationResult> ModifyCustomer([FromBody] Customer customer)
        {
            CommonOperationResult rez = ModifyCustomerBase(customer);
            if (rez.success) { return Ok(rez); } else { return Conflict(rez); }
        }

        [HttpDelete("{id}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ActionResult))]
        public ActionResult<CommonOperationResult> Delete(int id)
        {
            try
            {
                CommonOperationResult rez = _repo.Delete(id);
                return Ok(rez);
            }
            catch (Exception ex)
            {
                return Conflict(CommonOperationResult.sayFail(ex.Message));
            }
        }

        #region GetPostModeOnly
        [HttpPost]
        [Route("update/")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ActionResult))]
        public ActionResult<CommonOperationResult> PostModifyCustomer([FromBody] Customer customer)
        {
            CommonOperationResult rez = ModifyCustomerBase(customer);
            if (rez.success) { return Ok(rez); } else { return Conflict(rez); }
        }

        [HttpPost("delete/{id}")]
       // [Route("delete/")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ActionResult))]
        public ActionResult<CommonOperationResult> PostDelete(int id)
        {
            CommonOperationResult rez = DeleteCustomerBase(id);
            if (rez.success) { return Ok(rez); } else { return Conflict(rez); }
        }

        private CommonOperationResult ModifyCustomerBase(Customer customer)
        {
            try
            {
                CommonOperationResult rez = _repo.UpdateItem(customer);
                return rez;
            }
            catch (Exception ex)
            {
                return CommonOperationResult.sayFail(ex.Message);
            }
        }

        private CommonOperationResult DeleteCustomerBase(int id)
        {
            try
            {
                CommonOperationResult rez = _repo.Delete(id);
                return rez;
            }
            catch (Exception ex)
            {
                return CommonOperationResult.sayFail(ex.Message);
            }
        }

        #endregion 
    }
}
