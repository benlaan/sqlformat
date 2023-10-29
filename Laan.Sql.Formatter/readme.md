# Laan Sql Formatter

A tool form formatting T-SQL. Currently incomplete. Provides reasonable support for the following statement types:

* SELECT
* INSERT
* UPDATE
* DELETE
* CREATE / ALTER (PROCEDURE|TABLE|VIEW)

## How to use

1. Formatting

```lang=csharp

    using Laan.Sql.Formatter;

    var formatter = new FormattingEngine { TabSize = 4 };
    var formattedSql = formatter.Execute("SELECT A, B, C FROM Table T JOIN Other O ON O.TableId = T.Id");
```
