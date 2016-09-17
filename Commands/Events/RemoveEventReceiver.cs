﻿using System.Management.Automation;
using Microsoft.SharePoint.Client;
using SharePointPnP.PowerShell.CmdletHelpAttributes;
using SharePointPnP.PowerShell.Commands.Base.PipeBinds;

namespace SharePointPnP.PowerShell.Commands.Events
{
    [Cmdlet(VerbsCommon.Remove, "SPOEventReceiver", SupportsShouldProcess = true)]
    [CmdletHelp("Removes/unregisters a specific event receiver",
        Category = CmdletHelpCategory.EventReceivers)]
    [CmdletExample(
      Code = @"PS:> Remove-SPOEventReceiver -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22",
      Remarks = @"This will remove an event receiver with id fb689d0e-eb99-4f13-beb3-86692fd39f22 from the current web", SortOrder = 1)]
    [CmdletExample(
      Code = @"PS:> Remove-SPOEventReceiver -List ProjectList -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22",
      Remarks = @"This will remove an event receiver with id fb689d0e-eb99-4f13-beb3-86692fd39f22 from the list with name ""ProjectList""", SortOrder = 2)]
    public class RemoveEventReceiver : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The Guid of the event receiver on the list")]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = false, ParameterSetName="List", HelpMessage = "The list object from where to get the event receiver object")]
        public ListPipeBind List;

        [Parameter(Mandatory = false, HelpMessage = "Overwrites the output file if it exists.")]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "List")
            {
                var list = List.GetList(SelectedWeb);

                if (Force || ShouldContinue(Properties.Resources.RemoveEventReceiver, Properties.Resources.Confirm))
                {
                    var eventReceiver = list.GetEventReceiverById(Identity.Id);
                    if(eventReceiver != null)
                    {
                        eventReceiver.DeleteObject();
                        ClientContext.ExecuteQueryRetry();
                    }
                }
            }
            else
            {
                if (Force || ShouldContinue(Properties.Resources.RemoveEventReceiver, Properties.Resources.Confirm))
                {
                    var eventReceiver = SelectedWeb.GetEventReceiverById(Identity.Id);
                    if (eventReceiver != null)
                    {
                        eventReceiver.DeleteObject();
                        ClientContext.ExecuteQueryRetry();
                    }
                }
            }
        }

    }

}


