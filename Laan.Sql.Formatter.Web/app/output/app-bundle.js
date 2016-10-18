var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
define('app',["require", "exports", 'aurelia-framework', 'aurelia-http-client', 'google-code-prettify'], function (require, exports, aurelia_framework_1, aurelia_http_client_1, google_code_prettify_1) {
    "use strict";
    var App = (function () {
        function App(client) {
            this.client = client;
            this.rawSql = "";
        }
        App.prototype.convert = function () {
            var _this = this;
            return this
                .client
                .createRequest("/api/sql/format")
                .asPost()
                .withHeader('Content-Type', 'text/plain')
                .withContent(this.rawSql)
                .send()
                .then(function (r) {
                var result = JSON.parse(r.response);
                _this.formattedSql = result.Sql;
                _this.timeTaken = result.Duration;
            })
                .catch(function (r) {
                console.log(r.response);
            });
        };
        App.prototype.copy = function () {
            if (!this.formattedCode)
                return;
            var range = document.createRange();
            range.selectNode(this.formattedCode);
            window.getSelection().addRange(range);
            document.execCommand("copy");
            window.getSelection().empty();
        };
        Object.defineProperty(App.prototype, "formattedSql", {
            get: function () {
                return this._formattedSql;
            },
            set: function (value) {
                this._formattedSql = value;
                this.formattedCode.innerHTML = google_code_prettify_1.prettyPrintOne(value.join("\n"));
            },
            enumerable: true,
            configurable: true
        });
        App = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [aurelia_http_client_1.HttpClient])
        ], App);
        return App;
    }());
    exports.App = App;
});

define('environment',["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = {
        debug: true,
        testing: true
    };
});

define('main',["require", "exports", './environment'], function (require, exports, environment_1) {
    "use strict";
    Promise.config({
        warnings: {
            wForgottenReturn: false
        }
    });
    function configure(aurelia) {
        aurelia.use
            .standardConfiguration();
        if (environment_1.default.debug) {
            aurelia.use.developmentLogging();
        }
        aurelia.start().then(function () { return aurelia.setRoot(); });
    }
    exports.configure = configure;
});

define('text!app.html', ['module'], function(module) { module.exports = "<template>\r\n    <require from=\"google-code-prettify/prettify.min.css\"></require>\r\n    <div id=\"container\">\r\n        <div id=\"header\">Laan.Sql.Formatter (alpha)</div>\r\n        <div id=\"top\">\r\n            <ul class=\"menulist\">\r\n                <li><a href=\"/app\">Home</a></li>\r\n                <li><a href=\"http://www.github.com\">Source Hosting by GitHub</a></li>\r\n                <li><a href=\"http://github.com/benlaan/sqlformat\">Want the code? have a look</a></li>\r\n                <li><a href=\"http://github.com/benlaan/sqlformat/issues\">Found a bug? log it</a></li>\r\n                <li><a href=\"http://benlaan.com\">by Ben Laan</a></li>\r\n            </ul>\r\n        </div>\r\n        <div id=\"centre\">\r\n            <h4>Input Sql</h4>\r\n            <textarea class=\"code prettyprint lang-sql\" style=\"width: 100%; height:180px\" value.bind=\"rawSql\" ></textarea>\r\n\r\n            <div style=\"padding-bottom: 15px\">\r\n                Time Taken: ${timeTaken ? timeTaken : '0.0'}\r\n            </div>\r\n\r\n            <div style=\"text-align:right\">\r\n                <button type=\"button\" click.delegate=\"convert()\">Convert</button>\r\n            </div>\r\n\r\n            <h4>Output Sql</h4>\r\n            <div style=\"text-align:right\">\r\n                <button type=\"button\" click.delegate=\"copy()\">Copy</button>\r\n            </div>\r\n            <div style=\"height:270px;overflow-y: scroll; width: 100%\">\r\n                <pre class=\"code prettyprint\" ref=\"formattedCode\">\r\n                <template repeat.for=\"line of formattedSql\">${'\\n' + line}</template>\r\n\t\t\t</pre>\r\n            </div>\r\n        </div>\r\n        <div id=\"footer\">Web design by <a href=\"http://nicolas.freezee.org\">Nicolas Fafchamps</a></div>\r\n    </div>\r\n</template>"; });
//# sourceMappingURL=app-bundle.js.map