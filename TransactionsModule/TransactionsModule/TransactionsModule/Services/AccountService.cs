using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using TransactionsModule.Models;

namespace TransactionsModule.Services
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly IRulesService _rulesService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILog _logger = LogManager.GetLogger(typeof(AccountService));

        public AccountService(IConfiguration configuration, IRulesService rulesService, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _rulesService = rulesService;
            _httpContextAccessor = httpContextAccessor;
        }

        public AmountResponse Deposit(Account account)
        {
            try
            {
                _logger.Info("Deposit method called in account service");
                using (HttpClient _client = new HttpClient())
                {
                    StringValues token;
                    _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);
                    _client.BaseAddress = new Uri(_configuration["BaseUrl:Account"]);
                    _client.DefaultRequestHeaders.Add("Authorization", token.ToString());
                    var stringPayload = JsonConvert.SerializeObject(account);
                    var payload = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseMessage = _client.PostAsync("api/account/deposit", payload).Result;
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        return new AmountResponse { Message = "Amount Deposited Successfull", Success = true };
                    }
                    return new AmountResponse { Message = "Error while Deposit Amount", Success = false };
                }

            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }
        public AmountResponse WithDraw(Account account)
        {
            try
            {
                //RuleService
                _logger.Info("Withdraw method called in account service");
                RuleStatus ruleStatus = _rulesService.CheckMinimumBalance(account);
                if (ruleStatus.Status == Status.Denied)
                    return new AmountResponse { Message = "Balance is Less Than Minimum Balance", Success = false };
                using (HttpClient _client = new HttpClient())
                {
                    StringValues token;
                    _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);
                    _client.BaseAddress = new Uri(_configuration["BaseUrl:Account"]);
                    _client.DefaultRequestHeaders.Add("Authorization", token.ToString());
                    var stringPayload = JsonConvert.SerializeObject(account);
                    var payload = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseMessage = _client.PostAsync("api/account/withdraw", payload).Result;
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        return new AmountResponse { Message = "Amount Withdraw Successfull", Success = true };
                    }
                    return new AmountResponse { Message = "Error while Withdraw Amount", Success = false };
                }

            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public bool GetAccountId(int accountId)
        {
            try
            {
                _logger.Info("GetAccountId method called in account service");
                using (HttpClient _client = new HttpClient())
                {
                    StringValues token;
                    _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);
                    _client.BaseAddress = new Uri(_configuration["BaseUrl:Account"]);
                    _client.DefaultRequestHeaders.Add("Authorization", token.ToString());
                    HttpResponseMessage responseMessage = _client.GetAsync("api/Account/GetAccount/" + accountId).Result;
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }
    }
}
