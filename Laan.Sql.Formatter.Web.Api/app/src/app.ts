import { autoinject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { prettyPrintOne } from 'google-code-prettify';

@autoinject
export class App {

  private _formattedSql: Array<string>;

  public constructor(private client: HttpClient) {

      this.rawSql = "SELECT * FROM Table T JOIN Other O ON O.Id = T.OtherId";
  }

  public convert(): Promise<void> {

    return this
      .client
      .get(`/api/sql/format?query=${this.rawSql}`)
      .then(r => {

        var result = JSON.parse(r.response);
        this.formattedSql = result.Sql;
        this.timeTaken = result.Duration;
      });
  }

  public copy(): void {


  }

  public get formattedSql(): Array<string> {

      return this._formattedSql;
  }

  public set formattedSql(value: Array<string>) {

      this._formattedSql = value;
      this.formattedCode.innerHTML = prettyPrintOne(value.join("\n"));
  }

  public timeTaken: string;
  public rawSql: string;
  public formattedCode: Element;
}
