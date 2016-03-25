#Get-SPOWorkflowSubscription
Returns a workflow subscriptions from a list
##Syntax
```powershell
Get-SPOWorkflowSubscription [-Web <WebPipeBind>] [-Name <String>] [-List <ListPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|List|ListPipeBind|False|A list to search the association for|
|Name|String|False|The name of the workflow|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
