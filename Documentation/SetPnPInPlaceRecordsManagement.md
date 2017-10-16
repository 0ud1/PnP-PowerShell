# Set-PnPInPlaceRecordsManagement
Activates or deactivates in the place records management feature.
## Syntax
```powershell
Set-PnPInPlaceRecordsManagement -On [<SwitchParameter>]
                                [-Web <WebPipeBind>]
```


```powershell
Set-PnPInPlaceRecordsManagement -Off [<SwitchParameter>]
                                [-Web <WebPipeBind>]
```


## Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Off|SwitchParameter|True|Turn records management off|
|On|SwitchParameter|True|Turn records management on|
|Web|WebPipeBind|False|The GUID, server relative url (i.e. /sites/team1) or web instance of the web to apply the command to. Omit this parameter to use the current web.|
## Examples

### Example 1
```powershell
PS:> Set-PnPInPlaceRecordsManagement -On
```
Activates In Place Records Management
