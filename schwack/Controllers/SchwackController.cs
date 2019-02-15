using System.Linq;
using System.Web.Mvc;

namespace schwack.Controllers
{
    public class SchwackController : Controller
    {
        // GET: Schwack
        public ActionResult home()
        {
            return View();
        }



        public JsonResult PostMessage(string from, string to, string message)
        {
            var postMessage = new Service.SchwackService().PostMessage(new System.Guid(from), new System.Guid(to), message);

            return Json(new { success = true, message = postMessage }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SignIn(string who)
        {
            try
            {
                var users = new Service.SchwackService().SignIn(who);
                var user = users.SingleOrDefault(w => w.Name.ToLower() == who.ToLower());
                return Json(new { success = true, user = user, onlineUsers = users }, JsonRequestBehavior.AllowGet);
            }
            catch (System.ApplicationException ex)
            {
                if (ex.Message == "100")
                {
                    return Json(new { success = false, message = "User already exists.  Try again." }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, message = "I bent my wookie! </ralph>" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SignOut(string id)
        {
            new Service.SchwackService().SignOut(new System.Guid(id));
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsers()
        {
            var users = new Service.SchwackService().GetUsers();
            return Json(new { success = true, users = users }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMessages(string forId)
        {
            if (string.IsNullOrEmpty(forId))
                return Json(new { success = true, messages = new System.Collections.Generic.List<Entities.Message>() }, JsonRequestBehavior.AllowGet);

            var messages = new Service.SchwackService().GetMessages(new System.Guid(forId));
            return Json(new { success = true, messages = messages }, JsonRequestBehavior.AllowGet);
        }
    }
}