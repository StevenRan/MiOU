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
using MiOU.Util;
namespace MiOU.BL
{

    public class ApplicationUserManager : UserManager<ApplicationUser, int>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, int> store)
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

        public virtual Task<ApplicationUser> FindByNickNameAsync(string nickName)
        {
            
            Task<ApplicationUser> user = null;
            ApplicationUserStore store = (ApplicationUserStore)this.Store;
            user = store.FindByNickAsync(nickName);
            return user;
        }
        public override Task<ApplicationUser> FindByEmailAsync(string email)
        {
            Task<ApplicationUser> user = null;
            ApplicationUserStore store = (ApplicationUserStore)this.Store;
            user = store.FindByEmailAsync(email);
            return user;
        }
        public ApplicationUser FindUserByOpenId(string openId,string provider)
        {
            ApplicationUser user = null;
            return user;
        }
        protected override async Task<bool> VerifyPasswordAsync(IUserPasswordStore<ApplicationUser, int> store, ApplicationUser user, string password)
        {
            string hashedPassword = await store.GetPasswordHashAsync(user);
            PasswordVerificationResult ret = this.PasswordHasher.VerifyHashedPassword(hashedPassword, password);
            return ret != PasswordVerificationResult.Failed;
        }

        public override Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            IUserLoginStore<ApplicationUser, int> store = (IUserLoginStore<ApplicationUser, int>)this.Store;
            return store.FindAsync(login);
        }

        public ApplicationUser FindExternalUser(string provider,string id)
        {
            ApplicationUser user = null;
            int type = 0;
            switch (provider)
            {
                case "WeChat":
                    type = 1;
                    break;
                default:
                    break;
            }
            if (type == 0)
            {
                return null;
            }
            using (MiOUEntities content = new MiOUEntities())
            {
                User dbUser = (from u in content.User where u.ExternalUserType == type && u.ExternalUserId == id select u).FirstOrDefault<User>();
                user = ApplicationUser.DBUserToAppUser(dbUser);
            }
            return user;
        }
    }
    public class ApplicationSignInManager : SignInManager<ApplicationUser, int>
    {
        public ApplicationSignInManager(UserManager<ApplicationUser, int> userManager, Microsoft.Owin.Security.IAuthenticationManager authenticationManager)
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
        private string passwordSalt = "5c149dc0-3a0f-438f-ae70-273575d2e66d";
        public string HashPassword(string password)
        {
            return UrlSignUtil.GetMD5(UrlSignUtil.GetMD5(password)+passwordSalt);
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
            if (hashedPassword.Trim() != HashPassword(providedPassword).Trim())
            {
                return PasswordVerificationResult.Failed;
            }

            return PasswordVerificationResult.Success;
        }
    }

    public class ApplicationUserStore : IUserStore<ApplicationUser, int>,
        IUserPasswordStore<ApplicationUser, int>,
        IUserSecurityStampStore<ApplicationUser, int>,
        IUserLockoutStore<ApplicationUser, int>,
        IUserPhoneNumberStore<ApplicationUser, int>,
        IUserLoginStore<ApplicationUser, int>,
        Microsoft.AspNet.Identity.IUserTwoFactorStore<ApplicationUser, int>
    {
        public MiOUEntities content { get; private set; }
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

        public Task<ApplicationUser> FindByIdAsync(int userId)
        {
            User user = content.User.Find(userId);
            return Task.FromResult(ApplicationUser.DBUserToAppUser(user));
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var tmp = content.User.Where(us => us.Email == userName);
            User dbUser = tmp.FirstOrDefault<User>();
            return Task.FromResult(ApplicationUser.DBUserToAppUser(dbUser));
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var tmp = content.User.Where(us => us.Email == email);
            User dbUser = tmp.FirstOrDefault<User>();
            return Task.FromResult(ApplicationUser.DBUserToAppUser(dbUser));
        }
        public Task<ApplicationUser> FindByNickAsync(string nickName)
        {
            if(string.IsNullOrEmpty(nickName))
            {
                throw new ArgumentNullException("NickName cannot be empty");
            }
            var tmp = content.User.Where(us => us.NickName == nickName);
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
            User dbUser = (from u in content.User where u.UserId == user.Id select u).FirstOrDefault<User>();
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

    public class ApplicationUser : User, Microsoft.AspNet.Identity.IUser<int>
    {       
        public string UserName
        {
            get
            {
                return this.Email;
            }
            set
            {
                this.Email = value;
            }
        }

        public int Id
        {
            get
            {
                return this.UserId;
            }
            set
            {
                this.UserId = value;
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
            appUser.Id = dbUser.UserId;
            appUser.UserName = dbUser.Email;
            return appUser;
        }

        public static User AppUserToDBUser(ApplicationUser appUser)
        {
            if (appUser == null)
            {
                return null;
            }

            User dbUser = new User();
            dbUser.UserId = (int)appUser.Id;
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
