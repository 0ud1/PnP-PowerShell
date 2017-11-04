---
external help file:
applicable: SharePoint Online
schema: 2.0.0
---
# Add-PnPApp

## SYNOPSIS
Add/uploads an available app to the app catalog

## SYNTAX 

```powershell
Add-PnPApp -Path <String>
           [-Connection <SPOnlineConnection>]
```

## EXAMPLES

### ------------------EXAMPLE 1------------------
```powershell
PS:> Add-PnPApp -Path ./myapp.sppkg
```

This will upload the specified app package to the app catalog

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

### -Path
Specifies the Id or an actual app metadata instance

```yaml
Type: String
Parameter Sets: (All)

Required: True
Position: 0
Accept pipeline input: True
```

## OUTPUTS

### OfficeDevPnP.Core.ALM.AppMetadata

# RELATED LINKS

[SharePoint Developer Patterns and Practices](http://aka.ms/sppnp)