using OtpManager.Application.Abstract;
using OtpManager.Common;
using OtpManager.Common.Abstract;
using OtpManager.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using OtpManager.Application.Properties;

namespace OtpManager.Application
{
    /// <summary>
    /// Application logic for otp generation and checking.
    /// try-catch blocks are at this boundary
    /// </summary>
    public class CredentialsManager : ICredentialsManager<ApplicationRule>
    {
        private IRepository<User> _userRepo;
        private IOtpGenerator _otpGenerator;
        private ILogger _logger;

        public IList<ApplicationRule> ApplicationRules { get; private set; }

        public void AddRule(ApplicationRule applicationRule)
        {
            if (ApplicationRules == null)
                ApplicationRules = new List<ApplicationRule>();

            ApplicationRules.Add(applicationRule);
        }

        public void ResetRules()
        {
            ApplicationRules = null;
        }

        public CredentialsManager(IRepository<User> userRepository, IOtpGenerator otpGenerator, ILogger logger)
        {
            _userRepo = userRepository;
            _otpGenerator = otpGenerator;
            _logger = logger;
            if (_userRepo == null) throw new NullReferenceException("User Repository must be initialized");
            if (_otpGenerator == null) throw new NullReferenceException("Otp Generator must be initialized");
            if (_logger == null) throw new NullReferenceException("Logger must be initialized");
        }

        public string CreateOtp(string userId)
        {
            try
            {
                // datetime to check against
                DateTime now = DateTime.Now;
                string newPassword = _otpGenerator.Create();
                // Verify rules on userId
                // if ok generates password else null
                Func<IRepository<User>, bool> ok = repo =>
                {
                    Func<User, bool> duplicate = u =>
                    {
                        bool checkUserId = u?.UserId.ToLower() == userId.ToLower();
                        bool checkExpired = checkUserId && ((TimeSpan)(now - u.Otp?.StartDate)).TotalMilliseconds <= Settings.Default.ValidityMsec;
                        return checkExpired;
                    };
                    ApplicationRule existentRule = new ApplicationRule(this, repo.SingleOrDefault(u => duplicate(u)) == null, ReasonEnum.ElementDuplication);
                    ApplicationRule validationRule1 = new ApplicationRule(this, userId.Length <= Settings.Default.UserIdLength, ReasonEnum.UserIdLength);
                    ApplicationRule validationRule2 = new ApplicationRule(this, !string.IsNullOrEmpty(userId) && !string.IsNullOrWhiteSpace(userId), ReasonEnum.ElementValidation);
                    ApplicationRule validationRule3 = new ApplicationRule(this, !userId.Contains(' '), ReasonEnum.ElementValidation);
                    return existentRule & validationRule1 & validationRule2 & validationRule3;
                };
                if (ok(_userRepo))
                {
                    // check if element exists and update or insert
                    Func<User, bool> expiredDuplicate = u =>
                    {
                        bool checkUserId = u.UserId.ToLower() == userId.ToLower();
                        bool checkExpired = checkUserId && ((TimeSpan)(now - u.Otp?.StartDate)).TotalMilliseconds > Settings.Default.ValidityMsec;
                        return checkExpired;
                    };
                    User user = _userRepo.SingleOrDefault(u => expiredDuplicate(u));
                    if (user != null)
                    {
                        user.Otp.Password = newPassword;
                        user.Otp.StartDate = now;
                    }
                    else
                    {
                        Otp newOtp = new Otp { Password = newPassword, StartDate = now };
                        User newUser = _userRepo.Create();
                        newUser.UserId = userId;
                        newUser.Otp = newOtp;
                        _userRepo.Add(newUser);
                    }
                    _userRepo.SaveChanges();
                    return newPassword;
                }
                else return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                new ApplicationRule(this, false, ReasonEnum.Error);
                return null;
            }

        }

        public bool CheckOtp(string userId, string password)
        {
            try
            {
                DateTime now = DateTime.Now;
                Func<IRepository<User>, bool> ok = repo =>
                {
                    Func<User, bool> valid = u =>
                    {
                        bool checkExpired = ((TimeSpan)(now - u.Otp?.StartDate)).TotalMilliseconds <= Settings.Default.ValidityMsec;
                        return checkExpired;
                    };

                    ApplicationRule wrongUserRule = new ApplicationRule(this, repo.SingleOrDefault(u => u.UserId.ToLower() == userId.ToLower()) != null, ReasonEnum.WrongUser);
                    ApplicationRule expiredRule = new ApplicationRule(this, repo.SingleOrDefault(u => valid(u)) != null, ReasonEnum.ElementExpired);
                    ApplicationRule validationRule1 = new ApplicationRule(this, userId.Length <= Settings.Default.UserIdLength, ReasonEnum.UserIdLength);
                    ApplicationRule validationRule2 = new ApplicationRule(this, !string.IsNullOrEmpty(userId) && !string.IsNullOrWhiteSpace(userId), ReasonEnum.ElementValidation);
                    ApplicationRule passwordRule = new ApplicationRule(this, repo.SingleOrDefault(u => u.Otp.Password == password) != null, ReasonEnum.WrongPassword);
                    return wrongUserRule & expiredRule & validationRule1 & validationRule2 & passwordRule;
                };
                return ok(_userRepo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                new ApplicationRule(this, false, ReasonEnum.Error);
                return false;
            }
        }

    }
}
