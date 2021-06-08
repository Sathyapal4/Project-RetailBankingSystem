using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace CustomerModule.CustomersRepository
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILog _logger = LogManager.GetLogger(typeof(AccountService));

        public AccountService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool CreateAccount(int customerId)
        {
            try
            {
                _logger.Info("Create Account Method Called in Account Service");
                using (HttpClient _client = new HttpClient())
                {
                    StringValues token;
                    _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);
                    _client.BaseAddress = new Uri(_configuration["BaseUrl:Account"]);
                    _client.DefaultRequestHeaders.Add("Authorization", token.ToString());
                    var Stringpayload = JsonConvert.SerializeObject(new { CustomerId = customerId });
                    var payload = new StringContent(Stringpayload, Encoding.UTF8, "application/json");
                    _logger.Info("Account Api Called");
                    HttpResponseMessage responseMessage = _client.PostAsync($"api/account/createAccount", payload).Result;
                    if (responseMessage.IsSuccessStatusCode)
                        return true;
                    else
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
