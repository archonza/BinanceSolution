using System.Web.Mvc;
using PublicAPIToolkit.Toolkit.Models;
using PublicAPIToolkit.Rest.Controllers;

namespace PublicAPIToolkit.Toolkit.Controllers
{
   public class ToolkitController : Controller
   {
      private RestClientController restClientController;
      private ToolkitViewModel toolkitViewModel = new ToolkitViewModel();

      // GET: Toolkit
      public ActionResult Index()
      {
         ViewBag.Title = "Toolkit";
         return View();
      }

      [HttpPost]
      public JsonResult GetConnectionStatus(ToolkitInputModel toolkitInputModel)
      {
         restClientController = new RestClientController(toolkitInputModel.EndPoint, toolkitInputModel.HttpMethod);
         restClientController.MakeRequest();
         toolkitViewModel.ConnectionStatus = (restClientController.GetResponse() == "{}") ? true : false;
         return Json(toolkitViewModel, JsonRequestBehavior.AllowGet);
      }
   }
}