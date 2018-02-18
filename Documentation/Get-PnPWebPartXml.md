---
external help file:
applicable: SharePoint Server 2013, SharePoint Server 2016, SharePoint Online
schema: 2.0.0
---
# Get-PnPWebPartXml

## SYNOPSIS
Returns the webpart XML of a webpart registered on a site

## SYNTAX 

### 
```powershell
Get-PnPWebPartXml [-ServerRelativePageUrl <String>]
                  [-Identity <WebPartPipeBind>]
                  [-Web <WebPipeBind>]
                  [-Connection <SPOnlineConnection>]
```

## EXAMPLES

### ------------------EXAMPLE 1------------------
```powershell
PS:> Get-PnPWebPartXml -ServerRelativePageUrl "/sites/demo/sitepages/home.aspx" -Identity a2875399-d6ff-43a0-96da-be6ae5875f82
```

Returns the webpart XML for a given webpart on a page.

## PARAMETERS

### -Identity


```yaml
Type: WebPartPipeBind
Parameter Sets: 

Required: False
Position: 0
Accept pipeline input: False
```

### -ServerRelativePageUrl


```yaml
Type: String
Parameter Sets: 
Aliases: new String[1] { "PageUrl" }

Required: False
Position: 0
Accept pipeline input: False
```

### -Connection


```yaml
Type: SPOnlineConnection
Parameter Sets: 

Required: False
Position: 0
Accept pipeline input: False
```

### -Web


```yaml
Type: WebPipeBind
Parameter Sets: 

Required: False
Position: 0
Accept pipeline input: False
```

## OUTPUTS

### System.String

## RELATED LINKS

[SharePoint Developer Patterns and Practices](http://aka.ms/sppnp)