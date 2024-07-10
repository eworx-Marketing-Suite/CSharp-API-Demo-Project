/*
---------------------------------------------------------------------------------------------------------------------------------------------------
---  The following API methods get used in this example:                                                                                        ---
---     • SendTriggerMails                  https://www.eworx.at/doku/sendtriggermails                                                          ---
---     • GetCampaigns                      https://www.eworx.at/doku/getcampaigns/                                                             ---
---     • GetSectionDefinitions             https://www.eworx.at/doku/getsectiondefinitions/                                                    ---
---------------------------------------------------------------------------------------------------------------------------------------------------
 */
using System;
using System.IO;
using System.Linq;
using SampleImplementation.Common;
using SampleImplementation.mailworxAPI;

namespace SampleImplementation.Examples.SendTriggerMail {

    /// <summary>
    /// This class will show you how to send trigger mails.
    /// </summary>
    public class Example_TriggerMail : IExample {
        private readonly EmsServiceAgent _serviceAgent;
        private readonly string _assetsPath;


        /// <summary>
        /// Initializes a new instance of the <see cref="Example_TriggerMail"/> class.
        /// </summary>
        /// <param name="serviceAgent">Communicate with eMS</param>
        public Example_TriggerMail(EmsServiceAgent serviceAgent) {
            if (serviceAgent == null)
                throw new ArgumentNullException(nameof(serviceAgent), "serviceAgent must not be null!");

            _assetsPath = Path.Combine(Environment.CurrentDirectory, @"Assets\");
            _serviceAgent = serviceAgent;
        }

        /// <summary>
        /// Runs the send trigger mail example.
        /// </summary>
        public void RunExample() {
            Console.WriteLine();

            // Set the campaign id and the subsrciber id here.
            Guid campaignId = new Guid("[CAMPAIGN_ID]");
            Guid subscriberId = new Guid("[SUBSCRIBER_ID]");

            // Create a request to send trigger mails.

            SendTriggerMailsRequest sendTriggerMailsRequest = _serviceAgent.CreateRequest(new SendTriggerMailsRequest() {
                CampaignId = campaignId,
                TriggerMails = new TriggerMail[] { GetTriggerMail(campaignId, subscriberId) },
                // TriggerMailIds don't need to be set.
                TriggerMailIds = null,
                // Read the documentation for the following options here: https://www.eworx.at/doku/sendtriggermails/
                UseIRated = true,
                IgnoreCulture = true,
                UseRTR = false,
                ConsiderSubscriberExclusionCriterias = false,
                ConsiderDOIStatus = true
            });

            // Send out the trigger mail.

            SendTriggerMailsResponse triggerMailsResponse = _serviceAgent.SendTriggerMails(sendTriggerMailsRequest);
        }


        /// <summary>
        /// Creates a new trigger mail with the according subscriber.
        /// </summary>
        /// <param name="campaignId">The campaign id.</param>
        /// <param name="subscriberId">The subscriber id.</param>
        /// <returns></returns>
        private TriggerMail GetTriggerMail(Guid campaignId, Guid subscriberId) {
            // Create a new triggerMail.

            TriggerMail triggerMail = new TriggerMail() {
                SubscriberId = subscriberId,
                // Here the sections are set.
                Sections = GenerateSections(campaignId),
                // Here the custom text will be inserted.
                Data = new TriggerMailData[] {
                    new TriggerMailData() {
                        Key = "description",
                        Value = "This text will be inserted in the trigger mail"
                    },
                    new TriggerMailData() {
                        Key = "mailsubject",
                        Value = "Testbetreff"
                    }
                }
            };

            return triggerMail;
        }

        /// <summary>
        /// Generates sections according to the given template of the campaign.
        /// </summary>
        /// <param name="campaignId">The id of the campaign.</param>
        /// <returns></returns>
        private Section[] GenerateSections(Guid campaignId) {
            // Create a new campaign request.
            CampaignsRequest campaignsRequest = _serviceAgent.CreateRequest(new CampaignsRequest() {
                Id = campaignId,
                ResponseDetail = CampaignResponseDetailInfo.Sections
                // ResponseDetail = CampaignResponseDetailInfo.BasicInformation -> Get almost all details of the campaign type (without links and sections).
                // ResponseDetail = CampaignResponseDetailInfo.Sections -> Also gets the sections of the campaign.
                // ResponseDetail = CampaignResponseDetailInfo.Links -> Also gets the links of the campaign.
                // ResponseDetail = CampaignResponseDetailInfo.SectionProfiles -> Also gets info about which sections are restricted to which target groups.
            });

            // Gets the campaigns.
            CampaignsResponse campaignsResponse = _serviceAgent.GetCampaigns(campaignsRequest);

            Guid templateId = campaignsResponse.Campaigns.First().TemplateGuid;

            if (templateId != null) {
                SectionDefinitionRequest sectionDefinitionRequest = _serviceAgent.CreateRequest(new SectionDefinitionRequest() {
                    Template = new Template() { Guid = templateId }
                });

                SectionDefinitionResponse sectionDefinitionResponse = _serviceAgent.GetSectionDefinitions(sectionDefinitionRequest);

                SectionDefinition[] sectionDefinitions = sectionDefinitionResponse.SectionDefinitions;

                if (sectionDefinitions != null && sectionDefinitions.Length > 0) {
                    // Gets the section where the custom data will be set.
                    SectionDefinition definitionArticle = sectionDefinitions.FirstOrDefault(s => s.Name.Equals("Artikel", StringComparison.InvariantCultureIgnoreCase));

                    return new Section[] { CreateArticleSection(definitionArticle) };
                }
            }

            return null;
        }

        /// <summary>
        /// Creates an article section to the according section definition.
        /// </summary>
        /// <param name="sectionDefinition">The section definition.</param>
        /// <returns></returns>
        private Section CreateArticleSection(SectionDefinition sectionDefinition) {
            // Create a new section.
            Section sectionArticle = new Section() {
                Created = DateTime.Now,
                SectionDefinitionName = sectionDefinition.Name,
                StatisticName = "Trigger mail"
            };

            TextField textField = new TextField() {
                InternalName = "a_text",
                UntypedValue = "This is some text in the trigger mail: [%mwtm:description%]"
            };

            // Upload the file from the given path to the eMS media data base.
            Guid fileId = this.UploadFile(Path.Combine(_assetsPath, "email.png"), "email.png");

            MdbField imgField = new MdbField() {
                InternalName = "a_img",
                UntypedValue = fileId.ToString()
            };




            // Sets the fields of the section.
            sectionArticle.Fields = new Field[] {
                textField,
                imgField
            };

            return sectionArticle;
        }

        /// <summary>
        /// Uploads a file to the eMS media data base.
        /// </summary>
        /// <param name="path">The path where the file to upload is located.</param>
        /// <param name="fileName">Name of the file to upload.</param>
        /// <returns>Returns the id of the uploaded file.</returns>
        private Guid UploadFile(string path, string fileName) {
            // Get all files in the mdb for the directory mailworx.
            MediaDbRequest mediadbRequest = _serviceAgent.CreateRequest(new MediaDbRequest() {
                Path = "marketing-suite"
            });

            FileResponse fileResponse = _serviceAgent.GetMDBFiles(mediadbRequest);
            Guid fileId = Guid.Empty;

            // Check if there is already a file with the given filename.
            if (fileResponse == null || fileResponse.Files.FirstOrDefault(s => s.Name == fileName) == null) {
                // The file we want to upload.
                Byte[] picture = File.ReadAllBytes(path);

                // Send the data to eMS
                FileUploadRequest uploadRequest = _serviceAgent.CreateRequest(new FileUploadRequest() {
                    File = picture, // The picture as byte array.
                    Name = fileName, // The name of the file including the file extension.
                    Path = "marketing-suite" // The location within the eMS media database. If this path does not exist within the media data base, an exception will be thrown.
                });

                FileUploadResponse uploadResponse = _serviceAgent.UploadFileToMDB(uploadRequest);

                if (uploadResponse != null)
                    fileId = uploadResponse.FileId;
            }
            else {
                fileId = fileResponse.Files.FirstOrDefault(s => s.Name == fileName).Id;
            }

            return fileId;
        }
    }
}
