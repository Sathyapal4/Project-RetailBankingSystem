using log4net;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TransactionsModule.Models;
using TransactionsModule.Services;

namespace TransactionsModule.TransactionsRepository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TransactionDbContext _context;
        private readonly IAccountService _accountService;
        private readonly ILog _logger = LogManager.GetLogger(typeof(TransactionRepository));

        public TransactionRepository(TransactionDbContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }
        public Ref_Transaction_Status Deposit(Account account)
        {
            try
            {
                _logger.Info("Deposit Method called in Transaction Repository");
                //call deposit action of Account Microservice and pass Acc object
                AmountResponse response = _accountService.Deposit(account);
                Ref_Transaction_Status ref_Transaction_Status;
                if (response.Success)
                {
                    _context.Financial_Transactions.Add(
                        new Financial_Transaction
                        {
                            Account_ID = account.AccountId,
                            Counterparty_ID = 1,
                            Payment_Method_Code = 1,
                            Service_ID = 1,
                            Trans_Status_Code = 1,
                            Trans_Type_Code = 2,
                            Date_of_Transaction = DateTime.Now,
                            Amount_of_Transaction = account.Amount
                        }
                        );
                    ref_Transaction_Status = new Ref_Transaction_Status()
                    {
                        Trans_Status_Code = 1,
                        Trans_Status_Description = Trans_Status_Description.Completed
                    };

                }
                else
                {
                    _context.Financial_Transactions.Add(
                       new Financial_Transaction
                       {
                           Account_ID = account.AccountId,
                           Counterparty_ID = 1,
                           Payment_Method_Code = 1,
                           Service_ID = 1,
                           Trans_Status_Code = 2,
                           Trans_Type_Code = 2,
                           Date_of_Transaction = DateTime.Now,
                           Amount_of_Transaction = account.Amount
                       }
                       );
                    ref_Transaction_Status = new Ref_Transaction_Status()
                    {
                        Trans_Status_Code = 2,
                        Trans_Status_Description = Trans_Status_Description.Disputed
                    };

                }
                _context.SaveChanges();
                return ref_Transaction_Status;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public Ref_Transaction_Status Withdraw(Account account)
        {
            try
            {
                _logger.Info("Withdraw Method called in Transaction Repository");
                //call Withdraw action of Account Microservice and pass Acc object
                AmountResponse response = _accountService.WithDraw(account);
                Ref_Transaction_Status ref_Transaction_Status = null;
                if (response.Success)
                {
                    _context.Financial_Transactions.Add(
                        new Financial_Transaction
                        {
                            Account_ID = account.AccountId,
                            Counterparty_ID = 1,
                            Payment_Method_Code = 1,
                            Service_ID = 1,
                            Trans_Status_Code = 1,
                            Trans_Type_Code = 2,
                            Date_of_Transaction = DateTime.Now,
                            Amount_of_Transaction = account.Amount
                        }
                        );
                    ref_Transaction_Status = new Ref_Transaction_Status()
                    {
                        Trans_Status_Code = 1,
                        Trans_Status_Description = Trans_Status_Description.Completed
                    };

                }
                else
                {
                    _context.Financial_Transactions.Add(
                       new Financial_Transaction
                       {
                           Account_ID = account.AccountId,
                           Counterparty_ID = 1,
                           Payment_Method_Code = 1,
                           Service_ID = 1,
                           Trans_Status_Code = 2,
                           Trans_Type_Code = 2,
                           Date_of_Transaction = DateTime.Now,
                           Amount_of_Transaction = account.Amount
                       }
                       );
                    ref_Transaction_Status = new Ref_Transaction_Status()
                    {
                        Trans_Status_Code = 2,
                        Trans_Status_Description = Trans_Status_Description.Disputed
                    };

                }
                _context.SaveChanges();
                return ref_Transaction_Status;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public Ref_Transaction_Status Transfer(Transfer transfer)
        {
            try
            {
                _logger.Info("Transfer Method called in Transaction Repository");
                Ref_Transaction_Status ref_Transaction_Status = null;
                bool success = _accountService.GetAccountId(transfer.Target_AccountId);
                if (success == false)
                    return null;
                //Call Withdraw microservice for rules
                AmountResponse response = _accountService.WithDraw(new Account { AccountId = transfer.Source_AccountId, Amount = transfer.Amount });
                if (response.Success)
                {
                    AmountResponse response1 = _accountService.Deposit(new Account { AccountId = transfer.Target_AccountId, Amount = transfer.Amount });
                    if (response1.Success)
                    {
                        _context.Financial_Transactions.Add(
                        new Financial_Transaction
                        {
                            Account_ID = transfer.Source_AccountId,
                            Counterparty_ID = 1,
                            Payment_Method_Code = 1,
                            Service_ID = 1,
                            Trans_Status_Code = 1,
                            Trans_Type_Code = 2,
                            Date_of_Transaction = DateTime.Now,
                            Amount_of_Transaction = transfer.Amount
                        }
                        );
                        _context.Financial_Transactions.Add(
                        new Financial_Transaction
                        {
                            Account_ID = transfer.Target_AccountId,
                            Counterparty_ID = 1,
                            Payment_Method_Code = 1,
                            Service_ID = 1,
                            Trans_Status_Code = 1,
                            Trans_Type_Code = 2,
                            Date_of_Transaction = DateTime.Now,
                            Amount_of_Transaction = transfer.Amount
                        }
                        );
                        ref_Transaction_Status = new Ref_Transaction_Status()
                        {
                            Trans_Status_Code = 1,
                            Trans_Status_Description = Trans_Status_Description.Completed
                        };
                    }
                }
                else
                {
                    _context.Financial_Transactions.Add(
                       new Financial_Transaction
                       {
                           Account_ID = transfer.Source_AccountId,
                           Counterparty_ID = 1,
                           Payment_Method_Code = 1,
                           Service_ID = 1,
                           Trans_Status_Code = 2,
                           Trans_Type_Code = 2,
                           Date_of_Transaction = DateTime.Now,
                           Amount_of_Transaction = transfer.Amount
                       }
                       );
                    _context.Financial_Transactions.Add(
                       new Financial_Transaction
                       {
                           Account_ID = transfer.Target_AccountId,
                           Counterparty_ID = 1,
                           Payment_Method_Code = 1,
                           Service_ID = 1,
                           Trans_Status_Code = 2,
                           Trans_Type_Code = 2,
                           Date_of_Transaction = DateTime.Now,
                           Amount_of_Transaction = transfer.Amount
                       }
                       );
                    ref_Transaction_Status = new Ref_Transaction_Status()
                    {
                        Trans_Status_Code = 2,
                        Trans_Status_Description = Trans_Status_Description.Disputed
                    };
                }
                _context.SaveChanges();
                return ref_Transaction_Status;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }


        public List<Financial_Transaction> GetTransactions(int accountId)
        {
            try
            {
                _logger.Info("Get Transaction Method called in Transaction Repository");
                List<Financial_Transaction> financial_Transactions = _context.Financial_Transactions.Where(c => c.Account_ID == accountId)
                                                                                                    .Include(c => c.Ref_Payment_Methods)
                                                                                                    .Include(c => c.Ref_Transaction_Status)
                                                                                                    .Include(c => c.Ref_Transaction_Types)
                                                                                                    .ToList();
                return financial_Transactions;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }
    }
}
