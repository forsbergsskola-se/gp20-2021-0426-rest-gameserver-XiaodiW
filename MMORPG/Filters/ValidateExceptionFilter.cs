using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using MMORPG.Help;

namespace MMORPG.Filters {

    public class ValidateExceptionFilter : ExceptionFilterAttribute, IExceptionFilter {
        public override void OnException(ExceptionContext context) {
            if(context.Exception is NewItemValidationException) {
                var response = new HttpResponseMessage(HttpStatusCode.NotAcceptable)
                {
                    Content = new StringContent("New Item input Error!"),
                    ReasonPhrase = "The New Item's level not match Player level Requirement."
                };
                context.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "The New Item's level not match Player level Requirement.";
                context.Result = new HttpResponseMessageResult(response);
                base.OnException(context);
            }
        }
    }

}