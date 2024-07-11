/*
---------------------------------------------------------------------------------------------------------------------------------------------------
---  This is a sample implementation for using the eworx Marketing Suite API in order to create and send an email campaign over mailworx.       ---
---  Be aware of the fact that this example might not work in your mailworx account.                                                            ---
---------------------------------------------------------------------------------------------------------------------------------------------------
---  ENSURE YOU PROVIDE YOUR CORRECT LOGIN DATA AT THE GetSecurityContext-Method                                                                ---
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
---------------------------------------------------------------------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SampleImplementation.Common;
using SampleImplementation.Examples.SendCampaign;
using SampleImplementation.mailworxAPI;

namespace SampleImplementation {
    public class Example_SendCampaign : IExample {

        private readonly EmsServiceAgent _serviceAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Example_SendCampaign"/> class.
        /// </summary>
        /// <param name="serviceAgent">Communicate with eMS</param>
        public Example_SendCampaign(EmsServiceAgent serviceAgent) {
            if (serviceAgent == null)
                throw new ArgumentNullException(nameof(serviceAgent), "serviceAgent must not be null!");

            _serviceAgent = serviceAgent;
        }

        /// <summary>
        /// Runs the send campaign example.
        /// </summary>
        public async Task RunExample() {
            Console.WriteLine();

            // ### STEP 1 : Preparations ###

            // Set the campaign name here.
            string campaignName = "[CAMPAIGN_NAME]";
            // Set the profile name here.
            string profileName = "[PROFILE_NAME]";


            // ### STEP 1 : Preparations ###

            // ### STEP 2 : IMPORT ###
            // Here we use the SubscriberImport class in order to do all the necessary import steps.
            SubscriberImport import = new SubscriberImport(_serviceAgent);

            // The key is the id of the profile where the subscribers have been imported to.
            // The value is a list of ids of the imported subscribers.
            KeyValuePair<Guid, List<Guid>> importedData = await import.ImportSubscribers(profileName);

            // ### STEP 2 : IMPORT ###


            // If some data where imported....
            if (importedData.Value != null && importedData.Value.Count > 0) {

                // ### STEP 3 : CREATE CAMPAIGN ###

                // Here we use the CampaignCreator in order to do all the necessary steps for creating a campaign.
                CampaignCreator campaignCreator = new CampaignCreator(_serviceAgent);

                // The key is the id of the template.
                // The value is the id of the campaign.
                KeyValuePair<Guid, Guid> data = await campaignCreator.CreateCampaign(importedData.Key, campaignName);

                // ### STEP 3 : CREATE CAMPAIGN ###



                // If a campaign was returned we can add the sections (= content of campaign).
                if (data.Key != Guid.Empty && data.Value != Guid.Empty) {

                    // ### STEP 4 : ADD SECTIONS TO CAMPAIGN ###

                    // Here we use the SectionCreator in order to do all the necessary steps for adding sections to the campaign.
                    SectionCreator sectionCreator = new SectionCreator(_serviceAgent);

                    // Send the campaign, if all sections have been created.
                    if (await sectionCreator.GenerateSection(data.Value, data.Key)) {

                        // ### STEP 4 : ADD SECTIONS TO CAMPAIGN ###


                        // ### STEP 5 : SEND CAMPAIGN ###

                        SendCampaignRequest sendCampaignRequest = _serviceAgent.CreateRequest(new SendCampaignRequest() {
                            CampaignId = data.Key,
                            IgnoreCulture = false, // Send the campaign only to subscribers with the same language as the campaign
                            SendType = CampaignSendType.Manual,
                            // If the SendType is set to Manual, ManualSendSettings are needed
                            // If the SendType is set to ABSplit, ABSplitTestSendSettings are needed
                            Settings = new ManualSendSettings() { SendTime = DateTime.Now },
                            UseIRated = false, // Here is some more info about iRated http://www.mailworx.info/en/irated-technology
                            UseRTR = true
                        });

                        // Send the campaign
                        SendCampaignResponse sendCampaignResponse = await _serviceAgent.SendCampaignAsync(sendCampaignRequest);

                        // ### STEP 5 : SEND CAMPAIGN ###

                        if (sendCampaignResponse != null) {
                            Console.WriteLine(string.Format("Effective subscribers: {0}", sendCampaignResponse.RecipientsEffective));

                            // wait until the data is sent!
                            Console.WriteLine("Wait until data is sent, then hit enter");
                            Console.ReadLine();
                        }
                        else {
                            Console.WriteLine("Something went wrong.");
                        }
                    }
                }
            }
        }
    }
}
