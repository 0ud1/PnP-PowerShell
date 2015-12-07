#Get-SPOWikiPageContent
Gets the contents/source of a wiki page
##Syntax
```powershell
Get-SPOWikiPageContent [-Web <WebPipeBind>] -ServerRelativePageUrl <String>
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|ServerRelativePageUrl|String|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
