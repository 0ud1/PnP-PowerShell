﻿using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using SharePointPnP.PowerShell.CmdletHelpAttributes;
using SharePointPnP.PowerShell.Commands.Base;
using SharePointPnP.PowerShell.Commands.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace SharePointPnP.PowerShell.Commands.Provisioning
{
    [Cmdlet("Apply", "PnPProvisioningHierarchy", SupportsShouldProcess = true)]
    [CmdletHelp("Adds a provisioning sequence object to a provisioning site object",
        Category = CmdletHelpCategory.Provisioning)]
    [CmdletExample(
       Code = @"PS:> Add-PnPProvisioningSequence -Hierarchy $myhierarchy -Sequence $mysequence",
       Remarks = "Adds an existing sequence object to an existing hierarchy object",
       SortOrder = 1)]
    [CmdletExample(
       Code = @"PS:> New-PnPProvisioningSequence -Id ""MySequence"" | Add-PnPProvisioningSequence -Hierarchy $hierarchy",
       Remarks = "Creates a new instance of a provisioning sequence object and sets the Id to the value specified, then the sequence is added to an existing hierarchy object",
       SortOrder = 2)]
    public class ApplyProvisioningHierarchy : PnPAdminCmdlet
    {
        private const string ParameterSet_PATH = "By Path";
        private const string ParameterSet_OBJECT = "By Object";

        private ProgressRecord progressRecord = new ProgressRecord(0, "Activity", "Status");
        private ProgressRecord subProgressRecord = new ProgressRecord(1, "Activity", "Status");

        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, HelpMessage = "Path to the xml or pnp file containing the provisioning hierarchy.", ParameterSetName = ParameterSet_PATH)]
        public string Path;

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = ParameterSet_OBJECT)]
        public ProvisioningHierarchy Hierarchy;

        [Parameter(Mandatory = false)]
        public string SequenceId;

        [Parameter(Mandatory = false, HelpMessage = "Root folder where resources/files that are being referenced in the template are located. If not specified the same folder as where the provisioning template is located will be used.", ParameterSetName = ParameterAttribute.AllParameterSets)]
        public string ResourceFolder;

        [Parameter(Mandatory = false, HelpMessage = "Allows you to only process a specific part of the template. Notice that this might fail, as some of the handlers require other artifacts in place if they are not part of what your applying.", ParameterSetName = ParameterAttribute.AllParameterSets)]
        public Handlers Handlers;

        [Parameter(Mandatory = false, HelpMessage = "Allows you to run all handlers, excluding the ones specified.", ParameterSetName = ParameterAttribute.AllParameterSets)]
        public Handlers ExcludeHandlers;

        [Parameter(Mandatory = false, HelpMessage = "Allows you to specify ExtensbilityHandlers to execute while applying a template", ParameterSetName = ParameterAttribute.AllParameterSets)]
        public ExtensibilityHandler[] ExtensibilityHandlers;

        [Parameter(Mandatory = false, HelpMessage = "Allows you to specify ITemplateProviderExtension to execute while applying a template.", ParameterSetName = ParameterAttribute.AllParameterSets)]
        public ITemplateProviderExtension[] TemplateProviderExtensions;

        [Parameter(Mandatory = false, HelpMessage = "Specify this parameter if you want to overwrite and/or create properties that are known to be system entries (starting with vti_, dlc_, etc.)", ParameterSetName = ParameterAttribute.AllParameterSets)]
        public SwitchParameter OverwriteSystemPropertyBagValues;

        [Parameter(Mandatory = false, HelpMessage = "Ignore duplicate data row errors when the data row in the template already exists.", ParameterSetName = ParameterAttribute.AllParameterSets)]
        public SwitchParameter IgnoreDuplicateDataRowErrors;

        [Parameter(Mandatory = false, HelpMessage = "If set content types will be provisioned if the target web is a subweb.")]
        public SwitchParameter ProvisionContentTypesToSubWebs;

        [Parameter(Mandatory = false, HelpMessage = "If set fields will be provisioned if the target web is a subweb.")]
        public SwitchParameter ProvisionFieldsToSubWebs;

        [Parameter(Mandatory = false, HelpMessage = "Override the RemoveExistingNodes attribute in the Navigation elements of the template. If you specify this value the navigation nodes will always be removed before adding the nodes in the template")]
        public SwitchParameter ClearNavigation;

        protected override void ExecuteCmdlet()
        {
            var applyingInformation = new ProvisioningTemplateApplyingInformation();

            if (MyInvocation.BoundParameters.ContainsKey("Handlers"))
            {
                applyingInformation.HandlersToProcess = Handlers;
            }
            if (MyInvocation.BoundParameters.ContainsKey("ExcludeHandlers"))
            {
                foreach (var handler in (Handlers[])Enum.GetValues(typeof(Handlers)))
                {
                    if (!ExcludeHandlers.Has(handler) && handler != Handlers.All)
                    {
                        Handlers = Handlers | handler;
                    }
                }
                applyingInformation.HandlersToProcess = Handlers;
            }

            if (ExtensibilityHandlers != null)
            {
                applyingInformation.ExtensibilityHandlers = ExtensibilityHandlers.ToList();
            }

            applyingInformation.ProgressDelegate = (message, step, total) =>
            {
                if (message != null)
                {
                    var percentage = Convert.ToInt32((100 / Convert.ToDouble(total)) * Convert.ToDouble(step));
                    progressRecord.Activity = $"Applying template to tenant";
                    progressRecord.StatusDescription = message;
                    progressRecord.PercentComplete = percentage;
                    progressRecord.RecordType = ProgressRecordType.Processing;
                    WriteProgress(progressRecord);
                }
            };

            var warningsShown = new List<string>();

            applyingInformation.MessagesDelegate = (message, type) =>
            {
                switch (type)
                {
                    case ProvisioningMessageType.Warning:
                        {
                            if (!warningsShown.Contains(message))
                            {
                                WriteWarning(message);
                                warningsShown.Add(message);
                            }
                            break;
                        }
                    case ProvisioningMessageType.Progress:
                        {
                            if (message != null)
                            {
                                var activity = message;
                                if (message.IndexOf("|") > -1)
                                {
                                    var messageSplitted = message.Split('|');
                                    if (messageSplitted.Length == 4)
                                    {
                                        var current = double.Parse(messageSplitted[2]);
                                        var total = double.Parse(messageSplitted[3]);
                                        subProgressRecord.RecordType = ProgressRecordType.Processing;
                                        subProgressRecord.Activity = string.IsNullOrEmpty(messageSplitted[0]) ? "-" : messageSplitted[0];
                                        subProgressRecord.StatusDescription = string.IsNullOrEmpty(messageSplitted[1]) ? "-" : messageSplitted[1];
                                        subProgressRecord.PercentComplete = Convert.ToInt32((100 / total) * current);
                                        WriteProgress(subProgressRecord);
                                    }
                                    else
                                    {
                                        subProgressRecord.Activity = "Processing";
                                        subProgressRecord.RecordType = ProgressRecordType.Processing;
                                        subProgressRecord.StatusDescription = activity;
                                        subProgressRecord.PercentComplete = 0;
                                        WriteProgress(subProgressRecord);
                                    }
                                }
                                else
                                {
                                    subProgressRecord.Activity = "Processing";
                                    subProgressRecord.RecordType = ProgressRecordType.Processing;
                                    subProgressRecord.StatusDescription = activity;
                                    subProgressRecord.PercentComplete = 0;
                                    WriteProgress(subProgressRecord);
                                }
                            }
                            break;
                        }
                    case ProvisioningMessageType.Completed:
                        {

                            WriteProgress(new ProgressRecord(1, message, " ") { RecordType = ProgressRecordType.Completed });
                            break;
                        }
                }
            };

            applyingInformation.OverwriteSystemPropertyBagValues = OverwriteSystemPropertyBagValues;
            applyingInformation.IgnoreDuplicateDataRowErrors = IgnoreDuplicateDataRowErrors;
            applyingInformation.ClearNavigation = ClearNavigation;
            applyingInformation.ProvisionContentTypesToSubWebs = ProvisionContentTypesToSubWebs;
            applyingInformation.ProvisionFieldsToSubWebs = ProvisionFieldsToSubWebs;

            ProvisioningHierarchy hierarchyToApply = null;

            switch(ParameterSetName)
            {
                case ParameterSet_PATH:
                    {
                        hierarchyToApply = GetHierarchy();
                        break;
                    }
                case ParameterSet_OBJECT:
                    {
                        hierarchyToApply = Hierarchy;
                        if (ResourceFolder != null)
                        {
                            var fileSystemConnector = new FileSystemConnector(ResourceFolder, "");
                            hierarchyToApply.Connector = fileSystemConnector;
                        }
                        else
                        {
                            if (Path != null)
                            {
                                if (!System.IO.Path.IsPathRooted(Path))
                                {
                                    Path = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Path);
                                }
                            }
                            else
                            {
                                Path = SessionState.Path.CurrentFileSystemLocation.Path;
                            }
                            var fileInfo = new FileInfo(Path);
                            var fileConnector = new FileSystemConnector(fileInfo.DirectoryName, "");
                            hierarchyToApply.Connector = fileConnector;
                        }
                        break;
                    }
            }
            if (!string.IsNullOrEmpty(SequenceId))
            {
                Tenant.ApplyProvisionHierarchy(hierarchyToApply, SequenceId, applyingInformation);
            }
            else
            {
                foreach (var sequence in hierarchyToApply.Sequences)
                {
                    Tenant.ApplyProvisionHierarchy(hierarchyToApply, sequence.ID, applyingInformation);
                }
            }

        }

        private ProvisioningHierarchy GetHierarchy()
        {
            ProvisioningHierarchy hierarchy = null;
            FileConnectorBase fileConnector;

            bool templateFromFileSystem = !Path.ToLower().StartsWith("http");
            string templateFileName = System.IO.Path.GetFileName(Path);
            if (templateFromFileSystem)
            {
                if (!System.IO.Path.IsPathRooted(Path))
                {
                    Path = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Path);
                }
                if (!System.IO.File.Exists(Path))
                {
                    throw new FileNotFoundException($"File not found");
                }
                if (!string.IsNullOrEmpty(ResourceFolder))
                {
                    if (!System.IO.Path.IsPathRooted(ResourceFolder))
                    {
                        ResourceFolder = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path,
                            ResourceFolder);
                    }
                }
                var fileInfo = new FileInfo(Path);
                fileConnector = new FileSystemConnector(fileInfo.DirectoryName, "");
            }
            else
            {
                Uri fileUri = new Uri(Path);
                var webUrl = Microsoft.SharePoint.Client.Web.WebUrlFromFolderUrlDirect(ClientContext, fileUri);
                var templateContext = ClientContext.Clone(webUrl.ToString());

                var library = Path.ToLower().Replace(templateContext.Url.ToLower(), "").TrimStart('/');
                var idx = library.IndexOf("/", StringComparison.Ordinal);
                library = library.Substring(0, idx);

                // This syntax creates a SharePoint connector regardless we have the -InputInstance argument or not
                fileConnector = new SharePointConnector(templateContext, templateContext.Url, library);
            }

            // If we don't have the -InputInstance parameter, we load the template from the source connector

            using (var stream = fileConnector.GetFileStream(templateFileName))
            {
                var isOpenOfficeFile = FileUtilities.IsOpenOfficeFile(stream);
                XMLTemplateProvider provider;
                if (isOpenOfficeFile)
                {
                    provider = new XMLOpenXMLTemplateProvider(new OpenXMLConnector(templateFileName, fileConnector));
                    templateFileName = templateFileName.Substring(0, templateFileName.LastIndexOf(".", StringComparison.Ordinal)) + ".xml";
                }
                else
                {
                    if (templateFromFileSystem)
                    {
                        provider = new XMLFileSystemTemplateProvider(Path, "");
                    }
                    else
                    {
                        throw new NotSupportedException("Only .pnp package files are supported from a SharePoint library");
                    }
                }

                var formatter = XMLPnPSchemaFormatter.LatestFormatter;
                formatter.Initialize(provider);

                var serializer = (IProvisioningHierarchyFormatter)formatter;
                
                hierarchy = serializer.ToProvisioningHierarchy(provider.Connector.GetFileStream(Path));

                hierarchy.Connector = fileConnector;
                if (hierarchy == null)
                {
                    // If we don't have the template, raise an error and exit
                    WriteError(new ErrorRecord(new Exception("The -Path parameter targets an invalid repository or template object."), "WRONG_PATH", ErrorCategory.SyntaxError, null));
                    return null;
                }

                //if (isOpenOfficeFile)
                //{
                //    hierarchy.Connector = provider.Connector;
                //}
                //else
                //{
                //    if (ResourceFolder != null)
                //    {
                //        var fileSystemConnector = new FileSystemConnector(ResourceFolder, "");
                //        provisioningTemplate.Connector = fileSystemConnector;
                //    }
                //    else
                //    {
                //        provisioningTemplate.Connector = provider.Connector;
                //    }
                //}

                return hierarchy;
            }
        }
    }
}
