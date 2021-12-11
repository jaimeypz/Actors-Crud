using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FirstApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace FirstApplication.Areas.Cine.Controllers
{
    [Area("Cine")]
    [DisplayName("Cine.Actor")]
    public class ActorController : Controller
    {
        private readonly AppService _service;
        private readonly IHubContext<SystemHub> _context;

        public ActorController(AppService service, IHttpContextAccessor contextAccessor, IHubContext<SystemHub> context)
        {
            _context = context;
            _service = service;
            contextAccessor?.HttpContext?.User?.Identity?.Name?.Split(@"\");

        }

        private JsonResult GetMessageJson(string error) => string.IsNullOrWhiteSpace(error) switch
        {
            true => Json(new { Status = "success", Message = "Record save successfully!!" }),
            false => Json(new { Status = "error", Message = error })
        };

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ActorPopUp(string actorId)
        {
            ViewBag.Actor = null;
            if (actorId != null)
            {
                ViewBag.Actor = _service.GetData("GET_ACTORS", actorId)?.FirstOrDefault();
            }
            return PartialView();
        }

        public IActionResult DeleteActorPopUp(string actorId)
        {

            ViewBag.Actor = (actorId != null) ? _service.GetData("GET_ACTORS", actorId).FirstOrDefault() : null;
            ViewBag.Message = $"Do you want to delete?";
            return PartialView();

        }

        //public IActionResult SaveActor()
        //{

        //    var operation = string.IsNullOrWhiteSpace(HttpContext.Request.Form["ActorID"]) ? "INS_ACTORS" : "UPD_ACTORS";

        //    var data = new string[]
        //    {
        //    HttpContext.Request.Form["ActorName"],
        //    HttpContext.Request.Form["ActorDOB"],
        //    HttpContext.Request.Form["ActorGender"],
        //    };
        //    _service.DataChangeEventHandler += RefreshActors;
        //    var (_, error) = _service.Execute(operation, data);
        //    //

        //    //
        //    if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        //    {
        //        return GetMessageJson(error);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Error");
        //    }
        //}

        public IActionResult SaveActor()
        {
            var actorId = HttpContext.Request.Form["ActorID"];
            var Name = HttpContext.Request.Form["ActorName"];
            var Dob = HttpContext.Request.Form["ActorDOB"];
            var Gender = HttpContext.Request.Form["ActorGender"];

            var data = new string[]
            {
            actorId,
            Name,
            Dob,
            Gender,
            };
            _service.DataChangeEventHandler += RefreshActors;
            var (_, error) = _service.Execute(string.IsNullOrWhiteSpace(actorId) ? "INS_ACTORS" : "UPD_ACTORS", data);
            //

            //
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return GetMessageJson(error);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public IActionResult DeleteActor()
        {
            var actorId = HttpContext.Request.Form["ActorID"];

            var data = new string[]
            {
                actorId,
            };

            _service.DataChangeEventHandler += RefreshActors;
            var (_, error) = _service.Execute("DEL_ACTORS", data);
            //
            var json = string.IsNullOrWhiteSpace(error) switch
            {
                true => Json(new { Status = "success", Message = $"Delete with Actor: { actorId } deleted successfully!!" }),
                false => Json(new { Status = "error", Message = error })
            };

            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return json;
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public IActionResult ActorsJson() => _service.GetJsonData("GET_ACTORS");


        private async void RefreshActors(object? sender, EventArgs e) => await _context.Clients.All.SendAsync("RefreshActors");

    }
}
