using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using t_g_Minesweeper.WebApi.models;

namespace t_g_Minesweeper.WebApi.Controllers
{
    public class CustomExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext actionExecutedContext)
        {
            var options1 = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            };

            string exceptionMessage = actionExecutedContext.Exception.Message;
            var error = new ErrorMessage() {Error = exceptionMessage };
            var str = JsonSerializer.Serialize(error, options1);
            var response = new ContentResult()
            {
                Content = str
            };

            response.StatusCode = 500;
            if (actionExecutedContext.Exception.GetType() == typeof(InvalidOperationException))
            {

                response.StatusCode = 400;
            }
            actionExecutedContext.Result = response;
        }

    }
}
