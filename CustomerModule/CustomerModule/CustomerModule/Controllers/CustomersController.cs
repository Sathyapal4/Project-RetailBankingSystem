using CustomerModule.Models;
using CustomerModule.CustomersRepository;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CustomerModule.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Employee")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILog _logger = LogManager.GetLogger(typeof(CustomersController));

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAllCustomers()
        {
            try
            {
                _logger.Info("Get All Customers Called in Customers Controller");
                List<Customer> customers = _customerRepository.GetAllCustomers();
                if (customers == null)
                    return NotFound();
                return Ok(customers);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while fetching customer");
            }
        }

        [HttpGet]
        [Route("getCustomerDetails/{customer_Id}")]
        public IActionResult GetCustomerDetails(int customer_Id)
        {
            try
            {
                _logger.Info("Get Customer Details Called in Customer Controller");
                Customer customer = _customerRepository.GetCustomerDetails(customer_Id);
                if (customer == null)
                    return NotFound("Customer with this Id is not exist");
                return Ok(customer);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while fetching customer");
            }
        }

        [HttpPost]
        [Route("createCustomer")]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
            try
            {
                _logger.Info("Create Customer Called in Customer Controller");

                if (!ModelState.IsValid)
                    return BadRequest("Customer Data is not proper");
                CustomerCreationStatus customerCreationStatus = _customerRepository.CreateCustomer(customer);
                if (customerCreationStatus.CustomerId != null)
                    return StatusCode(StatusCodes.Status201Created, customerCreationStatus);
                else
                    return BadRequest(customerCreationStatus);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while fetching customer");
            }
        }

        [HttpPost]
        [Route("checkCredentials")]
        [AllowAnonymous]
        public IActionResult CheckCustomerCredentials([FromBody] CustomerRequest customerRequest)
        {
            try
            {
                _logger.Info("Check Customer Credentials Called in Customer Controller");
                if (!ModelState.IsValid)
                    return BadRequest("Invalid Email And Password");
                CustomerResponse result = _customerRepository.GetCustomer(customerRequest);
                if (result != null)
                    return Ok(result);
                return BadRequest("Invalid Email And Password");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Try Again After some Time");
            }
        }
    }
}
