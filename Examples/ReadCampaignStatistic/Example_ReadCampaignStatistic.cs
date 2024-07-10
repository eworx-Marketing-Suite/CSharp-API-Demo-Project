/*
---------------------------------------------------------------------------------------------------------------------------------------------------
---  The following API methods get used in this example:                                                                                        ---
---     • GetCampaigns                  https://www.eworx.at/doku/getcampaigns/                                                                 ---
---     • GetCampaignStatistics         https://www.eworx.at/doku/campaignstatistics/                                                           ---
---     • GetBouncesOfCampaign          https://www.eworx.at/doku/campaignstatistics/                                                           ---
---     • GetClickRatesOfCampaign       https://www.eworx.at/doku/campaignstatistics/                                                           ---
---     • GetOpeningRatesOfCampaign     https://www.eworx.at/doku/campaignstatistics/                                                           ---
---     • GetMailClientTypesOfCampaign  https://www.eworx.at/doku/getmailclienttypesofcampaign/                                                 ---
---------------------------------------------------------------------------------------------------------------------------------------------------
 */
using System;
using System.Linq;
using SampleImplementation.Common;
using SampleImplementation.mailworxAPI;

namespace SampleImplementation.Examples.ReadCampaignStatistic {

    /// <summary>
    /// This class will show you how campaign statistics can be read.
    /// </summary>
    public class Example_ReadCampaignStatistic : IExample {

        private readonly EmsServiceAgent _serviceAgent;


        /// <summary>
        /// Initializes a new instance of the <see cref="Example_ReadCampaignStatistic"/> class.
        /// </summary>
        /// <param name="serviceAgent">Communicate with eMS</param>
        public Example_ReadCampaignStatistic(EmsServiceAgent serviceAgent) {
            if (serviceAgent == null)
                throw new ArgumentNullException(nameof(serviceAgent), "serviceAgent must not be null!");

            _serviceAgent = serviceAgent;
        }


        /// <summary>
        /// Runs the campaign statistic example.
        /// </summary>
        public void RunExample() {
            Console.WriteLine();

            // Set the campaign id of the campaign you want to read statistic data of.
            Guid campaignId = new Guid("[CAMPAIGN_ID]");

            // Create a request with the campaign Id and set the ResponseDetail to BasicInformation.

            CampaignsRequest campaignsRequest = _serviceAgent.CreateRequest(new CampaignsRequest() {
                Id = campaignId,
                ResponseDetail = CampaignResponseDetailInfo.BasicInformation
                // ResponseDetail = CampaignResponseDetailInfo.BasicInformation -> Get almost all details of the campaign type (without links and sections).
                // ResponseDetail = CampaignResponseDetailInfo.Sections -> Also gets the sections of the campaign.
                // ResponseDetail = CampaignResponseDetailInfo.Links -> Also gets the links of the campaign.
                // ResponseDetail = CampaignResponseDetailInfo.SectionProfiles -> Also gets info about which sections are restricted to which target groups.
            });


            // Read the common campaign information.
            CampaignsResponse campaignInfo = _serviceAgent.GetCampaigns(campaignsRequest);


            // Get the first campaign
            Campaign campaign = campaignInfo.Campaigns.First();

            if (campaign != null) {
                // Print the campaign Info to the Console
                this.PrintCampaignStatistics(campaign);
            };
        }



        /// <summary>
        /// Prints campaign statistics to the console.
        /// </summary>
        /// <param name="campaign">The campaign of which the statistics are printet to the console.</param>
        private void PrintCampaignStatistics(Campaign campaign) {
            this.PrintGeneralCampaignInfo(campaign);
            this.PrintStatistics(campaign);
            this.PrintBouncesInfo(campaign);
            this.PrintClickratesInfo(campaign);
            this.PrintOpeningRatesInfo(campaign);
            this.PrintMailClientTypesInfo(campaign);
        }

        /// <summary>
        /// Prints the campaign info to the console.
        /// </summary>
        /// <param name="campaign">The campaign to print to the console</param>
        private void PrintGeneralCampaignInfo(Campaign campaign) {
            Console.WriteLine($"General info of {campaign.Name}, created {campaign.Created}");
            Console.WriteLine("************************************************************");
            Console.WriteLine($"Subject: {campaign.Subject}");
            Console.WriteLine($"Template name: {campaign.TemplateName}");
            Console.WriteLine($"Sender: {campaign.SenderAddress}");
            Console.WriteLine($"Profile name: {campaign.ProfileName}");
            Console.WriteLine($"Culture: {campaign.Culture}");
            Console.WriteLine($"Notify address: {campaign.NotifyAddress}");
            Console.WriteLine("************************************************************");
            Console.WriteLine();
        }

