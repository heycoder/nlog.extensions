## 介绍（Description）
- 扩展NLog，实现根据日志个数、时间阈值友好发送告警邮件的功能

## 配置（Config）

### 1、配置字典

字段名|说明
---|---
appname|应用程序名称，会体现在告警邮件主题中
host|SMTP服务器地址
port|	SMTP端口
displayname|	发件人名称
username|	SMTP账号
password|	SMTP密码
from|	发件人邮箱地址
to|	收件人邮件地址
maxerrorcount|	最大异常数：当第一次发生异常，或者异常数达到该阈值都会发送报警邮件
expiredtime|	过期时间：单位为毫秒，当第一次发生异常，或者最后一次异常触发的时间减去第一次异常发生的时间达到该阈值都会发送报警邮件
senderrorcount|	邮件中发送的异常个数，例如maxerrorcount=10，senderrorcount=5，发送邮件时只发送最后的5个异常信息

### 2、配置示例

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <extensions>
    <add assembly="Ppd.NLog" />
  </extensions>
  <targets>
    
    <target name="email"
        type="PpdEmail"
        appname="测试程序"
        host="127.0.0.1"
        port="25"
        displayname="测试告警"
        username="username"
        password="password"
        from="mail@corp.ppdai.com"
        to="you@ppdai.com"
        maxerrorcount="1000"
        senderrorcount="100"
        expiredtime="100000"
        ishtml="true"
        layout="${shortdate} ${time} | ${message}"
    />
  </targets>
  <rules>
    <logger name="*" minLevel="Error" appendTo="email"/>
  </rules>
</nlog>
```

## 其他（Other）

### 版本依赖

1. .NET Framewoork 4.0+
2. NLog 3.1.0+
  如果你的项目中引用了4.x版本的NLog，你需要的配置：
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="NLog" publicKeyToken="5120e14c03d0593c" />
        <bindingRedirect oldVersion="1.0.0.0-4.0.0.0" newVersion="4.0.0.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>
```