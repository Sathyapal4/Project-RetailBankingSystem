using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using TransactionsModule.Models;

namespace TransactionsModule.Services
{
    public class RulesService : IRulesService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILog _logger = LogManager.GetLogger(typeof(RulesService));

        public RulesService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public RuleStatus CheckMinimumBalance(Account account)
        {
            try
            {
                _logger.Info("Checking Minimum Balance");
                using (HttpClient _client = new HttpClient())
                {
                    StringValues token;
                    _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);
                    _client.BaseAddress = new Uri(_configuration["BaseUrl:Rules"]);
                    _client.DefaultRequestHeaders.Add("Authorization", token.ToString());
                    HttpResponseMessage responseMessage = _client.GetAsync($"api/rules/EvaluateMinBalance/{account.AccountId}").Result;
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        RuleStatus response = JsonConvert.DeserializeObject<RuleStatus>(responseMessage.Content.ReadAsStringAsync().Result);
                        return response;
                    }
                    return new RuleStatus { Status = Status.Denied };
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
