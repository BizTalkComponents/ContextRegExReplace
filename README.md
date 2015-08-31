[![Build status](https://ci.appveyor.com/api/projects/status/github/BizTalkComponents/ContextRegExReplace?branch=master)](https://ci.appveyor.com/api/projects/status/github/BizTalkComponents/ContextRegExReplace/branch/master)

##Description
Replaces a context value that matches a specific pattern with a hard coded string.

| Parameter       | Description                         | Type| Validation|
| ----------------|-------------------------------------|-----|-----------|
|Pattern To Replace|Regular expression to use when replacing.|String|Required|
|Value to Set|The value to replace any matches on the RegEx pattern with.|String|Required|
|Context Namespace|The path to the context property to replace.|String|Required, Format = namespace#property|


