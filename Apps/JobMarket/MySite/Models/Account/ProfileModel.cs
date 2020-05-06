using MySite.Resources;
using Newtonsoft.Json;
using SingleSignOn.DB.Sql.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MySite.Models.Account
{
    public class ProfileModel
    {
        public IdentityUser UserInfo { get; set; }

        [Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.LB_OLD_PASSWORD))]
        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_PASSWORD_NULL))]
        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        public string OldPassword { get; set; }

        [Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.LB_NEW_PASSWORD))]
        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.VAL_ERROR_PASSWORD_NULL))]
        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        public string NewPassword { get; set; }

        [Display(ResourceType = typeof(Resources.UserWebResource), Name = nameof(UserWebResource.LB_CONFIRM_NEW_PASSWORD))]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_CONFIRMPWD_INCORRECT))]
        [MinLength(6, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_LESS_6_CHARACTERS))]
        [MaxLength(50, ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_OVER_50_CHARACTERS))]
        public string ConfirmNewPassword { get; set; }

        public List<IdentityUser> ListFollowers { get; set; }

        public List<IdentityUser> ListFollowings { get; set; }

        public int OwnerId { get; set; }
        
        public int UserId { get; set; }

        public int UserFriendId { get; set; }

        public int UserSendId { get; set; }

        public bool IsOwnerRequest { get; set; }

        public ProfileModel()
        {
            UserInfo = new IdentityUser();
        }
    }

    public class ApiUserModel
    {
        public int UserId { get; set; }

        public int CurrentUserId { get; set; }
    }

    public class ApiListUserInfoModel
    {
        /// <summary>
        /// ListUserId
        /// </summary>
        public List<int> ListUserId { get; set; }
    }
}