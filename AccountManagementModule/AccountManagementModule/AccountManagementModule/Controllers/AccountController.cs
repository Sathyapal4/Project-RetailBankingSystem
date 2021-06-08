using AccountManagementModule.Models;
using AccountManagementModule.AccountsRepository;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace AccountManagementModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILog _logger = LogManager.GetLogger(typeof(AccountController));

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpPost("[action]")]
        [Authorize(Roles = "Employee")]
        public IActionResult CreateAccount([FromBody] CustomerID customerID)
        {
            try
            {
                _logger.Info("Create Account of Account Controller Called ");
                bool success = _accountRepository.CreateAccount(customerID.CustomerId);
                if (success)
                    return StatusCode(StatusCodes.Status201Created);
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
        [HttpGet("[action]/{customerId}")]
        public IActionResult GetCustomerAccounts(int customerId)
        {
            try
            {
                _logger.Info("Get Customer Accounts of Account Controller Called ");
                List<Account> accounts = _accountRepository.GetCustomerAccounts(customerId);
                if (accounts == null)
                    return NotFound("No Account Found for this Customer");
                else
                    return Ok(accounts);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }


        }


        [HttpGet("[action]/{accountId}")]
        public IActionResult GetAccount(int accountId)
        {
            try
            {
                _logger.Info("Get Account of Account Controller Called ");
                Account account = _accountRepository.GetAccount(accountId);
                if (account == null)
                    return NotFound("No Account Found for this Account Id");
                else
                    return Ok(account);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }


        [HttpPost("[action]")]
        public IActionResult Deposit([FromBody] AmountRequest amountRequest)
        {
            try
            {
                _logger.Info("Deposit of Account Controller Called ");
                bool success = _accountRepository.Deposit(amountRequest);
                if (success)
                    return Ok();
                return BadRequest();
            }

            catch (Exception e)
            {
                _logger.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost("[action]")]
        public IActionResult Withdraw([FromBody] AmountRequest amountRequest)
        {
            try
            {
                _logger.Info("Withdraw of Account Controller Called ");
                bool success = _accountRepository.Withdraw(amountRequest);
                if (success)
                    return Ok();
                return BadRequest(new { Message = "Customer didn't have own this account Or Withdrawl Amount is higher than balance" });
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpGet("[action]/{accountId}/{from_date?}/{to_date?}")]
        public IActionResult GetStatement(int accountId, string from_date = null, string to_date = null)
        {
            try
            {
                _logger.Info("Get Statement of Account Controller Called ");
                List<Statement> statements = _accountRepository.GetStatements(accountId, from_date, to_date);
                if (statements == null)
                    return NotFound("No Statements");
                else
                    return Ok(statements);

            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
