// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import { dotnet } from './dotnet.js'

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .create();

setModuleImports('main.mjs', {
    node: {
        process: {
            version: () => globalThis.process.version,
            p: () => globalThis.process
        }
    }
});

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

console.log(globalThis.process);

const text = exports.Formatter.Execute("SELECT 1 FROM [Table] T JOIN [Other] O ON O.Id = T.OtherId");
console.log(text);

await dotnet.run();
