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
using System.Threading.Tasks;
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
        public async Task RunExample() {
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
            CampaignsResponse campaignInfo = await _serviceAgent.GetCampaignsAsync(campaignsRequest);


            // Get the first campaign
            Campaign campaign = campaignInfo.Campaigns.First();

            if (campaign != null) {
                // Print the campaign Info to the Console
                await this.PrintCampaignStatistics(campaign);
            };
        }



        /// <summary>
        /// Prints campaign statistics to the console.
        /// </summary>
        /// <param name="campaign">The campaign of which the statistics are printet to the console.</param>
        private async Task PrintCampaignStatistics(Campaign campaign) {
            await this.PrintGeneralCampaignInfo(campaign);
            await this.PrintStatistics(campaign);
            await this.PrintBouncesInfo(campaign);
            await this.PrintClickratesInfo(campaign);
            await this.PrintOpeningRatesInfo(campaign);
            await this.PrintMailClientTypesInfo(campaign);
        }

        /// <summary>
        /// Prints the campaign info to the console.
        /// </summary>
        /// <param name="campaign">The campaign to print to the console</param>
        private Task PrintGeneralCampaignInfo(Campaign campaign) {
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

            return Task.CompletedTask;
        }

        /// <summary>
        /// Prints the campaign statistics to the console.
        /// </summary>
        /// <param name="campaign">The campaign to print to the console.</param>
        private async Task PrintStatistics(Campaign campaign) {
            // Create a statistics request with the campaignId.
            CampaignStatisticsRequest statisticsRequest = _serviceAgent.CreateRequest(new CampaignStatisticsRequest() {
                CampaignGuid = campaign.Guid
            });

            CampaignStatisticsResponse campaignStatisticsInfo = await _serviceAgent.GetCampaignStatisticsAsync(statisticsRequest);

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
        private async Task PrintBouncesInfo(Campaign campaign) {
            CampaignBounceRequest bounceRequest = _serviceAgent.CreateRequest(new CampaignBounceRequest() {
                CampaignGuid = campaign.Guid
            });

            CampaignBounceResponse bounceInfo = await _serviceAgent.GetBouncesOfCampaignAsync(bounceRequest);

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
        private async Task PrintClickratesInfo(Campaign campaign) {
            CampaignClickRatesRequest clickRatesRequest = _serviceAgent.CreateRequest(new CampaignClickRatesRequest() {
                CampaignGuid = campaign.Guid
            });

            CampaignClickRatesResponse clickRatesInfo = await _serviceAgent.GetClickRatesOfCampaignAsync(clickRatesRequest);

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
        private async Task PrintOpeningRatesInfo(Campaign campaign) {
            CampaignOpeningRatesRequest openingRatesRequest = _serviceAgent.CreateRequest(new CampaignOpeningRatesRequest() {
                CampaignGuid = campaign.Guid
            });

            CampaignOpeningRatesResponse openingRatesInfo = await _serviceAgent.GetOpeningRatesOfCampaignAsync(openingRatesRequest);

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
        private async Task PrintMailClientTypesInfo(Campaign campaign) {
            MailClientTypesRequest mailClientRequest = _serviceAgent.CreateRequest(new MailClientTypesRequest() {
                CampaignId = campaign.Guid
            });

            MailClientTypesResponse mailClientTypeInfo = await _serviceAgent.GetMailClientTypesOfCampaignAsync(mailClientRequest);

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
