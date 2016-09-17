﻿using System.Management.Automation;
using Microsoft.SharePoint.Client;
using SharePointPnP.PowerShell.CmdletHelpAttributes;
using SharePointPnP.PowerShell.Commands.Base.PipeBinds;

namespace SharePointPnP.PowerShell.Commands.ContentTypes
{

    [Cmdlet(VerbsCommon.Set, "SPODefaultContentTypeToList")]
    [CmdletHelp("Sets the default content type for a list", 
        Category = CmdletHelpCategory.ContentTypes)]
    [CmdletExample(
        Code = @"PS:> Set-SPODefaultContentTypeToList -List ""Project Documents"" -ContentType ""Project""",
        Remarks = @"This will set the Project content type (which has already been added to a list) as the default content type", 
        SortOrder = 1)]
    public class SetDefaultContentTypeToList : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The list object where the content type needs to be added")]
        public ListPipeBind List;

        [Parameter(Mandatory = true, HelpMessage = "The content type object that needs to be added to the list")]
        public ContentTypePipeBind ContentType;

        protected override void ExecuteCmdlet()
        {
            ContentType ct = null;
            List list = List.GetList(SelectedWeb);

            if (ContentType.ContentType == null)
            {
                if (ContentType.Id != null)
                {
                    ct = SelectedWeb.GetContentTypeById(ContentType.Id,true);
                }
                else if (ContentType.Name != null)
                {
                    ct = SelectedWeb.GetContentTypeByName(ContentType.Name,true);
                }
            }
            else
            {
                ct = ContentType.ContentType;
            }
            if (ct != null)
            {
                SelectedWeb.SetDefaultContentTypeToList(list, ct);
            }
        }

    }
}
