#  RollingOutTools - all my coolest libraries
<br />Here you can see solution with may libraries that i use in own projects.
<br />This assemblies is unified and i can make it opensource.
<br />Now i use it via nuget local folder, maybe some day i will upload current libraries into nuget.org .
<br />
<br />Why "rolling out tools"? It`s name of my team, where we share expirience and just communicate.
<br />
<br />Some info about projects:
<br />&nbsp;&nbsp;&nbsp;&nbsp;.CmdLine - one of my best sources, this utility make CLI development much easier. 
It can automatically build CLI from classes, automatically read/write ANY type from console (using serialization for difficult objects), cache values for each called command (and you can run it like simple autotest by pressing enter) and more.
<br />&nbsp;&nbsp;&nbsp;&nbsp;.ReflectionVisit - utility to call methods from classes by its names, with reflection caching, awaiting async methods and getting its values (note, that it is difficult, because you don`t know type of returning value). Can be used for building CLI too or, for examle, own web framework.
<br />&nbsp;&nbsp;&nbsp;&nbsp;.PureApi.AspNetCore.Json - cool lib, that extend asp.net web api models resolving. Just use [FromPureApi] attribute in controller method params like you always do it with [FromBody] attribute and etc. But this utility allow to use more than one parameters, when you sending it through http method body in json. This lib add no limitations to standart asp.net controllers, because os use it`s request pipeline. 
<br />&nbsp;&nbsp;&nbsp;&nbsp;.Storage - simple generic key/value storage. Platform dependencies can be injected.
<br />&nbsp;&nbsp;&nbsp;&nbsp;.Json - just wrapper on Newtonsoft.Json with some extensions.
<br />&nbsp;&nbsp;&nbsp;&nbsp;.SimpleIoc - wrapper on ioc container. Now use only Autofac. I make it, because i need support of Bridge.NET, where Autofac doesn`t work.
<br />
<br />To know more, you can read my workflow telegram channel https://t.me/coding_workflow
<br />
<br />Contacts:
<br />&nbsp;&nbsp;&nbsp;&nbsp;Vk - https://vk.com/yura_mysko
<br />&nbsp;&nbsp;&nbsp;&nbsp;Youtube - https://www.youtube.com/channel/UCiIj3Q0z1pNJ2KyNRcspoZw
<br />&nbsp;&nbsp;&nbsp;&nbsp;Habrahabr blog - https://habrahabr.ru/users/kogercoder/
<br />&nbsp;&nbsp;&nbsp;&nbsp;Telegram - https://t.me/yura_mysko