using LineBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace LineBot.Filters
{
    public class LineVerifySignatureAttribute : TypeFilterAttribute
    {
        public LineVerifySignatureAttribute() : base(typeof(LineVerifySignatureFilter))
        {
        }
    }

    public class LineVerifySignatureFilter : IAuthorizationFilter
    {
        public IOptions<LineSetting> lineSetting { get; }

        public LineVerifySignatureFilter(IOptions<LineSetting> _lineSetting)
        {
            lineSetting = _lineSetting;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Ensure the requestBody can be read multiple times.
            context.HttpContext.Request.EnableBuffering();

            string requestBody = new StreamReader(context.HttpContext.Request.Body).ReadToEndAsync().Result;
            context.HttpContext.Request.Body.Position = 0;

            string xLineSignature = context.HttpContext.Request.Headers["x-line-signature"].ToString();
            byte[] channelSecret = Encoding.UTF8.GetBytes(lineSetting.Value.ChannelSecret);
            byte[] body = Encoding.UTF8.GetBytes(requestBody);

            using (HMACSHA256 hmac = new HMACSHA256(channelSecret))
            {
                byte[] hash = hmac.ComputeHash(body, 0, body.Length);
                byte[] xLineBytes = Convert.FromBase64String(xLineSignature);

                if(SlowEquals(hash, xLineBytes) == false )
                {
                    context.Result = new ForbidResult();
                }
            }
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for(int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}
