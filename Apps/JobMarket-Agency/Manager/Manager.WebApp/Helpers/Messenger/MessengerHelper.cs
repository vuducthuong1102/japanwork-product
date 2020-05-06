using Manager.WebApp.Models;
using MsSql.AspNet.Identity.Entities;
using System.Collections.Generic;

namespace Manager.WebApp.Helpers
{
    public class MessengerHelper
    {
        public static List<MessageItemModel> GroupListMessageModel(List<IdentityConversationReply> myList, int currentUserId)
        {
            var total = myList.Count;
            var listModel = new List<MessageItemModel>();
            if (total > 0)
            {
                var skipNum = 0;
                for (int i = 0; i < total; i++)
                {
                    //Same owner
                    if (skipNum != 0 && skipNum >= i)
                    {
                        continue;
                    }
                    else
                    {
                        var currentModel = new MessageItemModel();
                        currentModel.CurrentUserId = currentUserId;
                        myList[i].UserOneInfo = myList[0].UserOneInfo;
                        myList[i].UserTwoInfo = myList[0].UserTwoInfo;
                        currentModel.MessageItem = myList[i];

                        for (int j = skipNum + 1; j <= total - 1; j++)
                        {
                            if (myList[i].UserId == myList[j].UserId && myList[i].UserObjectType == myList[j].UserObjectType)
                            {
                                currentModel.NextMessages.Add(myList[j]);
                                skipNum = j;
                            }
                            else
                            {
                                break;
                            }
                        }

                        listModel.Add(currentModel);
                    }
                }
            }

            return listModel;
        }
    }
}