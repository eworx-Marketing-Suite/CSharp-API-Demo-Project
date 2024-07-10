# Send a campaign

## Preparation

Set the campaign name and the profile name in the [Example_SendCampaign](./Example_SendCampaign.cs) class.

## Steps
1. Import the subscribers
2. Create a campaign
3. Add content to the campaign
4. Send the campaign to the imported subscribers

## 1. Import the subscribers

Use the [SubscriberImport](./SubscriberImport.cs) class to do all the necessary import steps and get a list of the imported Data.

## 2. Create a campaign

Use the class [CampaignCreator](./CampaignCreator.cs) to create a campaign and get the ids for the campaign and the used template.

## 3. Add content to the campaign

In the [SectionCreator](./SectionCreator.cs) class you generate the content sections.

## 4. Send the campaign to the imported subscribers

Create a request to send the created campaign.