        /// <summary>
        /// Prints the campaign statistics to the console.
        /// </summary>
        /// <param name="campaign">The campaign to print to the console.</param>
        private void PrintStatistics(Campaign campaign) {
            // Create a statistics request with the campaignId.
            CampaignStatisticsRequest statisticsRequest = _serviceAgent.CreateRequest(new CampaignStatisticsRequest() {
                CampaignGuid = campaign.Guid
            });

            CampaignStatisticsResponse campaignStatisticsInfo = _serviceAgent.GetCampaignStatistics(statisticsRequest);

            Console.WriteLine($"Campaign statistics of {campaign.Name}");
            Console.WriteLine("************************************************************");
            Console.WriteLine($"Sent mails: {campaignStatisticsInfo.TotalMails}");
            Console.WriteLine($"Opened mails: {campaignStatisticsInfo.OpenedMails}");
            Console.WriteLine($"Bounce mails: {campaignStatisticsInfo.BounceMails}");
            Console.WriteLine($"Amount of clicks: {campaignStatisticsInfo.Clicks}");
            Console.WriteLine("************************************************************");
            Console.WriteLine();
        }

        /// <summary>
        /// Prints the bounces info to the console.
        /// </summary>
        /// <param name="campaign"></param>
        /// <exception cref="NotImplementedException">The campaign to print to the console.</exception>
        private void PrintBouncesInfo(Campaign campaign) {
            CampaignBounceRequest bounceRequest = _serviceAgent.CreateRequest(new CampaignBounceRequest() {
                CampaignGuid = campaign.Guid
            });

            CampaignBounceResponse bounceInfo = _serviceAgent.GetBouncesOfCampaign(bounceRequest);

            Console.WriteLine($"Bounces statistics of {campaign.Name}");
            Console.WriteLine("************************************************************");
            Console.WriteLine($"Subscribers:");
            foreach (Guid subscriberId in bounceInfo.Subscribers) {
                Console.WriteLine($"Guid of the subscriber: {subscriberId}");
            }
            Console.WriteLine("************************************************************");
            Console.WriteLine();
        }

        /// <summary>
        /// Prints the click rates info to the console.
        /// </summary>
        /// <param name="campaign">The campaign to print to the console.</param>
        private void PrintClickratesInfo(Campaign campaign) {
            CampaignClickRatesRequest clickRatesRequest = _serviceAgent.CreateRequest(new CampaignClickRatesRequest() {
                CampaignGuid = campaign.Guid
            });

            CampaignClickRatesResponse clickRatesInfo = _serviceAgent.GetClickRatesOfCampaign(clickRatesRequest);

            Console.WriteLine($"Click rates statistics of {campaign.Name}");
            Console.WriteLine("************************************************************");
            Console.WriteLine($"Clicked links:");
            foreach (StatisticLink statisticLink in clickRatesInfo.ClickedLinks) {
                Console.WriteLine($"Linkname: {statisticLink.LinkName}");
                Console.WriteLine($"Clicks: {statisticLink.Clicks}");
                Console.WriteLine($"Url: {statisticLink.Url}");
                Console.WriteLine("------------------------------------------------------------");
            }
            Console.WriteLine("************************************************************");
            Console.WriteLine();
        }

        /// <summary>
        /// Prints the opening rates info to the console.
        /// </summary>
        /// <param name="campaign">The campaign to print to the console.</param>
        private void PrintOpeningRatesInfo(Campaign campaign) {
            CampaignOpeningRatesRequest openingRatesRequest = _serviceAgent.CreateRequest(new CampaignOpeningRatesRequest() {
                CampaignGuid = campaign.Guid
            });

            CampaignOpeningRatesResponse openingRatesInfo = _serviceAgent.GetOpeningRatesOfCampaign(openingRatesRequest);

            Console.WriteLine($"Opening rates statistics of {campaign.Name}");
            Console.WriteLine("************************************************************");
            Console.WriteLine($"Opened mails:");
            foreach (StatisticOpenedMail openedMail in openingRatesInfo.Openings) {
                Console.WriteLine($"State: {openedMail.ReadingState}");
                Console.WriteLine($"Opened at: {openedMail.OpenedAt}");
                Console.WriteLine($"Reading state specified: {openedMail.ReadingStateSpecified}");
                Console.WriteLine("------------------------------------------------------------");
            }
            Console.WriteLine("************************************************************");
            Console.WriteLine();
        }

        /// <summary>
        /// Prints the mail client types to the console.
        /// </summary>
        /// <param name="campaign">The campaign to print to the console.</param>
        private void PrintMailClientTypesInfo(Campaign campaign) {
            MailClientTypesRequest mailClientRequest = _serviceAgent.CreateRequest(new MailClientTypesRequest() {
                CampaignId = campaign.Guid
            });

            MailClientTypesResponse mailClientTypeInfo = _serviceAgent.GetMailClientTypesOfCampaign(mailClientRequest);

            Console.WriteLine($"Mail client types info of {campaign.Name}");
            Console.WriteLine("************************************************************");

            foreach (MailClientTypeStatisticData statisticData in mailClientTypeInfo.MailClientTypes) {
                Console.WriteLine($"Type: {statisticData.Type}");
                Console.WriteLine($"Amount of openings: {statisticData.Amount}");
                Console.WriteLine("------------------------------------------------------------");
            }
            Console.WriteLine("************************************************************");
            Console.WriteLine();
        }
    }
}
