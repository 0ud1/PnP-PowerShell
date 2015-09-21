﻿using System.Linq;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOWebPart")]
    [CmdletHelp("Removes a webpart from a page",
        Category = CmdletHelpCategory.WebParts)]
    public class RemoveWebPart : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "ID")]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = true, ParameterSetName = "NAME")]
        public string Name = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = ParameterAttribute.AllParameterSets)]
        [Alias("PageUrl")]
        public string ServerRelativePageUrl = string.Empty;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "NAME")
            {
                SelectedWeb.DeleteWebPart(ServerRelativePageUrl, Name);
            }
            else
            {
                var wps = SelectedWeb.GetWebParts(ServerRelativePageUrl);
                var wp = from w in wps where w.Id == Identity.Id select w;
                if(wp.Any())
                {
                    wp.FirstOrDefault().DeleteWebPart();
                    ClientContext.ExecuteQueryRetry();
                }
            }
        }
    }
}
