using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI.Common;
using TempLogg.Models;

namespace TempLogg
{
    


    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient _client;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _client = new HttpClient();
            _logger.LogInformation(" Temprature logging has been started");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Dispose();
            _logger.LogInformation(" Temprature logging has been stopped");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var httpClient = HttpClientFactory.Create();
            while (!stoppingToken.IsCancellationRequested)
            {

                try
                {
                    
                    var url = "https://api.openweathermap.org/data/2.5/onecall?lat=59.27412&units=metric&lon=15.2066&exclude=hourly,daily,minutely&appid=5bf919005c4c20e778ba98f74c7f2e33";
                    HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        
                        var content = httpResponseMessage.Content;
                        var datatemp = await content.ReadAsAsync<Rootobject>();
                        
                        
                        if (datatemp.current.temp < 20)
                        {
                            _logger.LogInformation($"The temperature is : {datatemp.current.temp}");
                            
                        }
                        else
                        {
                            _logger.LogInformation($"Warning!! Temperature is: {datatemp.current.temp}");
                           
                        }

                    }
                    
                }
                catch(Exception ex)
                {
                    _logger.LogInformation(ex.Message);
                    
                }
                await Task.Delay(60*1000, stoppingToken);
            }
        }
    }
}
