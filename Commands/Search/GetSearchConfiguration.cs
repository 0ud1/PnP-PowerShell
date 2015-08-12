﻿using System;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Administration;
using Microsoft.SharePoint.Client.Search.Portability;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Enums;
using Resources = OfficeDevPnP.PowerShell.Commands.Properties.Resources;

namespace OfficeDevPnP.PowerShell.Commands.Search
{
    [Cmdlet(VerbsCommon.Get, "SPOSearchConfiguration")]
    [CmdletHelp("Returns the search configuration", Category = "Search")]
    [CmdletExample(
        Code = @"PS:> Get-SPOSearchConfiguration",
        Remarks = "Returns the search configuration for the current web",
        SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Get-SPOSearchConfiguration -Scope Site",
        Remarks = "Returns the search configuration for the current site collection",
        SortOrder = 2)]
    [CmdletExample(
        Code = @"PS:> Get-SPOSearchConfiguration -Scope Subscription",
        Remarks = "Returns the search configuration for the current tenant",
        SortOrder = 3)]
    public class GetSearchConfiguration : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public SearchConfigurationScope Scope = SearchConfigurationScope.Web;

        protected override void ExecuteCmdlet()
        {
            switch (Scope)
            {
                case SearchConfigurationScope.Web:
                    {
                        WriteObject(this.SelectedWeb.GetSearchConfiguration());
                        break;
                    }
                case SearchConfigurationScope.Site:
                    {
                        WriteObject(ClientContext.Site.GetSearchConfiguration());
                        break;
                    }
                case SearchConfigurationScope.Subscription:
                {
                    if (!ClientContext.Url.ToLower().Contains("-admin"))
                    {
                        throw new InvalidOperationException(Resources.CurrentSiteIsNoTenantAdminSite);
                    }

                    SearchObjectOwner owningScope = new SearchObjectOwner(ClientContext, SearchObjectLevel.SPSiteSubscription);
                    var config = new SearchConfigurationPortability(ClientContext);
                    ClientResult<string> configuration = config.ExportSearchConfiguration(owningScope);                
                    ClientContext.ExecuteQueryRetry(10, 60*5*1000);
                    WriteObject(configuration.Value);
                    break;
                }
            }
        }
    }
}
