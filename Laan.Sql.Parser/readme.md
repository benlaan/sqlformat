# Laan Sql Parser

A general purpose utility for parsing T-SQL. Currently incomplete. Provides reasonable support for the following statement types:

* SELECT
* INSERT
* UPDATE
* DELETE
* CREATE / ALTER (PROCEDURE|TABLE|VIEW)

## How to use

1. Parsing

```lang=csharp

    using Laan.Sql.Parser;
    using Laan.Sql.Parser.Entities;

    var sql = "SELECT * FROM Table; SELECT * FROM OtherTable";

    IList<IStatement> statements = ParserFactory.Execute(sql);
    var select = (SelectStatement)statements[0];
    select.From.TableName = "Test";
```

2 Constructing Sql

```lang=csharp

    using Laan.Sql.Parser;
    using Laan.Sql.Parser.Entities;

    var select = new SelectStatement();
    select.From.TableName = "Test";
    select.Fields.Add(new Field { Name = "Column1", Alias = "[Aliased_Column]", AliasType = AliasType.As });

    // to output usable SQL you need to feed this statement into [Laan.Sql.Formatter](https://nuget.org/packages/Laan.Sql.Formatter)
    var formatter = new Laan.Sql.Formatter.FormattingEngine();
    var formattedSql = formatter.Execute(select);
```
