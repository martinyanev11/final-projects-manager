// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FinalProjectManager.Data.Constants;
using FinalProjectManager.Data.Data;
using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace FinalProjectManager.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ISpecialisationService _specialisationService;
        private readonly ApplicationDbContext _context;

        private static readonly string[] ClassLetters =
        [
            "А","Б","В","Г","Д","Е","Ж","З","И","Й",
            "К","Л","М","Н","О","П","Р","С","Т","У",
            "Ф","Х","Ц","Ч","Ш","Щ","Ъ","Ю","Я"
        ];

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ISpecialisationService specialisationService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _specialisationService = specialisationService;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public IEnumerable<SelectListItem> Specialisations { get; set; }
        public IEnumerable<SelectListItem> ClassLetterItems { get; set; }

        public class InputModel
        {
            [Required, MaxLength(50), Display(Name = "Първо Ime")]
            public string FirstName { get; set; }

            [Required, MaxLength(50), Display(Name = "Презиме")]
            public string MiddleName { get; set; }

            [Required, MaxLength(50), Display(Name = "Фамилия")]
            public string LastName { get; set; }

            [Required, EmailAddress, Display(Name = "Имейл")]
            public string Email { get; set; }

            [Required, Display(Name = "Специализация")]
            public int SpecialisationId { get; set; }

            [Required, Display(Name = "Клас")]
            public string ClassLetter { get; set; }

            [Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password), Display(Name = "Парола")]
            public string Password { get; set; }

            [DataType(DataType.Password), Compare("Password"), Display(Name = "Потвърди паролата")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            await PopulateDropdownsAsync();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return Page();
            }

            var fullName = $"{Input.FirstName} {Input.MiddleName} {Input.LastName}".Trim();

            var user = new ApplicationUser
            {
                FullName = fullName,
                IsApproved = true,
                EmailConfirmed = true
            };

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, AppRoles.Student);

                _context.Students.Add(new Student
                {
                    FullName = fullName,
                    Email = Input.Email,
                    SpecialisationId = Input.SpecialisationId,
                    ClassName = Input.ClassLetter
                });
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            await PopulateDropdownsAsync();
            return Page();
        }

        private async Task PopulateDropdownsAsync()
        {
            var specs = await _specialisationService.GetAllAsync();
            Specialisations = specs.Select(s => new SelectListItem(s.Name, s.Id.ToString()));
            ClassLetterItems = ClassLetters.Select(l => new SelectListItem($"12{l}", l));
        }

        private ApplicationUser CreateUser()
        {
            try { return Activator.CreateInstance<ApplicationUser>(); }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("The default UI requires a user store with email support.");
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
