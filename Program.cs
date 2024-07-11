/*
---------------------------------------------------------------------------------------------------------------------------------------------------
---  This is a sample implementation for using the eworx Marketing Suite API in order to create and send an email campaign over mailworx.       ---
---  Be aware of the fact that this example might not work in your mailworx account.                                                            ---
---------------------------------------------------------------------------------------------------------------------------------------------------
---  ENSURE YOU PROVIDE YOUR CORRECT LOGIN DATA AT THE SERVICE AGENNT.                                                                          ---
---------------------------------------------------------------------------------------------------------------------------------------------------
---																																				---
---  The following API methods get used in this example:                                                                                        ---
---     • GetProfiles                   https://www.eworx.at/doku/getprofiles                                                                   ---
---     • GetSubscriberFields           https://www.eworx.at/doku/getsubscriberfields/                                                          ---
---     • ImportSubscribers             https://www.eworx.at/doku/importsubscribers                                                             ---
---     • GetCampaigns                  https://www.eworx.at/doku/getcampaigns/                                                                 ---
---     • CopyCampaign                  https://www.eworx.at/doku/copycampaign                                                                  ---
---     • UpdateCampaign                https://www.eworx.at/doku/updatecampaign/                                                               ---
---     • GetSectionDefinitions         https://www.eworx.at/doku/getsectiondefinitions                                                         ---
---     • CreateSection                 https://www.eworx.at/doku/createsection                                                                 ---
---     • SendCampaign                  https://www.eworx.at/doku/sendcampaign/                                                                 ---
---     • SendTriggerMails              https://www.eworx.at/doku/sendtriggermails                                                              ---
---     • GetMDBFiles                   https://www.eworx.at/doku/getmdbfiles/                                                                  ---
---     • UploadFileToMDB               https://www.eworx.at/doku/uploadfiletomdb/                                                              ---
---------------------------------------------------------------------------------------------------------------------------------------------------
 */

using System;
using System.Threading.Tasks;
using SampleImplementation.Common;
using SampleImplementation.Examples.ReadCampaignStatistic;
using SampleImplementation.Examples.SendTriggerMail;

namespace SampleImplementation {
    public static class Program {

        static async Task Main(string[] args) {

            // This is the eMS service agent which will be used as connection to the mailworx webservice.
            // Sets up SecurityContext and Language, since both are always need to be sent in a request.
            EmsServiceAgent emsServiceAgent = new EmsServiceAgent()
                // Set your login data here.
                // You must register your application source at the following page, before you try to access the eMS webservice: https://www.eworx.at/doku/api-schnittstellenbeschreibung/#zugang-zur-api-anlegen.
                .UseCredentials(
                    "[ACCOUNT]",                // account name (Mandant) of the eMS to login
                    "[USERNAME]",               // user name to use to login
                    "[PASSWORD]",               // the user's password
                    "[APPLICATION]"             // the name of the registered application
                )
                .UseLanguage("EN")
                // The url to the webservice.
                .UseServiceUrl("https://mailworx.marketingsuite.info/services/serviceagent.asmx");

            Console.WriteLine("Csharp-API-Demo-Project");
            Console.WriteLine("=======================");

            bool running = true;
            IExample example = null;

            while (running) {
                example = null;

                Console.WriteLine("Run one of the following examples:");
                Console.WriteLine("a   - Send a campaign");
                Console.WriteLine("b   - Read campaign statistics");
                Console.WriteLine("c   - Send a trigger mail");
                Console.WriteLine("esc - End the program");

                Console.Write("Enter your choice: ");

                switch (Console.ReadKey().Key) {
                    case ConsoleKey.A:
                        example = new Example_SendCampaign(emsServiceAgent);
                        break;
                    case ConsoleKey.B:
                        example = new Example_ReadCampaignStatistic(emsServiceAgent);
                        break;
                    case ConsoleKey.C:

                        example = new Example_TriggerMail(emsServiceAgent);
                        break;
                    case ConsoleKey.Escape:
                        running = false;
                        break;
                    default:
                        break;
                }
                if (running && example != null) {
                    await example.RunExample();
                }
            }
        }
    }
}
