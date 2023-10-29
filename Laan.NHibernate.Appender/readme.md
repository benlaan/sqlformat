# Formats NHibernate.SQL logger output

to configure, add the following appender and logger to your `log4net.config` file.

```lang=xml
<appender name="SqlAppender" type="Laan.NHibernate.Appender.NHibernateAppender, Laan.NHibernate.Appender">
    <file type="log4net.Util.PatternString" value="${LOGS}\Sql.log" />
    <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="-- %d{yyyy-MM-dd HH:mm:ss.fff}%n%m%n" />
    </layout>
</appender>

...

<logger name="NHibernate.SQL>
    <level value="INFO" />
    <appender-ref ref="SqlAppender" />
</logger>
```