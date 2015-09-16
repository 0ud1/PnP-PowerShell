#Add-SPOContentTypeToList
*Topic automatically generated on: 2015-09-17*

Adds a new content type to a list
##Syntax
```powershell
Add-SPOContentTypeToList -List <ListPipeBind> -ContentType <ContentTypePipeBind> [-DefaultContentType [<SwitchParameter>]] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|ContentType|ContentTypePipeBind|True||
|DefaultContentType|SwitchParameter|False||
|List|ListPipeBind|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
```powershell
PS:> Add-SPOContentTypeToList -List "Documents" -ContentType "Project Document" -DefaultContentType
```
This will add an existing content type to a list and sets it as the default content type
<!-- Ref: 1DCA6AC33BEFB573BFCB181F9F7F49AE -->