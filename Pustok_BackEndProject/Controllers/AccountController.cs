using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using MimeKit;
using NuGet.Protocol;
using Pustok_BackEndProject.Areas.Manage.ViewModels.AccountViewModels;
using Pustok_BackEndProject.Models;
using Pustok_BackEndProject.ViewModels;
using System.Data;

namespace Pustok_BackEndProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _con;
        private readonly SmtpSetting _smtpSetting;

        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration con,IOptions<SmtpSetting>smtpSetting)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _con = con;
            _smtpSetting = smtpSetting.Value;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            AppUser appUser = new AppUser
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                SurName = registerVM.SurName,
                Name = registerVM.Name,
                IsActive = true

            };

            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(appUser, "Member");

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            string url = Url.Action("EmailConfirm", "Account", new { id = appUser.Id, token = token }, HttpContext.Request.Scheme,HttpContext.Request.Host.ToString());

            await _userManager.AddToRoleAsync(appUser, "Member");
            string templateFullPath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "_EmailConfirm.cshtml");
            string templateContent = await System.IO.File.ReadAllTextAsync(templateFullPath);
            templateContent = templateContent.Replace("{{name}}", appUser.Name);
            templateContent = templateContent.Replace("{{url}}", url);

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            //mimeMessage.From.Add(MailboxAddress.Parse(_con.GetSection("SmtpSetting:Email").Value));
            //mimeMessage.From.Add(MailboxAddress.Parse("faridmnsvr@gmail.com"));
            mimeMessage.To.Add(MailboxAddress.Parse(appUser.Email));
            mimeMessage.Subject = "Email Confirmation";
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "<a href='http://localhost:21473/Account/login'>Go to Pustok man</a>"
            };

            using (SmtpClient smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync( _smtpSetting.Host, _smtpSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_smtpSetting.Email ,_smtpSetting.Password);
                await smtpClient.SendAsync(mimeMessage);
                await smtpClient.DisconnectAsync(true);
                smtpClient.Dispose();
            }

            //using (SmtpClient smtpClient = new SmtpClient())
            //{
            //    await smtpClient.ConnectAsync(_con.GetSection("SmtpSetting:Host").Value,int.Parse(_con.GetSection("SmtpSetting:Port").Value) , MailKit.Security.SecureSocketOptions.StartTls);
            //    await smtpClient.AuthenticateAsync(_con.GetSection("SmtpSetting:Email").Value, _con.GetSection("SmtpSetting:Password").Value);
            //    await smtpClient.SendAsync(mimeMessage);
            //    await smtpClient.DisconnectAsync(true);
            //    smtpClient.Dispose();
            //}


            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {

            if (!ModelState.IsValid) return View(loginVM);

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email ve ya Sifre Yalnisdir");
                return View(loginVM);
            }
            if (!appUser.IsActive)
            {
                ModelState.AddModelError("", "Hesabiniz DeActivedi");
                return View(loginVM);
            }
            //if (!await _userManager.CheckPasswordAsync(appUser,loginVM.Password))
            //{
            //    ModelState.AddModelError("", "Email ve ya Sifre Yalnisdir");
            //    return View(loginVM);
            //}

            Microsoft.AspNetCore.Identity.SignInResult signInResult = null;
            if (appUser.EmailConfirmed)
            {
                 signInResult = await _signInManager
                .PasswordSignInAsync(appUser, loginVM.Password, loginVM.RememberMe, true);
            }
            else
            {
                ModelState.AddModelError("", "Check your Email");
                return View();
            }

            if (appUser.LockoutEnd > DateTime.UtcNow)
            {
                ModelState.AddModelError("", "Hesabiniz Bloklanib");
                return View(loginVM);
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email ve ya Sifre Yalnisdir");
                return View(loginVM);
            }

            return RedirectToAction("Index", "Dashboard", new { areas = "Manage" });
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            ProfileVM profileVM = new ProfileVM
            {
                Name = appUser.Name,
                SurName = appUser.SurName,
                UserName = appUser.UserName,
                Email = appUser.Email
            };
            return View(profileVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {

            if (!ModelState.IsValid)
            {
                return View(profileVM);
            }


            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            appUser.Name = profileVM.Name;
            appUser.SurName = profileVM.SurName;


            if (appUser.NormalizedUserName != profileVM.UserName.Trim().ToUpperInvariant())
            {
                if (await _userManager.Users.AnyAsync(u => u.NormalizedUserName == profileVM.UserName.Trim().ToUpperInvariant() && u.Id != appUser.Id))
                {
                    ModelState.AddModelError("UserName", $"User Name {profileVM.UserName} Already Taken");
                    return View(profileVM);
                }
                else
                {
                    appUser.UserName = profileVM.UserName;
                }

            }
            if (appUser.NormalizedEmail != profileVM.Email.Trim().ToUpperInvariant())
            {
                if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == profileVM.Email.Trim().ToUpperInvariant()))
                {
                    ModelState.AddModelError("Email", $"Email {profileVM.Email} Already Taken");
                    return View(profileVM);
                }
                else
                {
                    appUser.Email = profileVM.Email;
                }

            }


            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);

                }
                return View(profileVM);
            }

            if (!string.IsNullOrWhiteSpace(profileVM.CurrentPassword) && await _userManager.CheckPasswordAsync(appUser, profileVM.CurrentPassword))
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

                identityResult = await _userManager.ResetPasswordAsync(appUser, token, profileVM.Password);
                if (!identityResult.Succeeded)
                {
                    foreach (IdentityError identityError in identityResult.Errors)
                    {
                        ModelState.AddModelError("", identityError.Description);

                    }
                    return View(profileVM);
                }
            }
            else
            {
                ModelState.AddModelError("CurrentPassword", $"CurrentPassword Yalnisdir");
                return View(profileVM);
            }


            await _signInManager.SignInAsync(appUser, true);

            return RedirectToAction("Index", "Dashboard", new { areas = "Manage" });
        }

        [HttpGet]
        public async Task<IActionResult>EmailConfirm(string? id,string? token)
        {

            if (string.IsNullOrWhiteSpace(id)) return BadRequest();
            if (string.IsNullOrWhiteSpace(token)) return BadRequest();
           
            AppUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null) return NotFound();

            IdentityResult identityResult = await _userManager.ConfirmEmailAsync(appUser, token);
            if (identityResult.Succeeded) return BadRequest();
            TempData["Success"] = $"{appUser.Email} Successed"; 


            return RedirectToAction(nameof(Login));
        }
    }
}
