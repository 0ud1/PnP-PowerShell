#Get-SPOSubWebs
Returns the subwebs
##Syntax
```powershell
Get-SPOSubWebs [-Recurse [<SwitchParameter>]] [-Web <WebPipeBind>] [-Identity <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Identity|WebPipeBind|False||
|Recurse|SwitchParameter|False||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
