using MyFinanceWebApi.CustomHandlers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using MyFinanceModel;

namespace MyFinanceWebApi.Controllers
{
    public class TestController : BaseController
    {
        //[HttpGet]
        //public string SomeAction()
        //{
        //    return "some action method";
        //}

        //[ValidateModelState]
        //[HttpPost]
        //public IHttpActionResult ThrowErrorPost([FromBody] CustomModel customModel)
        //{
        //    return Ok();
        //}

        //[AllowAnonymous]
        //[ValidateModelState]
        //[HttpPost]
        //public IHttpActionResult CustomThrowErrorPost([FromBody] CustomModel customModel)
        //{
        //    return Ok();
        //}

        //[HttpGet]
        //public string ThrowError(string message)
        //{
        //    throw new ServiceException(message, 535);
        //}

        //[HttpGet]
        //public IHttpActionResult ComplexUrl([FromUri] CustomModel customModel)
        //{
        //    return Ok();
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public IHttpActionResult DateTimeTest([FromUri] DateTime dateTime)
        //{
        //    return Ok();
        //}
    }

    public class CustomModel
    {
        [Required]
        public string Id { get; set; }

        public DateTime? Date { get; set; }

        [MaxLength(5)]
        [Required]
        public string Name { get; set; }
    }
}
