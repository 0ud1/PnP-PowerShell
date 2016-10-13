#Get-SPOGroup
Returns a specific group or all groups.
##Syntax
```powershell
Get-SPOGroup [-Web <WebPipeBind>]
             [-Identity <GroupPipeBind>]
```


```powershell
Get-SPOGroup [-AssociatedMemberGroup [<SwitchParameter>]]
             [-Web <WebPipeBind>]
```


```powershell
Get-SPOGroup [-AssociatedOwnerGroup [<SwitchParameter>]]
             [-Web <WebPipeBind>]
```


```powershell
Get-SPOGroup [-AssociatedVisitorGroup [<SwitchParameter>]]
             [-Web <WebPipeBind>]
```


##Returns
```[System.Collections.Generic.List`1[Microsoft.SharePoint.Client.Group]](https://msdn.microsoft.com/en-us/library/microsoft.sharepoint.client.group.aspx)```

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|AssociatedMemberGroup|SwitchParameter|False|Retrieve the associated member group|
|AssociatedOwnerGroup|SwitchParameter|False|Retrieve the associated owner group|
|AssociatedVisitorGroup|SwitchParameter|False|Retrieve the associated visitor group|
|Identity|GroupPipeBind|False|Get a specific group by name|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
```powershell
PS:> Get-SPOGroup
```
Returns all groups

###Example 2
```powershell
PS:> Get-SPOGroup -Identity 'My Site Users'
```
This will return the group called 'My Site Users' if available

###Example 3
```powershell
PS:> Get-SPOGroup -AssociatedMemberGroup
```
This will return the current members group for the site
