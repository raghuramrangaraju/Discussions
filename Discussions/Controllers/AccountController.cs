using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Discussions.Models;
using Facebook;
using Newtonsoft.Json;
using System.Web.Security;


namespace Discussions.Controllers
{

    public class AccountController : Controller
    {

        [AllowAnonymous]
        public ActionResult signup(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public ActionResult signup(Signup signup)
        {
            // Registering new user
            StructuredDiscussionsEntities context = new StructuredDiscussionsEntities();
            User newuser = new User();
            newuser.EmailId = signup.email;
            newuser.CreatedAt = DateTime.UtcNow;
            newuser.FirstName = signup.Firstname;
            newuser.LastName = signup.Lastname;
            newuser.Password = signup.password;
            newuser.Status = true;
            context.Users.Add(newuser);
            context.SaveChanges();
            string message = "User has been successfully registered";
            return RedirectToAction("login", new { returnUrl = "", successregister = message });
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string successregister ="")
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.message = successregister;

            return View();
        }


        //
        // POST: /Account/Login
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    StructuredDiscussionsEntities context = new StructuredDiscussionsEntities();
        //    var result = context.Users.Where(x => x.EmailId == model.Email && x.Password == model.Password).FirstOrDefault();
        //    if (result != null)
        //    {
        //        Session["UserId"] = result.UserId;
        //        Session["UserName"] = result.UserName;
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else {
        //        ModelState.AddModelError("", "Invalid login credentials");
        //        return View(model);
        //    }
        //}

        public bool IsValid(string username, string password)
        {
            StructuredDiscussionsEntities context = new StructuredDiscussionsEntities();
            return context.Users.Any(x => x.EmailId == username && x.Password == password);

        }

        [HttpPost]
        [AllowAnonymous]
        //   [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (IsValid(model.Email, model.Password))
            {
                var ident = new ClaimsIdentity(new[] {
              new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, model.Email),
              new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, model.Email),
              new System.Security.Claims.Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
              new System.Security.Claims.Claim(ClaimTypes.Name,model.Email),
              new System.Security.Claims.Claim(ClaimTypes.Role, "User"),
                }, DefaultAuthenticationTypes.ApplicationCookie);

                HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
                StructuredDiscussionsEntities context = new StructuredDiscussionsEntities();
                var result = context.Users.Where(x => x.EmailId == model.Email).FirstOrDefault();
                Session["UserId"] = result.UserId;
                Session["UserName"] = model.Email;
                return RedirectToAction("Index", "Home");
            }
            // invalid username or password
            ModelState.AddModelError("", "invalid username or password");
            return View();
        }


        // POST: /Account/LogOff
        [HttpGet]
        //    [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();

            //FormsAuthentication.SignOut();
            Session.Clear();
            //  AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        ////
        //// GET: /Account/Register
        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

        //            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}


        private Uri RediredtUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }


        [AllowAnonymous]
        public ActionResult Facebook()
        {
            var fb = new FacebookClient();
            var loginurl = fb.GetLoginUrl(new
            {
                client_id = "389477284749373",
                client_secret = "22237e124c24ca7268c8aa511429468c",
                redirect_uri = RediredtUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });
            return Redirect(loginurl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "389477284749373",
                client_secret = "22237e124c24ca7268c8aa511429468c",
                redirect_uri = RediredtUri.AbsoluteUri,
                code = code

            });
            var accessToken = result.access_token;
            Session["AccessToken"] = accessToken;
            fb.AccessToken = accessToken;
            dynamic me = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,locale,timezone,verified,picture,age_range");
            string email = me.email;
            TempData["email"] = me.email;
            
            Session["picture"] = me.picture.data.url;
            Session["UserName"] = me.first_name;
            StructuredDiscussionsEntities context = new StructuredDiscussionsEntities();
            var data = context.Users.Where(x => x.EmailId == email).FirstOrDefault();
            if (data == null)// creating new user
            {
                User newuser = new User();
                newuser.EmailId = email;
                newuser.FirstName = me.first_name;
                newuser.LastName = me.last_name;
                newuser.CreatedAt = DateTime.UtcNow;
                newuser.Status = true;
                context.Users.Add(newuser);
                context.SaveChanges();
                Session["UserId"] = newuser.UserId;
            }
            else {
                Session["UserId"] = data.UserId;
            }
            Session["UserName"] = me.email;
            var ident = new ClaimsIdentity(new[] {
              new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, me.email),
              new System.Security.Claims.Claim(ClaimTypes.NameIdentifier,  me.email),
              new System.Security.Claims.Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
              new System.Security.Claims.Claim(ClaimTypes.Name, me.email),
              new System.Security.Claims.Claim(ClaimTypes.Role, "User"),
                }, DefaultAuthenticationTypes.ApplicationCookie);

            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
            return RedirectToAction("Index", "Home");
        }
    }
}
