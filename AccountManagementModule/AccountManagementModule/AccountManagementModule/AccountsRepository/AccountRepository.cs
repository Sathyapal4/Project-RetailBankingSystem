using AccountManagementModule.Models;
using log4net;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AccountManagementModule.AccountsRepository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILog _logger = LogManager.GetLogger(typeof(AccountRepository));

        public AccountRepository(AccountDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool CreateAccount(int customerId)
        {
            try
            {
                _logger.Info("Create Account of Account Repository Called ");
                _context.Accounts.Add(new Account() { AccountType = AccountType.Saving, Balance = 0, CustomerId = customerId });
                _context.Accounts.Add(new Account() { AccountType = AccountType.Current, Balance = 0, CustomerId = customerId });
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public List<Account> GetCustomerAccounts(int customerId)
        {
            try
            {
                _logger.Info("Get Customer Accounts of Account Repository Called ");
                List<Account> accounts = _context.Accounts.Where(c => c.CustomerId == customerId).ToList();
                return accounts;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public Account GetAccount(int accountId)
        {
            try
            {
                _logger.Info("Get Account of Account Repository Called ");
                Account account = _context.Accounts.Where(c => c.AccountId == accountId).SingleOrDefault();
                return account;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }


        public bool Deposit(AmountRequest amountRequest)
        {
            try
            {
                _logger.Info("Deposit of Account Repository Called ");
                Account acc = _context.Accounts.FirstOrDefault(a => a.AccountId == amountRequest.AccountId);
                acc.Balance += amountRequest.Amount;
                _context.TransactionStatuses.Add(new TransactionStatus() { AccountId = amountRequest.AccountId, Message = "Success", currentBalance = acc.Balance });
                _context.Statements.Add(new Statement() { AccountId = amountRequest.AccountId, Date = DateTime.Now, Narration = amountRequest.Narration, Deposit = amountRequest.Amount, Withdrawal = 0, ClosingBalance = acc.Balance, RefNo = $"Ref_No_{amountRequest.AccountId}_{DateTime.Now.ToShortDateString()}", ValueDate = DateTime.Now });
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public bool Withdraw(AmountRequest amountRequest)
        {
            try
            {
                _logger.Info("Withdraw of Account Repository Called ");
                int customerId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                Account acc = _context.Accounts.FirstOrDefault(a => a.AccountId == amountRequest.AccountId && customerId == a.CustomerId);
                if (acc == null)
                    return false;
                if ((acc.Balance - amountRequest.Amount) < 0)
                    return false;
                acc.Balance -= amountRequest.Amount;
                _context.TransactionStatuses.Add(new TransactionStatus() { AccountId = amountRequest.AccountId, Message = "Success", currentBalance = acc.Balance });
                _context.Statements.Add(new Statement() { AccountId = amountRequest.AccountId, Date = DateTime.Now, Narration = amountRequest.Narration, Deposit = 0, Withdrawal = amountRequest.Amount, ClosingBalance = acc.Balance, RefNo = $"Ref_No_{amountRequest.AccountId}_{DateTime.Now.ToShortDateString()}", ValueDate = DateTime.Now });
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public List<Statement> GetStatements(int accountId, string from_date, string to_date)
        {
            try
            {
                _logger.Info("Get Statement of Account Repository Called ");
                DateTime fromDate;
                DateTime toDate;
                if (from_date != null && to_date != null)
                {
                    fromDate = DateTime.ParseExact(from_date, "yyyy-MM-dd", null);
                    toDate = DateTime.ParseExact(to_date, "yyyy-MM-dd", null).AddDays(1);
                }
                else
                {
                    fromDate = DateTime.Now.AddMonths(-1);
                    toDate = DateTime.Now.AddDays(1);
                }
                List<Statement> statements = _context.Statements.Where(c => c.Date >= fromDate && c.Date <= toDate && c.AccountId == accountId).ToList();
                return statements;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

    }
}
