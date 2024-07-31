// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TestWebApplication.Data;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly string UploadsDirectory = "wwwroot/images";

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Job Title")]
            public string JobTitle { get; set; }

            [Display(Name = "Address")]
            public string Address { get; set; }

            [Display(Name = "City")]
            public string City { get; set; }

            [Display(Name = "State")]
            public string State { get; set; }

            [Display(Name = "Country")]
            public string Country { get; set; }

            [Display(Name = "Profile Image")]
            public IFormFile ProfileImage { get; set; }
            public string? ImgUrl { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Email = userName;

            var userProfile = await _context.UserProfile.Where(x => x.Email == Email).
                Select(x => new UserProfile
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    JobTitle = x.JobTitle,
                    Phone = x.Phone,
                    Address = x.Address,
                    City = x.City,
                    State = x.State,
                    Country = x.Country,
                    ImgUrl = x.ImgUrl
                }).FirstOrDefaultAsync();


            Input = new InputModel
            {
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                JobTitle = userProfile.JobTitle,
                PhoneNumber = userProfile.Phone,
                Address = userProfile.Address,
                City = userProfile.City,
                State = userProfile.State,
                Country = userProfile.Country,
                ImgUrl = userProfile.ImgUrl
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            //if (Input.PhoneNumber != phoneNumber)
            //{
            //    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            //    if (!setPhoneResult.Succeeded)
            //    {
            //        StatusMessage = "Unexpected error when trying to set phone number.";
            //        return RedirectToPage();
            //    }
            //}

            var Id = _userManager.GetUserId(User);
            var userProfile = await _context.UserProfile.Where(x => x.Id == Id).
                            Select(x => new UserProfile
                            {
                                Email = x.Email,
                                FirstName = x.FirstName,
                                LastName = x.LastName,
                                JobTitle = x.JobTitle,
                                Phone = x.Phone,
                                Address = x.Address,
                                City = x.City,
                                State = x.State,
                                Country = x.Country,
                                ImgUrl = x.ImgUrl
                            }).FirstOrDefaultAsync();

            string uniqueFileName = UploadFile(Input);

            if (Input.FirstName != userProfile.FirstName || Input.LastName != userProfile.LastName || Input.JobTitle != userProfile.JobTitle
                || Input.PhoneNumber != userProfile.Phone || Input.Address != userProfile.Address || Input.City != userProfile.City ||
                Input.State != userProfile.State || Input.Country != userProfile.Country || Input.ProfileImage != userProfile.ImgFile)
            {
                var res = new UserProfile()
                {
                    Id = Id,
                    Email = userProfile.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    JobTitle = Input.JobTitle,
                    Phone = Input.PhoneNumber,
                    Address = Input.Address,
                    City = Input.City,
                    State = Input.State,
                    Country = Input.Country,
                    ImgUrl = uniqueFileName
                };

                _context.UserProfile.Update(res);
                _context.SaveChanges();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public string UploadFile(InputModel input)
        {
            var uniqueFile = "";
            if (input.ProfileImage != null)
            {
                //string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath,"images");
                uniqueFile = input.ProfileImage.FileName;
                var filePath = Path.Combine(UploadsDirectory, uniqueFile);
                using (var fileSteam = new FileStream(filePath, FileMode.Create))
                {
                    input.ProfileImage.CopyTo(fileSteam);
                }
            }
            return uniqueFile;
        }
    }
}
