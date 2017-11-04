---
external help file:
applicable: SharePoint Server 2013, SharePoint Server 2016, SharePoint Online
schema: 2.0.0
---
# Get-PnPSite

## SYNOPSIS
Returns the current site collection from the context.

## SYNTAX 

### 
```powershell
Get-PnPSite [-Connection <SPOnlineConnection>]
            [-Includes <String[]>]
```

## EXAMPLES

### ------------------EXAMPLE 1------------------
```powershell
PS:> Get-PnPSite
```

Gets the current site

## PARAMETERS

### -Connection
Connection to be used by cmdlet

```yaml
Type: SPOnlineConnection
Parameter Sets: (All)

Required: False
Position: Named
Accept pipeline input: False
```

### -Includes
Specify properties to include when retrieving objects from the server.

```yaml
Type: String[]
Parameter Sets: 

Required: False
Position: 0
Accept pipeline input: False
```

## OUTPUTS

### [Microsoft.SharePoint.Client.Site](https://msdn.microsoft.com/en-us/library/microsoft.sharepoint.client.site.aspx)

# RELATED LINKS

[SharePoint Developer Patterns and Practices](http://aka.ms/sppnp)