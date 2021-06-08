using CustomerModule.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomerModule.CustomersRepository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _context;
        private readonly IAccountService _accountService;
        private readonly ILog _logger = LogManager.GetLogger(typeof(CustomerRepository));

        public CustomerRepository(CustomerDbContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public List<Customer> GetAllCustomers()
        {
            try
            {
                _logger.Info("Get All Customers called in Customer Repository");
                List<Customer> customers = _context.Customers.ToList();
                return customers;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public CustomerCreationStatus CreateCustomer(Customer customer)
        {
            try
            {
                _logger.Info("Create Customer Method Called in Customer Repository");
                Customer customerinDB = _context.Customers.Where(c => c.PAN == customer.PAN).SingleOrDefault();
                if (customerinDB != null)
                    return new CustomerCreationStatus { Message = "Customer with Pan Number is already existed" };
                _context.Customers.Add(customer);
                _context.SaveChanges();
                bool success = _accountService.CreateAccount(customer.CustomerId);
                if (success)
                {
                    return new CustomerCreationStatus { CustomerId = customer.CustomerId, Message = "CustomerAccount is Created Successfully" };
                }
                else
                    return new CustomerCreationStatus { Message = "Error while creating Account" };

            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public Customer GetCustomerDetails(int customerId)
        {
            try
            {
                _logger.Info("Get Customer Details Method Called in Customer Repository");
                Customer customer = _context.Customers.Find(customerId);
                if (customer == null)
                    return null;
                return customer;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }

        }

        public CustomerResponse GetCustomer(CustomerRequest customerRequest)
        {
            try
            {
                _logger.Info("Check Customer Credentials  Method Called in Customer Repository");
                Customer customer = _context.Customers.Where(c => c.Email == customerRequest.Email && c.Password == customerRequest.Password).FirstOrDefault();
                if (customer == null)
                    return null;
                return new CustomerResponse { Id = customer.CustomerId };
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }
    }
}
