﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;

using log4net;
using log4net.Config;

using Newtonsoft.Json;

using StructureMap;
using StructureMap.Graph;

using EmailConsoler.Logging;
using EmailConsoler.DataStorage;
using EmailConsoler.Services;
using EmailConsoler.Tasking;
using EmailConsoler.Configuration;
using EmailConsoler.Models;

namespace EmailConsoler
{
    //class Program_Old
    //{
    //    private static readonly EmailConsoler.Logging.ILog logger = LogProvider.GetCurrentClassLogger();
    //    private const string APP_NAME = "Log4net Parser";
        

    //    private const int MF_BYCOMMAND = 0x00000000;
    //    public const int SC_CLOSE = 0xF060;

    //    [DllImport("user32.dll")]
    //    public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

    //    [DllImport("user32.dll")]
    //    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    //    [DllImport("kernel32.dll", ExactSpelling = true)]
    //    private static extern IntPtr GetConsoleWindow();

    //    static void Main(string[] args)
    //    {
    //        //Disable the Close button
    //        //http://stackoverflow.com/questions/6052992/how-can-i-disable-close-button-of-console-window-in-a-visual-studio-console-appl
    //        DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);           

    //        Console.Title = APP_NAME;

    //        LoadLog4NetConfig();

    //        RegisterStructureMap();

    //        ConsoleKeyInfo cki;
    //        bool isHostRunning = false;
    //        do
    //        {
    //            DisplayMenuList();

    //            Console.WriteLine(">Your choice: ");
    //            cki = Console.ReadKey();
    //            Console.WriteLine();
    //            switch (cki.KeyChar.ToString().ToUpper())
    //            {
    //                case "S":
    //                    if (!isHostRunning)
    //                    {
    //                        Consoler.WriteAppLine("Starting Tracer");
    //                        isHostRunning = StartPCHost();
    //                        if (isHostRunning)                            
    //                            Consoler.WriteAppLine("Tracer Started");
    //                    }
    //                    else
    //                    {
    //                        Consoler.WriteErrorLine("Tracer already started");
    //                    }
    //                    break;
    //                case "P":
                                                
    //                    if (isHostRunning)
    //                    {
    //                        Consoler.WriteAppLine("Stoping Tracer");
    //                        isHostRunning = !StopPCHost();
    //                        if (!isHostRunning)
    //                        Consoler.WriteAppLine("Tracer Stoped");
    //                    }
    //                    else
    //                    {
    //                        Consoler.WriteErrorLine("Tracer already stoped");
    //                    }
                        
    //                    break;
    //                case "C":
    //                    Console.Clear();
    //                    break;
    //                case "X":
    //                    if (isHostRunning)
    //                    {
    //                        Consoler.WriteErrorLine("Tracer still running. Stop it first!");
    //                        //reset key
    //                        cki = new ConsoleKeyInfo();
    //                    }
    //                    else
    //                    {
    //                        Consoler.WriteAppLine("GOOD BYE");
    //                        System.Threading.Thread.Sleep(3000);
    //                    }
    //                    break;
    //                default:
    //                    Consoler.WriteErrorLine("Invalid command");
    //                    break;
    //            }
    //            Console.WriteLine();
    //        }
    //        while (! (cki.KeyChar.ToString().ToUpper() == "X"));
    //    }

    //    static void LoadLog4NetConfig()
    //    {
    //        //for Log4net configuration            
    //        var configFile =  AppDomain.CurrentDomain.BaseDirectory + "Log4net.config";
    //        if (!string.IsNullOrEmpty(configFile))
    //        {
    //            configFile = Environment.ExpandEnvironmentVariables(configFile);
    //            //No need: [assembly: log4net.Config.XmlConfigurator(Watch = true)]
    //            XmlConfigurator.ConfigureAndWatch(new FileInfo(configFile));
    //        }
    //    }

    //    static void DisplayMenuList()
    //    {          
    //        Console.WriteLine(APP_NAME);
    //        Console.WriteLine("===============");
    //        Console.WriteLine("[S] Start");
    //        Console.WriteLine("[P] Stop");
    //        Console.WriteLine("[C] Clear");
    //        Console.WriteLine("[X] Exit");
    //    }



