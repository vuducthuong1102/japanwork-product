//using ApiMySiteSocial.DB.Sql.Entities;
//using MySite.Logging;
//using MySite.Resources;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;

//namespace MySite.Helpers
//{
//    public class PostHelper
//    {
//        private static readonly ILog logger = LogProvider.For<PostHelper>();

//        //public static List<IdentityImage> ParseImagesFromPost(IdentityPost post)
//        //{
//        //    try
//        //    {
//        //        if(post != null && !string.IsNullOrEmpty(post.Images))
//        //            return JsonConvert.DeserializeObject<List<IdentityImage>>(post.Images);

//        //        if (post == null)
//        //        {
//        //            logger.Error(string.Format("Failed when ParseImagesFromPost. The Post data is null"));
//        //            return null;
//        //        }                    
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        logger.Error(string.Format("Failed when ParseImagesFromPost [{0}] due to {1}", post.Id, ex.ToString()));
//        //    }

//        //    return null;
//        //}

//        public static string ParseTitleMetaFromPost(IdentityPost post,int totalLocations = 0)
//        {
//            var title = string.Empty;

//            var totalLocation = (totalLocations == 0) ? post.TotalLocations : totalLocations;
//            var locStr = string.Empty;
//            try
//            {
//                if (post.Days > 0)
//                {
//                    title = post.Days + " " + UserWebResource.LB_DAYS;
//                    if (totalLocation > 0)
//                    {
//                        title += ", " + totalLocation + " " + UserWebResource.LB_PLACE;
//                    }
//                }
//                else
//                {
//                    if (totalLocation > 0)
//                    {
//                        title = totalLocation + " " + UserWebResource.LB_PLACE;
//                    }
//                }

//                if (post == null)
//                {
//                    logger.Error(string.Format("Failed when ParseTitleMetaFromPost. The Post data is null"));
//                    return string.Empty;
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Error(string.Format("Failed when ParseTitleMetaFromPost [{0}] due to {1}", post.Id, ex.ToString()));
//            }

//            return title;
//        }

//        public static string ParseDescriptionFromPost(IdentityPost post, bool showFull = false)
//        {
//            try
//            {
//                if (post != null && !string.IsNullOrEmpty(post.Description))
//                {
//                    if(!showFull)
//                    {
//                        if (post.Description.Length > 220)
//                        {
//                            return post.Description.Substring(0, 220) + "... <a href='/post/getinfo/"+post.Id+"' class='post-viewmore' target='_blank'>"+ UserWebResource.LB_VIEWMORE +"</a>";
//                        }
//                    }

//                    return post.Description;
//                }

//                if (post == null)
//                {
//                    logger.Error(string.Format("Failed when ParseDescriptionFromPost. The Post data is null"));
//                    return string.Empty;
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Error(string.Format("Failed when ParseDescriptionFromPost [{0}] due to {1}", post.Id, ex.ToString()));
//            }

//            return string.Empty;
//        }

//        public static string ParsePostOwnerInfo(IdentityPost post, int length = 25)
//        {
//            try
//            {
//                var result = string.Empty;
//                if (post != null)
//                {
//                    if (!string.IsNullOrEmpty(post.DisplayName))
//                    {
//                        result = post.DisplayName;
//                        return AccountHelper.GetShortDisplayName(result, length);
//                    }

//                    if (!string.IsNullOrEmpty(post.FullName))
//                    {
//                        result = post.FullName;
//                        return AccountHelper.GetShortDisplayName(result, length);
//                    }

//                    if (!string.IsNullOrEmpty(post.UserName))
//                    {
//                        result = post.UserName;
//                        return AccountHelper.GetShortDisplayName(result, length);
//                    }  
//                }

//                if (post == null)
//                {
//                    logger.Error(string.Format("Failed when ParsePostOwnerInfo. The Post data is null"));
//                    return string.Empty;
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Error(string.Format("Failed when ParsePostOwnerInfo [{0}] due to {1}", post.Id, ex.ToString()));
//            }

//            return string.Empty;
//        }

//        public static int GetStarOfPost(IdentityPost post, ref decimal ratingScore, ref int ratingCount)
//        {
//            try
//            {
//                if (post != null)
//                {
//                    if(post.RatingScore > 0)
//                    {
//                        ratingScore = post.RatingScore;
//                        ratingCount = post.Rating_Count;
//                        return (int)post.RatingScore;
//                    }
//                }

//                if (post == null)
//                {
//                    logger.Error(string.Format("Failed when GetStarOfPost. The Post data is null"));
//                    return 0;
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.Error(string.Format("Failed when GetStarOfPost [{0}] due to {1}", post.Id, ex.ToString()));
//            }

//            return 0;
//        }

//        public static string GetPostDetailUrl(int postId)
//        {
//            if (postId != 0)
//                return string.Format("/post/index/{0}", postId);

//            return "javascript:;";
//        }

//        public static string GetFriendlyUrl(IdentityPost post)
//        {
//            var urlFormat = "{0}{1}.{2}";
//            var url = string.Empty;
//            if(post != null)
//            {
//                url = string.Format(urlFormat, UrlFriendly.getUrlByUserProfile(post.DisplayName, post.UserId) + "/post/", UrlFriendly.ConvertToUrlFriendly(post.Title), SecurityHelper.Encrypt(post.Id.ToString(), post.CreatedDate));
//            }

//            return url;
//        }

//        public static string GetFirstImage(IdentityPost post)
//        {
//            try
//            {
//                if (post != null)
//                {
//                    if (post.Images != null && post.Images.Count > 0)
//                    {
//                        return post.Images[0].Url;
//                    }
//                }
//            }
//            catch
//            {

//            }

//            return string.Empty;
//        }
//    }
//}