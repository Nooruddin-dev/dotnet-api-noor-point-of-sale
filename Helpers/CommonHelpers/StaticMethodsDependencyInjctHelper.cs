using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.CommonHelpers
{
    public class StaticMethodsDependencyInjctHelper
    {
        public static IConfiguration? config;
        public static IHttpContextAccessor? contextAccessor;
        public static void Initialize(IConfiguration? Configuration, IHttpContextAccessor? httpContextAccessor)
        {
            config = Configuration;
            contextAccessor = httpContextAccessor;
        }
    }
}
