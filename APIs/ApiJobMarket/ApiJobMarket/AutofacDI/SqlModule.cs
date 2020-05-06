using Autofac;
using ApiJobMarket.DB.Sql.Stores;

namespace ApiJobMarket.AutofacDI
{
    public class SqlModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StoreCountry>().As<IStoreCountry>();
            builder.RegisterType<StoreRegion>().As<IStoreRegion>();
            builder.RegisterType<StoreSearchPlace>().As<IStoreSearchPlace>();
            builder.RegisterType<StorePrefecture>().As<IStorePrefecture>();
            builder.RegisterType<StoreCity>().As<IStoreCity>();
            builder.RegisterType<StoreStation>().As<IStoreStation>();
            builder.RegisterType<StoreTrainLine>().As<IStoreTrainLine>();
            builder.RegisterType<StoreEmploymentType>().As<IStoreEmploymentType>();
            builder.RegisterType<StoreSalaryType>().As<IStoreSalaryType>();
            builder.RegisterType<StoreQualification>().As<IStoreQualification>();
            builder.RegisterType<StoreSuggest>().As<IStoreSuggest>();
            builder.RegisterType<StoreJapaneseLevel>().As<IStoreJapaneseLevel>();
            builder.RegisterType<StoreCompanySize>().As<IStoreCompanySize>();
            builder.RegisterType<StoreField>().As<IStoreField>();
            builder.RegisterType<StoreIndustry>().As<IStoreIndustry>();
            builder.RegisterType<StoreMajor>().As<IStoreMajor>();
            builder.RegisterType<StoreVisa>().As<IStoreVisa>();
            builder.RegisterType<StoreTag>().As<IStoreTag>();
            builder.RegisterType<StoreSalaryFilter>().As<IStoreSalaryFilter>();
            builder.RegisterType<StoreTypeSuggest>().As<IStoreTypeSuggest>();
            builder.RegisterType<StoreSubField>().As<IStoreSubField>();
            builder.RegisterType<StoreJobSeekerNote>().As<IStoreJobSeekerNote>();
            builder.RegisterType<StoreCompanyNote>().As<IStoreCompanyNote>();
            builder.RegisterType<StoreJobSeekerWish>().As<IStoreJobSeekerWish>();

            builder.RegisterType<StoreProcessStatus>().As<IStoreProcessStatus>();
            builder.RegisterType<StoreNavigation>().As<IStoreNavigation>();
            builder.RegisterType<StoreInterviewProcess>().As<IStoreInterviewProcess>();
            builder.RegisterType<StoreAgencyCompany>().As<IStoreAgencyCompany>();
            builder.RegisterType<StoreAgency>().As<IStoreAgency>();
            builder.RegisterType<StoreCompany>().As<IStoreCompany>();
            builder.RegisterType<StoreJob>().As<IStoreJob>();
            builder.RegisterType<StoreJobSeeker>().As<IStoreJobSeeker>();
            builder.RegisterType<StoreDevice>().As<IStoreDevice>();
            builder.RegisterType<StoreEduHistory>().As<IStoreEduHistory>();
            builder.RegisterType<StoreWorkHistory>().As<IStoreWorkHistory>();
            builder.RegisterType<StoreCertificate>().As<IStoreCertificate>();
            builder.RegisterType<StoreCv>().As<IStoreCv>();
            builder.RegisterType<StoreCs>().As<IStoreCs>();
            builder.RegisterType<StoreApplication>().As<IStoreApplication>();
            builder.RegisterType<StoreCandidate>().As<IStoreCandidate>();
            builder.RegisterType<StoreInvitation>().As<IStoreInvitation>();
            builder.RegisterType<StoreFriendInvitation>().As<IStoreFriendInvitation>();
            builder.RegisterType<StoreNotification>().As<IStoreNotification>();

            builder.RegisterType<StoreAgencyNotification>().As<IStoreAgencyNotification>();
            builder.RegisterType<StoreSchedule>().As<IStoreSchedule>();
            builder.RegisterType<StoreReport>().As<IStoreReport>();

            //builder.RegisterType<StoreConversation>().As<IStoreConversation>();
            //builder.RegisterType<StoreConversationReply>().As<IStoreConversationReply>();

            builder.RegisterType<StoreDocumentApi>().As<IStoreDocumentApi>();

            builder.RegisterType<StoreFooter>().As<IStoreFooter>();
        }
    }
}