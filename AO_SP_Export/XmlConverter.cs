using System;
using System.Collections.Generic;
using System.Xml;

namespace AO_SP_Export
{
    internal class XmlConverter
    {
        internal static XmlDocument GetManifestXml(List<EzineItem> ezineItemsForExport)
        {
            var document = XmlHelper.CreateDefaultDocument();

            var spObjects = document.CreateElement("SPObjects");

            foreach (var item in ezineItemsForExport)
            {
                var spObject = document.CreateElement("SPObject");
                spObject.SetAttribute("Id", item.GuidId.ToString());
                spObject.SetAttribute("ObjectType", "SPListItem");
                spObject.SetAttribute("ParentId", item.ParentId);
                spObject.SetAttribute("ParentWebId", item.ParentWebId);
                spObject.SetAttribute("ParentWebUrl", item.ParentWebUrl);
                spObject.SetAttribute("Url", item.Url);

                var listItem = document.CreateElement("ListItem");
                listItem.SetAttribute("FileUrl", item.FileUrl);
                listItem.SetAttribute("DocType", "File");
                listItem.SetAttribute("ParentFolderId", item.ParentFolderId);
                listItem.SetAttribute("Order", item.Order.ToString());
                listItem.SetAttribute("Id", item.GuidId.ToString());
                listItem.SetAttribute("ParentWebId", item.ParentWebId);
                listItem.SetAttribute("ParentListId", item.ParentListId);
                listItem.SetAttribute("Name", item.Name);
                listItem.SetAttribute("DirName", item.DirName);
                listItem.SetAttribute("IntId", item.Id.ToString());
                listItem.SetAttribute("DocId", item.DocId.ToString());
                listItem.SetAttribute("Version", item.Version);
                listItem.SetAttribute("ContentTypeId", item.ContentTypeId);
                listItem.SetAttribute("Author", item.Author);
                listItem.SetAttribute("ModifiedBy", item.ModiiedBy);
                listItem.SetAttribute("TimeLastModified", item.ModifiedOn.ToString("yyyy-MM-dd") + "T" + item.ModifiedOn.ToString("hh:mm:ss"));
                listItem.SetAttribute("TimeCreated", item.CreatedOn.ToString("yyyy-MM-dd") + "T" + item.CreatedOn.ToString("hh:mm:ss"));
                listItem.SetAttribute("ModerationStatus", "Approved");

                var fields = document.CreateElement("Fields");

                var field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "_ModerationComments"); field1.SetAttribute("FieldId", "34ad21eb-75bd-4544-8c73-0e08330291fe");
                fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "Modified_x0020_By"); field1.SetAttribute("FieldId", "822c78e3-1ea9-4943-b449-57863ad33ca9"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "Created_x0020_By"); field1.SetAttribute("FieldId", "4dd7e525-8d6b-4cb4-9d3e-44ee25f973eb"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "File_x0020_Type"); field1.SetAttribute("Value", "aspx"); field1.SetAttribute("FieldId", "39360f11-34cf-4356-9945-25c44e68dade"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "HTML_x0020_File_x0020_Type"); field1.SetAttribute("FieldId", "0c5e0085-eb30-494b-9cdd-ece1d3c649a2"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "_SourceUrl");
                field1.SetAttribute("FieldId", "c63a459d-54ba-4ab7-933a-dcf1c6fadec2"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "_SharedFileIndex"); field1.SetAttribute("FieldId", "034998e9-bf1c-4288-bbbd-00eacfc64410"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "Title"); field1.SetAttribute("Value", "Spoedwet Justitie en Veiligheid in werking getreden - Wet en regelgeving update"); field1.SetAttribute("FieldId", "fa564e0f-0c70-4ab9-b863-0177e6ddd247"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "TemplateUrl"); field1.SetAttribute("FieldId", "4b1bf6c6-4f39-45ac-acd5-16fe7a214e5e"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "xd_ProgID"); field1.SetAttribute("FieldId", "cd1ecb9f-dd4e-4f29-ab9e-e9ff40048d64"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "xd_Signature"); field1.SetAttribute("FieldId", "fbf29b2d-cae5-49aa-8e0a-29955b540122"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "Comments"); field1.SetAttribute("FieldId", "9da97a8a-1da5-4a77-98d3-4bc10456e700"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PublishingStartDate"); field1.SetAttribute("FieldId", "51d39414-03dc-4bd0-b777-d3e20cb350f7"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PublishingExpirationDate"); field1.SetAttribute("FieldId", "a990e64f-faa3-49c1-aafa-885fda79de62"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PublishingContact"); field1.SetAttribute("Value", "13590;UserInfo"); field1.SetAttribute("FieldId", "aea1a4dd-0f19-417d-8721-95a1d28762ab"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PublishingContactEmail"); field1.SetAttribute("FieldId", "c79dba91-e60b-400e-973d-c6d06f192720"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PublishingContactName"); field1.SetAttribute("FieldId", "7546ad0d-6c33-4501-b470-fb3003ca14ba"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PublishingContactPicture"); field1.SetAttribute("FieldId", "dc47d55f-9bf9-494a-8d5b-e619214dd19a"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PublishingPageLayout"); field1.SetAttribute("Value", "/_catalogs/masterpage/AOTeamNewsPage.aspx"); field1.SetAttribute("Value2", "AO Team News Page"); field1.SetAttribute("FieldId", "0f800910-b30d-4c8f-b011-8189b2297094"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PublishingVariationGroupID"); field1.SetAttribute("FieldId", "914fdb80-7d4f-4500-bf4c-ce46ad7484a4"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PublishingVariationRelationshipLinkFieldID"); field1.SetAttribute("FieldId", "766da693-38e5-4b1b-997f-e830b6dfcc7b"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PublishingRollupImage"); field1.SetAttribute("FieldId", "543bc2cf-1f30-488e-8f25-6fe3b689d9ac"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "Audience"); field1.SetAttribute("FieldId", "61cbb965-1e04-4273-b658-eedaa662f48d"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PublishingPageImage"); field1.SetAttribute("FieldId", "3de94b06-4120-41a5-b907-88773e493458"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PublishingPageContent");
                field1.SetAttribute("Value", item.Content);
                field1.SetAttribute("FieldId", "f55c4d88-1f2e-4ad9-aaa8-819af4ee7ee8");
                fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "SummaryLinks"); field1.SetAttribute("FieldId", "b3525efe-59b5-4f0f-b1e4-6e26cb6ef6aa"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "ArticleByLine"); field1.SetAttribute("FieldId", "d3429cc9-adc4-439b-84a8-5679070f84cb"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "ArticleStartDate"); field1.SetAttribute("FieldId", "71316cea-40a0-49f3-8659-f0cefdbdbd4f"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PublishingImageCaption"); field1.SetAttribute("FieldId", "66f500e9-7955-49ab-abb1-663621727d10"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "HeaderStyleDefinitions"); field1.SetAttribute("FieldId", "a932ec3f-94c1-48b1-b6dc-41aaa6eb7e54"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "SummaryLinks2"); field1.SetAttribute("FieldId", "27761311-936a-40ba-80cd-ca5e7a540a36"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "SummaryText"); field1.SetAttribute("FieldId", "8e31c65b-5818-4e67-8a33-ba2bd13ba763"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "KeyContact"); field1.SetAttribute("Value", "13590;UserInfo"); field1.SetAttribute("FieldId", "24b01db4-803d-48e7-9010-c8194d41ec19"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "Client"); field1.SetAttribute("FieldId", "cbe7249f-e57a-45ad-98d3-1e900e08b343"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "ClientTaxHTField0"); field1.SetAttribute("FieldId", "81840176-5413-4607-938a-9a5d6fb0929b"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "TaxCatchAll"); field1.SetAttribute("Value", "54;# ;#8;# ;3141f51b-d550-46fb-9c04-c485d48545cb"); field1.SetAttribute("FieldId", "f3b0adf9-c1a2-4b02-920d-943fba4b3611"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "ReminderDate"); field1.SetAttribute("FieldId", "eaff6595-853a-4345-a595-fd16418a8d00"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "ShowTOC"); field1.SetAttribute("Value", "False"); field1.SetAttribute("FieldId", "a29900e3-55df-489c-bd7c-eeee02b88af1"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "_dlc_Exempt"); field1.SetAttribute("FieldId", "b0227f1a-b179-4d45-855b-a18f03706bcb"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "_dlc_ExpireDateSaved"); field1.SetAttribute("FieldId", "74e6ae8a-0e3e-4dcb-bbff-b5a016d74d64"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "_dlc_ExpireDate"); field1.SetAttribute("Value", "04/29/2021 08:12:30"); field1.SetAttribute("FieldId", "acd16fdf-052f-40f7-bb7e-564c269c9fbc"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "LocationsTaxHTField0"); field1.SetAttribute("Value", "Netherlands|b167894c-408e-41f0-b893-5fd30e1865ea"); field1.SetAttribute("FieldId", "b52157de-08ea-4cf1-a8e9-aad26a2afc35"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "Locations"); field1.SetAttribute("Value", "54;# ;3141f51b-d550-46fb-9c04-c485d48545cb"); field1.SetAttribute("FieldId", "52ddadbd-abb6-4b44-abc7-356514ec1a4b"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "TeamsTaxHTField0"); field1.SetAttribute("Value", "Corporate|d51735ec-5598-41c3-97eb-c84856476a28"); field1.SetAttribute("FieldId", "fcce508d-aa07-4eb7-a129-1418c4b93177"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "Teams"); field1.SetAttribute("Value", "8;# ;3141f51b-d550-46fb-9c04-c485d48545cb"); field1.SetAttribute("FieldId", "531405fc-48fe-4a3c-beec-5101269abbb8"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "j1552d3b52fc4616b1bbf12bd8159db7"); field1.SetAttribute("FieldId", "c66d690f-b6f1-436f-847f-7e31d2edf543"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "Page_x0020_Category"); field1.SetAttribute("FieldId", "31552d3b-52fc-4616-b1bb-f12bd8159db7"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "ArticleImage"); field1.SetAttribute("Value", "&lt;img alt=&quot;&quot; src=&quot;/Locations/Europe/Netherlands/NL%20Corporate/PublishingImages/Rijksoverheid1a.jpg&quot; style=&quot;border:0px solid&quot; /&gt;"); field1.SetAttribute("FieldId", "bf348435-b5cb-46c8-9095-6509800a4b69"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "AuthorsAndRelatedLawyers"); field1.SetAttribute("FieldId", "4b830c37-40ad-4dae-b3d1-937128357450"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "ArticleDate"); field1.SetAttribute("Value", "04/28/2020 23:00:00"); field1.SetAttribute("FieldId", "c8c5ab92-bb03-4cc4-91d7-61e186abecd3"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "ArchiveDate"); field1.SetAttribute("FieldId", "882522ef-2557-46bd-a4d5-c14b79f0b626"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "LocationSingle"); field1.SetAttribute("FieldId", "5fc26f60-1175-4003-a243-51e049ce8a83"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "LocationSingleTaxHTField0"); field1.SetAttribute("FieldId", "2e7fee35-4b89-4532-9764-d35baf9495fc"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "Team"); field1.SetAttribute("FieldId", "4180e4a2-595f-4cd6-85b4-4efd30bcaefe"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "TeamTaxHTField0"); field1.SetAttribute("FieldId", "03bbfd10-acd0-4a58-bc4b-d301894f6a16"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "KnowHowLevel2Grouping"); field1.SetAttribute("FieldId", "fe0c545d-7d5e-41c3-b2da-142bc7bb91ce"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "LibrarySubContentType"); field1.SetAttribute("FieldId", "cf99db5b-8282-441e-a8ca-ece37288ec54"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "LibraryCategory"); field1.SetAttribute("FieldId", "38dbe6f8-0be7-49e1-9617-6e1ee74a287f"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "LibraryCategoryTaxHTField0"); field1.SetAttribute("FieldId", "5412a3d1-9763-467b-8cea-85c56278d82f"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "c271ddbd8a6041f6b590f9d8d553e1c2"); field1.SetAttribute("FieldId", "845f6d97-6e37-4a4f-a410-f58996db9bcd"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "KnowledgeJurisdiction"); field1.SetAttribute("FieldId", "c271ddbd-8a60-41f6-b590-f9d8d553e1c2"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "hb15562ae8d342639eaee52eac6fed22"); field1.SetAttribute("FieldId", "197da4bc-fe99-4a5b-b57a-266470510d8c"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "KnowledgeExpertise"); field1.SetAttribute("FieldId", "1b15562a-e8d3-4263-9eae-e52eac6fed22"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "GroupCollectionOwner"); field1.SetAttribute("FieldId", "3d543c7b-09bf-4c3a-971d-febb7906cbfd"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "GroupCollectionOwnerEmail"); field1.SetAttribute("FieldId", "f8f96077-e3b6-47af-ab85-992a86e6b281"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "KnowledgeTableOfContents"); field1.SetAttribute("Value", "True"); field1.SetAttribute("FieldId", "5a2dd491-42c9-42a8-89c7-cd9af4d74c40"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "KnowledgeCollectionDescription"); field1.SetAttribute("FieldId", "f7ac7132-dac9-48d7-9863-f51e078957a6"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "icd605e340f2473fa042948b28310134"); field1.SetAttribute("FieldId", "3191f0a5-e3cb-4e27-a437-90f9e7f097c4"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "KnowledgeLanguage"); field1.SetAttribute("FieldId", "2cd605e3-40f2-473f-a042-948b28310134"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "UIVersion"); field1.SetAttribute("Value", ";#4;#"); field1.SetAttribute("FieldId", "8e334549-c2bd-4110-9f61-672971be6504"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "Practices"); field1.SetAttribute("FieldId", "303f3cd2-204b-447c-90a2-e52beae9be02"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "PracticesTaxHTField0"); field1.SetAttribute("FieldId", "f1c361ac-2182-49e0-b376-dae33bc3c13a"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "Wiki_x0020_Page_x0020_Categories"); field1.SetAttribute("FieldId", "e1a5b98c-dd71-426d-acb6-e478c7a5882f"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "e1a5b98cdd71426dacb6e478c7a5882f"); field1.SetAttribute("FieldId", "f863c21f-5fdb-4a91-bb0c-5ae889190dd7"); fields.AppendChild(field1);

                field1 = document.CreateElement("Field");
                field1.SetAttribute("Name", "Categories"); field1.SetAttribute("FieldId", "9ebcd900-9d05-46c8-8f4d-e46e87328844"); fields.AppendChild(field1);


                listItem.AppendChild(fields);

                spObject.AppendChild(listItem);

                spObjects.AppendChild(spObject);
            }

