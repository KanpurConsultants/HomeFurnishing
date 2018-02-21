using Mailer;
using Mailer.Model;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class MailboxController : Controller
    {
        public ActionResult Inbox()
        {
            return View();
        }

        public ActionResult Compose(string ToEMail, string Subject, string Body, string attachment)
        {
            EmailMessage vm = new EmailMessage();
            vm.To = ToEMail;
            vm.Subject = Subject;
            vm.Body = Body;
            vm.Attachment = attachment;

            return View(vm);
        }

        [ValidateInput(false)]
        public ActionResult ComposePost(EmailMessage vm)
        {
            SendEmail.SendEmailMsgWithAttachment(vm,vm.Attachment);

            return View("Compose");
        }

        public ActionResult Read()
        {
            return View();
        }
    }
}