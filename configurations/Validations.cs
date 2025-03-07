using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CacheServer.configurations
{
    public static class Validations
    {


        public static void Validate(int port,string url)
        {
            if (port < 1024 || port > 65535)
            {
                throw new ArgumentException("Port must be between 1024 and 65535.");
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("URL cannot be null or empty.");
            }
           

            var urlRegex = new Regex(
            @"^(https?|ftps?):\/\/(?:[a-zA-Z0-9]" +
                    @"(?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}" +
                    @"(?::(?:0|[1-9]\d{0,3}|[1-5]\d{4}|6[0-4]\d{3}" +
                    @"|65[0-4]\d{2}|655[0-2]\d|6553[0-5]))?" +
                    @"(?:\/(?:[-a-zA-Z0-9@%_\+.~#?&=]+\/?)*)?$",
            RegexOptions.IgnoreCase);

            urlRegex.Matches(url);
            if (!urlRegex.IsMatch(url))
            {
                throw new ArgumentException("Invalid URL format.");
            } 
        }
    }
}
