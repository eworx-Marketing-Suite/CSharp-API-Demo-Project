/*
---------------------------------------------------------------------------------------------------------------------------------------------------
---  The following API methods get used in this example:                                                                                        ---
---     • CreateSection                 https://www.eworx.at/doku/createsection/                                                                ---
---     • GetSectionDefinitions         https://www.eworx.at/doku/getsectiondefinitions/                                                        ---
---     • GetMDBFiles                   https://www.eworx.at/doku/getmdbfiles/                                                                  ---
---     • UploadFileToMDB               https://www.eworx.at/doku/uploadfiletomdb/                                                              ---
---------------------------------------------------------------------------------------------------------------------------------------------------
 */

namespace SampleImplementation.Examples.SendCampaign {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using mailworxAPI;
    using SampleImplementation.Common;

    /// <summary>
    /// This class will show you how sections can be added to a campaign in eMS.
    /// </summary>
    public class SectionCreator {

        private readonly EmsServiceAgent _serviceAgent;
        private readonly string _assetsPath;


        /// <summary>
        /// Initializes a new instance of the <see cref="SectionCreator"/> class.
        /// </summary>
        /// <param name="serviceAgent">Communicate with eMS</param>
        public SectionCreator(EmsServiceAgent serviceAgent) {
            if (serviceAgent == null)
                throw new ArgumentNullException(nameof(serviceAgent), "serviceAgent must not be null!");

            _assetsPath = Path.Combine(Environment.CurrentDirectory, @"Assets\");
            _serviceAgent = serviceAgent;
        }


        /// <summary>
        /// Generates the section for the given template into the given campaign.
        /// </summary>
        /// <param name="templateId">The template Id.</param>
        /// <param name="campaignId">The campaign Id.</param>
        /// templateId;templateId must not be an empty guid!
        /// or
        /// campaignId;campaignId must not be an empty guid!
        /// </exception>
        public bool GenerateSection(Guid templateId, Guid campaignId) {
            if (templateId == Guid.Empty)
                throw new ArgumentException("templateId", "templateId must not be an empty guid!");
            if (campaignId == Guid.Empty)
                throw new ArgumentException("campaignId", "campaignId must not be an empty guid!");

            // Load all available section definitions for the given template
            SectionDefinition[] sectionDefinitions = this.LoadSectionDefinitions(templateId);
            bool sectionsCreated = sectionDefinitions != null && sectionDefinitions.Length > 0;

            // If there are no section definitions we can't setup the campaign.
            if (sectionDefinitions != null && sectionDefinitions.Length > 0) {
                // Right here we create three different sample sections for our sample campaign.

                // Load the section definition that defines an article.
                SectionDefinition definitionArticle = sectionDefinitions.FirstOrDefault(s => s.Name.Equals("Artikel", StringComparison.InvariantCultureIgnoreCase));

                if (definitionArticle != null) {
                    CreateSectionRequest createSectionRequest = _serviceAgent.CreateRequest(new CreateSectionRequest() {
                        Campaign = new Campaign { Guid = campaignId },
                        Section = this.CreateArticleSection(definitionArticle)
                    });

                    // ### CREATE THE SECTION ###

                    CreateSectionResponse response = _serviceAgent.CreateSection(createSectionRequest);

                    // ### CREATE THE SECTION ###

                    sectionsCreated = sectionsCreated && response != null && response.Guid != Guid.Empty;
                }

                // Load the section definition that defines a banner.
                SectionDefinition definitionBanner = sectionDefinitions.FirstOrDefault(s => s.Name.Equals("banner", StringComparison.InvariantCultureIgnoreCase));
                if (definitionBanner != null) {
                    CreateSectionRequest createBanner = _serviceAgent.CreateRequest(new CreateSectionRequest() {
                        Campaign = new Campaign { Guid = campaignId },
                        Section = this.CreateBannerSection(definitionBanner)
                    });

                    CreateSectionResponse response = _serviceAgent.CreateSection(createBanner);

                    sectionsCreated = sectionsCreated && response != null && response.Guid != Guid.Empty;
                }

                // Load the section definition that defines a two column.
                SectionDefinition definitionTwoColumn = sectionDefinitions.FirstOrDefault(s => s.Name.Equals("2 Spaltiger Beitrag", StringComparison.InvariantCultureIgnoreCase));
                if (definitionTwoColumn != null) {
                    CreateSectionRequest createTwoColumn = _serviceAgent.CreateRequest(new CreateSectionRequest() {
                        Campaign = new Campaign { Guid = campaignId },
                        Section = this.CreateTwoColumnSection(definitionTwoColumn)
                    });

                    CreateSectionResponse response = _serviceAgent.CreateSection(createTwoColumn);

                    sectionsCreated = sectionsCreated && response != null && response.Guid != Guid.Empty;
                }
            }

            return sectionsCreated;
        }

        /// <summary>
        /// Creates a two columns section.
        /// </summary>
        /// <param name="definitionTwoColumn">The two column definition.</param>
        /// <returns>Returnes the created two column section.</returns>
        private Section CreateTwoColumnSection(SectionDefinition definitionTwoColumn) {
            Section twoColoumn = new Section() {
                Created = DateTime.Now,
                SectionDefinitionName = definitionTwoColumn.Name,
                StatisticName = "section with two columns"
            };

            List<Field> fieldsToAdd = new List<Field>();
            foreach (Field currentField in definitionTwoColumn.Fields) {
                if (currentField.InternalName == "c2_l_img") {
                    Guid fileId = this.UploadFile(Path.Combine(_assetsPath, "messaging.png"), "messaging.png");

                    if (fileId != Guid.Empty) {
                        fieldsToAdd.Add(new MdbField() { InternalName = currentField.InternalName, UntypedValue = fileId.ToString() });
                    }
                }
                else if (currentField.InternalName == "c2_l_text") {
                    fieldsToAdd.Add(new TextField() { InternalName = currentField.InternalName, UntypedValue = @"Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. 
																														Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim,
																														qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi." });
                }
                else if (currentField.InternalName == "c2_r_img") {
                    Guid fileId = this.UploadFile(Path.Combine(_assetsPath, "events.png"), "events.png");

                    if (fileId != Guid.Empty) {
                        fieldsToAdd.Add(new MdbField() { InternalName = currentField.InternalName, UntypedValue = fileId.ToString() });
                    }
                }
                else if (currentField.InternalName == "c2_r_text") {
                    fieldsToAdd.Add(new TextField() {
                        InternalName = currentField.InternalName,
                        UntypedValue = @"Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Lorem ipsum dolor sit amet, consectetuer adipiscing elit,
											   sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo."
                    });
                }
            }

            twoColoumn.Fields = fieldsToAdd.ToArray();
            return twoColoumn;
        }

        /// <summary>
        /// Creates a banner section.
        /// </summary>
        /// <param name="definitionBanner">The banner definition.</param>
        /// <returns>Returnes the created banner section.</returns>
        private Section CreateBannerSection(SectionDefinition definitionBanner) {
            Section banner = new Section() {
                Created = DateTime.Now,
                SectionDefinitionName = definitionBanner.Name,
                StatisticName = "banner"
            };

            List<Field> fieldsToAdd = new List<Field>();
            foreach (Field currentField in definitionBanner.Fields) {
                if (currentField.InternalName == "t_img") {
                    Guid fileId = this.UploadFile(Path.Combine(_assetsPath, "logo.png"), "eMS-logo.png");

                    if (fileId != Guid.Empty) {
                        fieldsToAdd.Add(new MdbField() { InternalName = currentField.InternalName, UntypedValue = fileId.ToString() });
                    }
                }
                else if (currentField.InternalName == "t_text") {
                    fieldsToAdd.Add(
                        new TextField() {
                            InternalName = currentField.InternalName,
                            UntypedValue = @"Developed in the <a href=""http://www.mailworx.info/en/"">mailworx</a> laboratory the intelligent and auto-adaptive algorithm <a href=""http://www.mailworx.info/en/irated-technology"">iRated®</a>
													 brings real progress to your email marketing. It is more than a target group oriented approach.
													 iRated® sorts the sections of your emails automatically depending on the current preferences of every single subscriber.
													 This helps you send individual emails even when you don't know much about the person behind the email address."
                        });
                }
            }

            banner.Fields = fieldsToAdd.ToArray();
            return banner;
        }

        /// <summary>
        /// Creates an article section
        /// </summary>
        /// <param name="definitionArticle">The section definition.</param>
        /// <returns>Returnes the created article section.</returns>
        private Section CreateArticleSection(SectionDefinition definitionArticle) {

            Section sectionArticle = new Section() {
                Created = DateTime.Now,
                SectionDefinitionName = definitionArticle.Name,
                StatisticName = "my first article"
            };

            /*
             * Beware when setting field values: Please send new field-objects and ansure that the InternalName of the field contains the same value than the original field...
             * The different field types use OO paradigms and define the kind of value in the field. Multi-Text-Line, Single-Text-Line, True/False settings etc. They are like datatypes in programming languages.
             * The InternalName and the fieldtype has to match and they define at which field in the section the value will be entered.
             * The fields are defined by the eMS Template (=Layout of the email) and the defined fields there.
             */
            List<Field> fieldsToAdd = new List<Field>();
            foreach (Field currentField in definitionArticle.Fields) {
                if (currentField.InternalName == "a_text") {
                    fieldsToAdd.Add(new TextField() {
                        InternalName = currentField.InternalName,
                        // Beware single quotes do not work for attributes in HTML tags.
                        // If you want to use double quotes for your text, you must use them HTML-encoded.
                        // A text can only be linked with <a> tags and a href attributes. E.g.: <a href=""www.mailworx.info"">go to mailworx website</a>
                        UntypedValue = @"Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy &quot;eirmod tempor&quot; invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. 
												At vero eos et accusam et <a href=""www.mailworx.info"">justo</a> duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. 
												Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. 
												At vero eos et accusam et justo duo dolores et ea rebum.  <a href=""http://sys.mailworx.info/sys/Form.aspx?frm=4bf54eb6-97a6-4f95-a803-5013f0c62b35"">Stet</a> clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet."
                    });
                }
                else if (currentField.InternalName == "a_img") {
                    // Upload the file from the given path to the eMS media data base.
                    Guid fileId = this.UploadFile(Path.Combine(_assetsPath, "email.png"), "email.png");

                    if (fileId != Guid.Empty) {
                        fieldsToAdd.Add(new MdbField() { InternalName = currentField.InternalName, UntypedValue = fileId.ToString() });
                    }
                }
                else if (currentField.InternalName == "a_hl") {
                    fieldsToAdd.Add(new TextField() { InternalName = currentField.InternalName, UntypedValue = "[%mwr:briefanrede%]" });
                }
            }

            sectionArticle.Fields = fieldsToAdd.ToArray();

            return sectionArticle;
        }

        /// <summary>
        /// Loads the section definitions for the given template id.
        /// </summary>
        /// <param name="templateId">The template id.</param>
        /// <returns>Returns a array of section definitions for the given template.</returns>
        private SectionDefinition[] LoadSectionDefinitions(Guid templateId) {
            SectionDefinitionRequest sectionDefinitionRequest = _serviceAgent.CreateRequest(new SectionDefinitionRequest() {
                Template = new Template() { Guid = templateId }
            });

            SectionDefinitionResponse sectionDefinitionResponse = _serviceAgent.GetSectionDefinitions(sectionDefinitionRequest);

            if (sectionDefinitionResponse == null)
                return null;
            else {
                // ### DEMONSTRATE SECTION DEFINITION STRUCTURE ###
                // Here we use the console application in order to demonstrate the structure of each section definition.
                // You need to know the structure in order to be able to create sections on your own.

                Console.WriteLine("-------------------------------Section definitions----------------------");

                for (int i = 0; i < sectionDefinitionResponse.SectionDefinitions.Length; i++) {
                    Console.WriteLine(string.Format("    +++++++++++++++ Section definition {0} +++++++++++++++    ", i + 1));
                    Console.WriteLine(string.Format("    Name:{0}", sectionDefinitionResponse.SectionDefinitions[i].Name));
                    if (sectionDefinitionResponse.SectionDefinitions[i].Fields.Length > 0) {
                        for (int j = 0; j < sectionDefinitionResponse.SectionDefinitions[i].Fields.Length; j++) {
                            Field currentField = sectionDefinitionResponse.SectionDefinitions[i].Fields[j];
                            Type currentFieldType = currentField.GetType();

                            Console.WriteLine(string.Format("        *********** Field {0} ***********", j + 1));
                            Console.WriteLine(string.Format("        Name: {0}", currentField.InternalName));
                            Console.WriteLine(string.Format("        Type: {0}", currentFieldType.Name));

                            if (currentFieldType == typeof(SelectionField)) {
                                Console.WriteLine("                Selections:");

                                for (int k = 0; k < ((SelectionField)currentField).SelectionObjects.Length; k++) {
                                    SelectionFieldElement selcField = ((SelectionField)currentField).SelectionObjects[k];
                                    Console.WriteLine(string.Format("                  Name:{0}", selcField.Caption));
                                    Console.WriteLine(string.Format("                  Value:{0}", selcField.InternalName));
                                }

                                Console.WriteLine($"        *****************************");
                            }
                        }
                    }
                    else {
                        Console.WriteLine($"    No fields found");
                    }

                    Console.WriteLine("    +++++++++++++++++++++++++++++++++++++++    ");
                }

                Console.WriteLine("------------------------------------------------------------------------");

                // ### DEMONSTRATE SECTION DEFINITION STRUCTURE ###

                return sectionDefinitionResponse.SectionDefinitions;
            }
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
