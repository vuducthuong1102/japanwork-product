using System.Configuration;
using SingleSignOn.DB.Sql.Repositories;
using SingleSignOn.DB.Sql.Entities;
using System.Collections.Generic;

namespace SingleSignOn.DB.Sql.Stores
{
    public class StoreUser : IStoreUser
    {
        private readonly string _connectionString;
        private RpsUser myRepository;

        public StoreUser()
            : this("SingleSignOnDB")
        {

        }

        public StoreUser(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsUser(_connectionString);
        }
        public List<IdentityUser> GetByPage(IdentityFilter identity)
        {
            return myRepository.GetByPage(identity);
        }

            public IdentityUser GetUserById(int userId)
        {
            return myRepository.GetUserById(userId);
        }

        public List<IdentityUser> GetListUserInfo(string listUserId)
        {
            return myRepository.GetListUserInfo(listUserId);
        }

        public IdentityUser GetUserByInfo(string userInfo)
        {
            return myRepository.GetUserByInfo(userInfo);
        }

        public IdentityUser GetUserByUserName(string userName)
        {
            return myRepository.GetUserByUserName(userName);
        }

        public IdentityUser GetUserByEmail(string email)
        {
            return myRepository.GetUserByEmail(email);
        }

        public bool ProvideTokenKeyForUser(UserTokenIdentity identity)
        {
            return myRepository.ProvideTokenKeyForUser(identity);
        }

        public bool WriteUserLog(UserLogIdentity identity, bool allowToWrite = true)
        {
            return myRepository.WriteUserLog(identity, allowToWrite);
        }

        public UserTokenIdentity GetCurrentTokenKeyByUser(int userId)
        {
            return myRepository.GetCurrentTokenKeyByUser(userId);
        }

        public int RefreshTokenKey(UserTokenIdentity identity)
        {
            return myRepository.RefreshTokenKey(identity);
        }

        public int CreateOTPCode(UserCodeIdentity identity)
        {
            return myRepository.CreateOTPCode(identity);
        }

        public UserCodeIdentity GetCurrentOTP(UserCodeIdentity identity)
        {
            return myRepository.GetCurrentOTP(identity);
        }

        public int VerifyOTPCode(UserCodeIdentity identity)
        {
            return myRepository.VerifyOTPCode(identity);
        }

        //Personal
        public WebAuthUserLoginIdentity WebLogin(IdentityUser identity)
        {
            return myRepository.WebLogin(identity);
        }

        public ApiAuthUserLoginIdentity ApiLogin(IdentityUser identity)
        {
            return myRepository.ApiLogin(identity);
        }

        public ApiAuthUserLoginIdentity ApiLoginWith(IdentityUser identity)
        {
            return myRepository.ApiLoginWith(identity);
        }

        public int ApiRegister(IdentityUser identity, ref int code)
        {
            return myRepository.ApiRegister(identity, ref code);
        }

        public int ChangePassword(IdentityUser identity, int passwordLevel)
        {
            return myRepository.ChangePassword(identity, passwordLevel);
        }

        public int UpdateProfile(IdentityUser identity)
        {
            return myRepository.UpdateProfile(identity);
        }

        public int WebRegister(IdentityUser identity, ref int code)
        {
            return myRepository.WebRegister(identity, ref code);
        }

        public int CheckPwd2IsValid(IdentityUser identity)
        {
            return myRepository.CheckPwd2IsValid(identity);
        }

        public int RecoverPasswordStep1(int userId, string pwd)
        {
            return myRepository.RecoverPasswordStep1(userId, pwd);
        }

        public int RecoverPasswordStep2(int userId, string pwdType)
        {
            return myRepository.RecoverPasswordStep2(userId, pwdType);
        }

        public IdentityUser ApiGetUserInfo(int userId, string token)
        {
            return myRepository.ApiGetUserInfo(userId, token);
        }

        public List<IdentityUser> GetTopTraveller(int userId, string token, int numberTop)
        {
            return myRepository.GetTopTraveller(userId, token, numberTop);
        }

        public int UserFollow(IdentityUserAction identity)
        {
            return myRepository.UserFollow(identity);
        }

        public IdentityUser GetUserProfile(int id)
        {
            return myRepository.GetUserProfile(id);
        }

        public int ActiveAccountByEmail(IdentityActiveAccount info)
        {
            return myRepository.ActiveAccountByEmail(info);
        }

        public int ActiveAccountByOTP(IdentityActiveAccount info)
        {
            return myRepository.ActiveAccountByOTP(info);
        }

        public bool UpdateAvatar(IdentityUser identity)
        {
            return myRepository.UpdateAvatar(identity);
        }
    }

    public interface IStoreUser
    {
        List<IdentityUser> GetByPage(IdentityFilter identity);
        IdentityUser GetUserById(int userId);
        IdentityUser GetUserByInfo(string userInfo);
        List<IdentityUser> GetListUserInfo(string listUserId);
        IdentityUser GetUserByUserName(string userName);
        IdentityUser GetUserByEmail(string email);
        bool ProvideTokenKeyForUser(UserTokenIdentity identity);
        bool WriteUserLog(UserLogIdentity identity, bool allowToWrite = true);
        UserTokenIdentity GetCurrentTokenKeyByUser(int userId);
        int RefreshTokenKey(UserTokenIdentity identity);

        int CreateOTPCode(UserCodeIdentity identity);
        UserCodeIdentity GetCurrentOTP(UserCodeIdentity identity);
        int VerifyOTPCode(UserCodeIdentity identity);

        //Personal
        WebAuthUserLoginIdentity WebLogin(IdentityUser identity);
        ApiAuthUserLoginIdentity ApiLogin(IdentityUser identity);
        ApiAuthUserLoginIdentity ApiLoginWith(IdentityUser identity);
        int ApiRegister(IdentityUser identity, ref int code);
        int ChangePassword(IdentityUser identity, int passwordLevel);
        int UpdateProfile(IdentityUser identity);

        int WebRegister(IdentityUser identity, ref int code);

        int CheckPwd2IsValid(IdentityUser identity);
        int RecoverPasswordStep1(int userId, string pwd);
        int RecoverPasswordStep2(int userId, string pwdType);

        IdentityUser ApiGetUserInfo(int userId, string token);

        List<IdentityUser> GetTopTraveller(int userId, string token, int numberTop);

        int UserFollow(IdentityUserAction identity);
        IdentityUser GetUserProfile(int id);

        //Active account
        int ActiveAccountByEmail(IdentityActiveAccount info);
        int ActiveAccountByOTP(IdentityActiveAccount info);

        bool UpdateAvatar(IdentityUser identity);
    }
}
