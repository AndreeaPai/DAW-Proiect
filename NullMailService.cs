using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//service for testing

namespace ArtShop.Services
{
    public class NullMailService : IMailService
    {
        //inject logger into our class, used to log information no matter where is going
        //create a constructor ctor + tab
        //cream un un read only field pt Ilogger
        private readonly ILogger<NullMailService> _logger;
        public NullMailService(ILogger<NullMailService> logger)
        {
            //legam in constr acel field logger creat mai sus
            _logger = logger;
        }

        public void SendMessage(string to, string subject, string body)
        {
            //log the message,but not actually send it
            _logger.LogInformation($"To: {to} Subject: {subject} Body: {body}");
        }
    }
}
