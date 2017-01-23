import environment from './environment';
import { autoinject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { prettyPrintOne } from 'google-code-prettify';

// TODO: Find a real promise type definition
declare class Promise<T> {
}

class Notification {

    public message: string;
    public type: string;
}

@autoinject
export class App {

  private _formattedSql: Array<string>;

  public constructor(private client: HttpClient) {

      this.showFormattedSql = false;
      this.rawSql = environment.debug ? "SELECT * FROM Table T JOIN Other O ON O.Id = T.OtherId" : "";
  }

  public convert(): Promise<void> {

      if (!this.rawSql)
          return;

      return this
          .client
          .createRequest("/api/sql/format")
          .asPost()
          .withHeader('Content-Type', 'text/plain')
          .withContent(this.rawSql)
          .send()
          .then(r => {

              var result = JSON.parse(r.response);
              this.formattedSql = result.Sql;
              this.timeTaken = result.Duration;
              this.showFormattedSql = true;
          })
          .catch(r => {

              console.log(r.response);
              this.showNotification({ message: r.response, type: "error" });
              this.showFormattedSql = false;
          });
  }

  public inputSqlKeyPress(event: KeyboardEvent): boolean  {

      if (event.keyCode == 13 && event.ctrlKey) {

          this.convert();
          return false;
      }

      return true;
  }

  public inputFocused(event: Event): void {

      this.showFormattedSql = false;
  }

  public formattedSqlFocused(event: Event): void {

      this.showFormattedSql = true;
  }

  public copy(): void {

    if (!this.formattedCode)
        return;

    var range = document.createRange();
    range.selectNode(this.formattedCode)
    window.getSelection().addRange(range);
    var success = document.execCommand("copy");

    if (success)
        this.showNotification({ message: "Formatted SQL was successfully copied to clipboard", type: "success" });

    window.getSelection().empty();
  }

  private showNotification(notification: Notification) {

      this.message = notification.message;
      this.messageType = notification.type;
      setTimeout(() => this.message = "", 5000);
  }

  public get formattedSql(): Array<string> {

      return this._formattedSql;
  }

  public set formattedSql(value: Array<string>) {

      this._formattedSql = value;
      this.formattedCode.innerHTML = prettyPrintOne(value.join("\n"));
  }

  public get hasFormattedSql(): boolean {

      return this.formattedSql
          && this.formattedSql.length > 0;
  }

  public timeTaken: string;
  public rawSql: string;
  public formattedCode: Element;

  public message: string;
  public messageType: string;

  public showFormattedSql: boolean;
}
