# Install-PnPApp
Installs an available app from the app catalog
>*Only available for SharePoint Online*
## Syntax
```powershell
Install-PnPApp -Identity <AppMetadataPipeBind>
```


## Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Identity|AppMetadataPipeBind|True|Specifies the Id or an actual app metadata instance|
## Examples

### Example 1
```powershell
PS:> Install-PnPApp -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe
```
This will install an available app, specified by the id, to the current site.

### Example 2
```powershell
PS:> Get-PnPAvailableApp -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe | Install-PnPApp
```
This will install the given app into the site.
