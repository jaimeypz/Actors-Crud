using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstApplication.Services
{
    public class AppService: DBService
    {
        public AppService(IConfiguration configuration)
          : base(writeConnectionString: configuration["ConnectionStrings:MoviesIGDConnection"],
                  storedProcedure: "[dbo].[SP.Manage_movies]")
        {
        }


    }
}