            document.AppendChild(spObjects);
            return document;
        }

        internal static XmlDocument GetRequirements()
        {
            var document = XmlHelper.CreateDefaultDocument();

            var requirements = document.CreateElement("Requirements");

            var requirement = document.CreateElement("Requirement");
            requirement.SetAttribute("Type", "Language");
            requirement.SetAttribute("Id", "1033");
            requirement.SetAttribute("Name", "English");

            requirements.AppendChild(requirement);
            document.AppendChild(requirements);
            return document;
        }

        internal static XmlDocument GetExportSettings(int increment)
        {
            var document = XmlHelper.CreateDefaultDocument();

            var exportSettings = document.CreateElement("ExportSettings");
            exportSettings.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            exportSettings.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
            exportSettings.SetAttribute("SiteUrl", "http://global.intranet.allenovery.com/");
            exportSettings.SetAttribute("FileLocation", @"c:\temp");
            exportSettings.SetAttribute("BaseFileName", $"Import{increment}.cmp");
            exportSettings.SetAttribute("IncludeSecurity", "None");
            exportSettings.SetAttribute("IncludeVersions", "LastMajor");
            exportSettings.SetAttribute("ExportPublicSchema", "true");
            exportSettings.SetAttribute("ExportFrontEndFileStreams", "true");
            exportSettings.SetAttribute("ExportMethod", "ExportAll");
            exportSettings.SetAttribute("ExcludeDependencies", "true");
            exportSettings.SetAttribute("xmlns", "urn:deployment-exportsettings-schema");

            var exportObjects = document.CreateElement("ExportObjects");

            var deploymentObject = document.CreateElement("DeploymentObject");
            deploymentObject.SetAttribute("Id", "5f84a434-ada7-42eb-9af3-83a1c6907365");
            deploymentObject.SetAttribute("Type", "Web");
            deploymentObject.SetAttribute("ParentId", "00000000-0000-0000-0000-000000000000");
            deploymentObject.SetAttribute("Url", "/Europe/Netherlands");
            deploymentObject.SetAttribute("ExcludeChildren", "false");
            deploymentObject.SetAttribute("IncludeDescendants", "All");

            exportObjects.AppendChild(deploymentObject);
            exportSettings.AppendChild(exportObjects);
            document.AppendChild(exportSettings);
            return document;
        }

        internal static XmlDocument GetSystemData(int numOfExportItems)
        {
            var document = XmlHelper.CreateDefaultDocument();

            var systemData = document.CreateElement("SystemData");
            systemData.SetAttribute("xmlns", "urn:deployment-systemdata-schema");

            var schemaVersion = document.CreateElement("SchemaVersion");
            schemaVersion.SetAttribute("Version", "14.0.0.0");
            schemaVersion.SetAttribute("Build", "14.0.7143.5000");
            schemaVersion.SetAttribute("DatabaseVersion", "13650364");
            schemaVersion.SetAttribute("SiteVersion", "0");
            schemaVersion.SetAttribute("ObjectsProcessed", numOfExportItems.ToString());

            var manifestFiles = document.CreateElement("ManifestFiles");
            var manifestFile = document.CreateElement("ManifestFile");
            manifestFile.SetAttribute("Name", "Manifest.xml");

            var systemObjects = document.CreateElement("SystemObjects");

/*            var systemObject = document.CreateElement("SystemObject");
            systemObject.SetAttribute("Id", "1a293eb8-2b49-40cd-832d-166d5ae70861");
            systemObject.SetAttribute("Type", "Folder");
            systemObject.SetAttribute("Url", "/Locations/Europe/Netherlands/NL Corporate/images");

            systemObjects.AppendChild(systemObject);
            */
            var rootWebOnlyLists = document.CreateElement("RootWebOnlyLists");

            manifestFiles.AppendChild(manifestFile);

            systemData.AppendChild(schemaVersion);
            systemData.AppendChild(manifestFiles);
            systemData.AppendChild(systemObjects);
            systemData.AppendChild(rootWebOnlyLists);

            document.AppendChild(systemData);

            return document;
        }
    }
}