    //    private static List<WorkerThreadEx> WorkerList = new List<WorkerThreadEx>();
    //    static bool StartPCHost()
    //    {          
    //        try
    //        {
    //            var logFolderSection = LogFolderConfigSection.GetConfigSection();
    //            var logFolderCollection = logFolderSection.LogFolders;

    //            Consoler.WriteInfoLine(string.Format("Found {0} log folders need to process", logFolderCollection.Count));                
    //            foreach(LogFolderConfigElement folderElement in logFolderCollection)
    //            {
    //                if (!Directory.Exists(folderElement.folder))
    //                {
    //                    Consoler.WriteErrorLine("The folder not found: " + folderElement.folder);
    //                    continue;
    //                }

    //                //Create worker
    //                var wkname = string.Format("LogFolder_Worker_Item_{0}", folderElement.item);
    //                var worker = new LogFolderWorker(wkname, folderElement);
    //                WorkerList.Add(worker);

    //                //Start worker
    //                worker.Start();
    //                worker.Signal();

    //                Consoler.WriteInfoLine(string.Format("The {0} has beend started on [{1}-{2}]: ", wkname, folderElement.machine, folderElement.serverip));                    
    //            }

    //            return true;
    //        }  
    //        catch(Exception ex)
    //        {
    //            Consoler.WriteErrorLine("Failed to start Tracer: "+ ex.Message);
    //        }
    //        return false;            
    //    }        

    //    static bool StopPCHost()
    //    {
    //        try
    //        {
    //            if (WorkerList!= null && WorkerList.Count>0)
    //            {
    //                for (var idx = 0; idx< WorkerList.Count; idx++)
    //                {
    //                    var worker = WorkerList[idx];
    //                    var wkName = worker.Name;                        
    //                    worker.Pause();
    //                    worker.Abort();
    //                    worker = null;
    //                    Consoler.WriteInfoLine(string.Format("The {0} has beend stopped", wkName));
    //                }
    //                WorkerList.Clear();
    //            }                

    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            Consoler.WriteErrorLine("Failed to stop Tracer: " + ex.Message);
    //        }
    //        return false;
    //    }


    //    public static Container DependencyContainer = null;

    //    static void RegisterStructureMap()
    //    {
    //        DependencyContainer = new Container(_ =>
    //        {                                
    //            /*
    //            _.For<ISerializer>().Singleton().Use(c => new NewtonsoftSerializer(null));
    //            _.For<ICacheClient>().Singleton().Use(c => new StackExchangeRedisCacheClient(c.GetInstance<ISerializer>(), null));
    //            _.For<IQueueNameResolver>().Singleton().Use(c => new QueueNameResolver());
    //            _.For<IExchangeNameResolver>().Singleton().Use(c => new ExchangeNameResolver());
    //            _.For<IExchangeSubscribersResolver>().Singleton().Use(c => new ExchangeSubscribersResolver(c.GetInstance<IExchangeNameResolver>()));
    //            _.For<IExchangeSubscriberService>().Singleton().Use(c => new ExchangeSubscriberService(c.GetInstance<ICacheClient>(), c.GetInstance<IExchangeSubscribersResolver>()));
    //            _.For<INotificationService>().Singleton().Use(c => new NotificationService(c.GetInstance<ICacheClient>(), c.GetInstance<IExchangeNameResolver>()));
    //            _.For<ISubscriberFactory>().Singleton().Use(c => new SubscriberFactory(c.GetInstance<IQueueNameResolver>(), c.GetInstance<IExchangeNameResolver>(), c.GetInstance<ICacheClient>(), c.GetInstance<IExchangeSubscriberService>()));
    //            _.For<IMessageBroker>().Singleton().Use(c => new MessageBroker(c.GetInstance<IExchangeSubscriberService>(), c.GetInstance<INotificationService>(), c.GetInstance<ISubscriberFactory>()));
    //            */

    //            _.For<IDataStorage>().Singleton().Use(c => new MySqlDataStorage());
    //            _.For<ILogParserService>().Singleton().Use(c => new LogParserService(c.GetInstance<IDataStorage>()));

    //            //Register singleton for VodStore, VodService
    //            _.For<IMySQLVodStore>().Singleton().Use(c => new MySQLVodStore());
    //            _.For<IVodService>().Singleton().Use(c => new VodService(c.GetInstance<IMySQLVodStore>()));
    //        });
    //    } 

    //}
}
