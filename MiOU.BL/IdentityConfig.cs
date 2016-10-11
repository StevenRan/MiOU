using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MiOU.DAL;
using System.Security.Claims;

namespace MiOU.BL
{

    public class ApplicationUserManager : UserManager<ApplicationUser, long>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, long> store)
            : base(store)
        {

        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            ApplicationUserStore store = new ApplicationUserStore(new MiOU.DAL.MiOUEntities());
            var manager = new ApplicationUserManager(store);
            manager.PasswordHasher = new MiOUPasswordHasher();
            // Configure validation logic for usernames
            //manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            //{
            //    AllowOnlyAlphanumericUserNames = false,
            //    RequireUniqueEmail = true
            //};

            //// Configure validation logic for passwords
            //manager.PasswordValidator = new PasswordValidator
            //{
            //    RequiredLength = 6,
            //    RequireNonLetterOrDigit = true,
            //    RequireDigit = true,
            //    RequireLowercase = true,
            //    RequireUppercase = true,
            //};

            //// Configure user lockout defaults
            //manager.UserLockoutEnabledByDefault = true;
            //manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            //manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            //// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            //// You can write your own provider and plug it in here.
            //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            //{
            //    MessageFormat = "Your security code is {0}"
            //});
            //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            //{
            //    Subject = "Security Code",
            //    BodyFormat = "Your security code is {0}"
            //});
            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();
            //var dataProtectionProvider = options.DataProtectionProvider;
            //if (dataProtectionProvider != null)
            //{
            //    manager.UserTokenProvider = 
            //        new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            //}
            return manager;
        }

        public override Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return base.CheckPasswordAsync(user, password);
        }

        public ApplicationUser FindUserByOpenId(string openId,string provider)
        {
            ApplicationUser user = null;
            return user;
        }

        protected override async Task<bool> VerifyPasswordAsync(IUserPasswordStore<ApplicationUser, long> store, ApplicationUser user, string password)
        {
            string hashedPassword = await store.GetPasswordHashAsync(user);
            PasswordVerificationResult ret = this.PasswordHasher.VerifyHashedPassword(hashedPassword, password);
            return ret != PasswordVerificationResult.Failed;
        }
    }
    public class ApplicationSignInManager : SignInManager<ApplicationUser, long>
    {
        public ApplicationSignInManager(UserManager<ApplicationUser, long> userManager, Microsoft.Owin.Security.IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            ApplicationUserManager userManager = context.GetUserManager<ApplicationUserManager>();
            //userManager.PasswordHasher = new QALinkPasswordHasher();
            return new ApplicationSignInManager(userManager, context.Authentication);
        }

        public async Task<SignInStatus> ExternalSignIn(ExternalLoginInfo loginInfo,bool isPersistent)
        {
            var user = await UserManager.FindAsync(loginInfo.Login);

            if (user == null)
            {
                return SignInStatus.Failure;
            }
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalBearer);
            await SignInAsync(user, isPersistent, false);
            return SignInStatus.Success;
        }
    }

    public class MiOUPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return password;
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword))
            {
                return PasswordVerificationResult.Failed;
            }
            if (string.IsNullOrEmpty(providedPassword))
            {
                return PasswordVerificationResult.Failed;
            }
            if (hashedPassword.Trim() != providedPassword.Trim())
            {
                return PasswordVerificationResult.Failed;
            }

            return PasswordVerificationResult.Success;
        }
    }

    public class ApplicationUserStore : IUserStore<ApplicationUser, long>,
        IUserPasswordStore<ApplicationUser, long>,
        IUserSecurityStampStore<ApplicationUser, long>,
        IUserLockoutStore<ApplicationUser, long>,
        IUserPhoneNumberStore<ApplicationUser, long>,
        IUserLoginStore<ApplicationUser, long>,
        Microsoft.AspNet.Identity.IUserTwoFactorStore<ApplicationUser, long>
    {
        MiOUEntities content;
        UserStore<IdentityUser> userStore;
        public ApplicationUserStore(MiOUEntities _context)
        {
            this.content = _context;
            if (this.content == null)
            {
                this.content = new MiOUEntities();
            }
            this.userStore = new UserStore<IdentityUser>(this.content);
        }

        public Task CreateAsync(ApplicationUser user)
        {
            User dbUser = ApplicationUser.AppUserToDBUser(user);
            content.User.Add(dbUser);
            user.Id = dbUser.Id;
            return content.SaveChangesAsync();
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (this.content != null)
            {
                this.content.Dispose();
            }
        }

        public Task<ApplicationUser> FindByIdAsync(long userId)
        {
            User user = content.User.Find(userId);
            return Task.FromResult(ApplicationUser.DBUserToAppUser(user));
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var tmp = content.User.Where(us => us.NickName == userName);
            User dbUser = tmp.FirstOrDefault<User>();
            return Task.FromResult(ApplicationUser.DBUserToAppUser(dbUser));
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var tmp = content.User.Where(us => us.Email == email);
            User dbUser = tmp.FirstOrDefault<User>();
            return Task.FromResult(ApplicationUser.DBUserToAppUser(dbUser));
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new Exception("Please login with valid user name and password");
            }
            return Task.FromResult(user.Password);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new Exception("Please login with valid user name and password");
            }
            if (string.IsNullOrEmpty(user.Password))
            {
                throw new Exception("No password for this user");
            }
            return Task.FromResult(true);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            user.Password = passwordHash;
            return Task.FromResult(0);
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            User dbUser = (from u in content.User where u.Id == user.Id select u).FirstOrDefault<User>();
            if (dbUser != null)
            {
                Task task = content.SaveChangesAsync();
                return task;
            }
            return null;
        }

        private static void SetApplicationUser(ApplicationUser user, IdentityUser identityUser)
        {
            //user.PasswordHash = identityUser.PasswordHash;
            //user.SecurityStamp = identityUser.SecurityStamp;
            user.Id = int.Parse(identityUser.Id);
            user.UserName = identityUser.UserName;
        }

        private IdentityUser ToIdentityUser(ApplicationUser user)
        {
            return new IdentityUser
            {
                Id = user.Id.ToString(),
                //PasswordHash = user.PasswordHash,
                //SecurityStamp = user.SecurityStamp,
                UserName = user.UserName
            };
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp)
        {
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            return Task.FromResult(0);
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            return Task.FromResult(true);
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            return Task.FromResult(0);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            return Task.FromResult(false);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            return Task.FromResult(true);
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            return Task.FromResult(false);
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber)
        {
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user)
        {
            return Task.FromResult("");
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user)
        {
            return Task.FromResult(true);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            return Task.FromResult(0);
        }

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            //if (user == null)
            //{
            //    throw new ArgumentNullException("user");
            //}

            //if (login == null)
            //{
            //    throw new ArgumentNullException("login");
            //}

            //UserLogins log = new UserLogins() { LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey, UserId = user.Id };
            //this.content.UserLogins.Add(log);
            //this.content.SaveChanges();
            return Task.FromResult<object>(null);
        }

        public Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            return Task.FromResult<Object>(null);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            List<UserLoginInfo> userLogins = new List<UserLoginInfo>();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            //List<UserLogins> ulogins = (from ul in content.UserLogins where ul.UserId == user.Id select ul).ToList<UserLogins>();
            //List<UserLoginInfo> logins = null;
            //if (ulogins != null)
            //{
            //    logins = new List<UserLoginInfo>();
            //    foreach (UserLogins l in ulogins)
            //    {
            //        logins.Add(new UserLoginInfo(l.LoginProvider, l.ProviderKey));
            //    }
            //    return Task.FromResult<IList<UserLoginInfo>>(logins);
            //}

            return Task.FromResult<IList<UserLoginInfo>>(null);
        }

        public Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }
            int type = 0;
            switch(login.LoginProvider)
            {
                case "WeChat":
                    type = 1;
                    break;
                default:
                    break;
            }
            if(type==0)
            {
                return Task.FromResult<ApplicationUser>(null);
            }
            User dbUser = (from u in content.User where u.ExternalUserType== type && u.ExternalUserId==login.ProviderKey select u).FirstOrDefault<User>();
            ApplicationUser appUser = ApplicationUser.DBUserToAppUser(dbUser);
            return Task.FromResult<ApplicationUser>(appUser);
        }
    }

    public class ApplicationUser : User, Microsoft.AspNet.Identity.IUser<long>
    {       
        public string UserName
        {
            get
            {
                return this.NickName;
            }

            set
            {
                this.NickName = value;
            }
        }

        public long Id
        {
            get
            {
                return this.Id;
            }
            set
            {
                this.Id = value;
            }
        }

        public static ApplicationUser DBUserToAppUser(User dbUser)
        {
            if (dbUser == null)
            {
                return null;
            }

            ApplicationUser appUser = new ApplicationUser();
            System.Reflection.PropertyInfo[] properties = appUser.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                System.Reflection.PropertyInfo p = dbUser.GetType().GetProperty(property.Name);
                if (p != null)
                {
                    property.SetValue(appUser, p.GetValue(dbUser));
                }
            }
            appUser.Id = dbUser.Id;
            return appUser;
        }

        public static User AppUserToDBUser(ApplicationUser appUser)
        {
            if (appUser == null)
            {
                return null;
            }

            User dbUser = new User();
            dbUser.Id = (int)appUser.Id;
            System.Reflection.PropertyInfo[] properties = dbUser.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                System.Reflection.PropertyInfo p = appUser.GetType().GetProperty(property.Name);
                if (p != null)
                {
                    property.SetValue(dbUser, p.GetValue(appUser));
                }
            }

            return dbUser;
        }
    }
}
