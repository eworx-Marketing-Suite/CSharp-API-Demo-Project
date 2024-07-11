/*
---------------------------------------------------------------------------------------------------------------------------------------------------
---  The following API methods get used in this example:                                                                                        ---
---     • GetCampaigns                  https://www.eworx.at/doku/getcampaigns/                                                                 ---
---     • CopyCampaign                  https://www.eworx.at/doku/copycampaign/                                                                 ---
---     • UpdateCampaign                https://www.eworx.at/doku/updatecampaign/                                                               ---
---------------------------------------------------------------------------------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SampleImplementation.Common;
using SampleImplementation.mailworxAPI;

namespace SampleImplementation.Examples.SendCampaign {

    /// <summary>
    /// This class will show you how a campaign can be created and updated in mailworx.
    /// </summary>
    public class CampaignCreator {

        private readonly EmsServiceAgent _serviceAgent;


        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignCreator"/> class.
        /// </summary>
        /// <param name="serviceAgent">Communicate with eMS</param>
        public CampaignCreator(EmsServiceAgent serviceAgent) {
            if (serviceAgent == null)
                throw new ArgumentNullException(nameof(serviceAgent), "serviceAgent must not be null!");

            _serviceAgent = serviceAgent;
        }


        /// <summary>
        /// Creates a campaign in mailworx.
        /// </summary>
        /// <param name="profileId">The profile id that should be used for the campaign.</param>
        /// <returns>Returns a KeyValuePair where the key is the template id and the value is the created campaign id.</returns>
        public async Task<KeyValuePair<Guid, Guid>> CreateCampaign(Guid profileId, string campaignName) {
            if (profileId == Guid.Empty)
                throw new ArgumentException("profileId", "profileId must not be an empty guid!");

            // Load the original campaign.
            Campaign originalCampaign = await this.LoadCampaign(campaignName);

            if (originalCampaign != null) {
                if (originalCampaign.Name.Equals(campaignName)) {

                    // Copy the original campaign
                    Campaign copyCampaign = await this.CopyCampaign(originalCampaign.Guid);

                    // Update the sender, profile, ....
                    if (await this.UpdateCampaign(copyCampaign, profileId, campaignName, "service@eworx.at", "eworx Service Crew", "My first Newsletter")) {
                        return new KeyValuePair<Guid, Guid>(copyCampaign.Guid, copyCampaign.TemplateGuid);
                    }
                }
                else {
                    // Return the already existing campaign.
                    return new KeyValuePair<Guid, Guid>(originalCampaign.Guid, originalCampaign.TemplateGuid);
                }
            }

            return new KeyValuePair<Guid, Guid>();
        }

        /// <summary>
        /// Loads the campaign with the specified id.
        /// </summary>
        /// <param name="campaignId">The campaign id.</param>
        /// <returns>Returns the campaign.</returns>
        private async Task<Campaign> LoadCampaign(string campaignName, Guid? campaignId = null) {
            // Build up the request.
            CampaignsRequest campaignRequest = _serviceAgent.CreateRequest(new CampaignsRequest() {
                Type = CampaignType.InWork
            });

            if (campaignId.HasValue) { // If there is a campaign id given, then load the campaign by its id.

                campaignRequest.Id = campaignId.Value;

                CampaignsResponse response = await _serviceAgent.GetCampaignsAsync(campaignRequest);

                if (response == null)
                    return null;
                else
                    return response.Campaigns.FirstOrDefault();
            }
            else { // If there is no campaign id given, then load the campaign by its name.

                CampaignsResponse response = await _serviceAgent.GetCampaignsAsync(campaignRequest);
                Campaign[] loadedCampaigns = response.Campaigns;
                Campaign existingCampaign = loadedCampaigns.FirstOrDefault(c => c.Name.Equals(campaignName, StringComparison.OrdinalIgnoreCase));

                if (existingCampaign == null) {
                    return loadedCampaigns.FirstOrDefault(c => c.Name.Equals(campaignName, StringComparison.OrdinalIgnoreCase));
                }
                else {
                    return existingCampaign;
                }
            }
        }

        /// <summary>
        /// Copies a Campaign.
        /// </summary>
        /// <param name="campaignId">The campaignId.</param>
        /// <returns>Returnes a copy of the given campaign.</returns>
        private async Task<Campaign> CopyCampaign(Guid campaignId) {
            CopyCampaignRequest copyCampaignRequest = _serviceAgent.CreateRequest(new CopyCampaignRequest() {
                CampaignToCopy = campaignId // The campaign which should be copied.
            });

            CopyCampaignResponse copyCampaignResponse = await _serviceAgent.CopyCampaignAsync(copyCampaignRequest);

            if (copyCampaignResponse != null) {
                return await this.LoadCampaign(String.Empty, copyCampaignResponse.NewCampaignGuid);
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// Updates the given campaign (name, senderAddress, senderName, subject...)
        /// </summary>
        /// <param name="campaignToUpdate">The campaign to update.</param>
        /// <param name="profileId">The profileId.</param>
        /// <param name="name">The name to update.</param>
        /// <param name="senderAddress">The senderAddress to update.</param>
        /// <param name="senderName">The senderName to update.</param>
        /// <param name="subject">The subject to update.</param>
        /// <returns>Returns true if the update is succesful</returns>
        private async Task<bool> UpdateCampaign(Campaign campaignToUpdate, Guid profileId, string name, string senderAddress, string senderName, string subject) {
            // Every value of type string in the UpdateCampaignRequest must be assigned, otherwise it will be updated to the default value (which is string.Empty).

            UpdateCampaignRequest updateRequest = _serviceAgent.CreateRequest(new UpdateCampaignRequest() {
                CampaignGuid = campaignToUpdate.Guid,
                Language = campaignToUpdate.Culture,
                ProfileGuid = profileId,
                Name = name,
                SenderAddress = senderAddress,
                SenderName = senderName,
                Subject = subject
            });

            return await _serviceAgent.UpdateCampaignAsync(updateRequest) != null;
        }
    }
}