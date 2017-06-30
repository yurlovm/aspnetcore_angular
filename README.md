# Quick start project for ASP.NET Core with Angular including Webpack, WebSockets, JWT authentication.
Prerequisite: install node.js, install TypeScript globally:

    npm install -g typescript

Prerequisite: Visual Studio 2015 Update 3 or Visual Studio 2017

Prerequisite: Configure Visual Studio to use the global external web tools instead of the tools that ship with Visual Studio:
  - Open the Options dialog with Tools | Options
  - In the tree on the left, select Projects and Solutions | External Web Tools.
  - On the right, move the $(PATH) entry above the $(DevEnvDir) entries. This tells Visual Studio to use the external tools (such as npm) found in the global path before using its own version of the external tools.
  - Click OK to close the dialog.
  - Restart Visual Studio for this change to take effect.

To disable compilation of ts files in IDE upon saving set for a given tsconfig.json "compileOnSave": false


If you want to disable "npm install" every time you open the project then turn off all entries in External Web Tools.


If you would like to disable building TypeScript files by IDE Build in your solution add node
```sh
<TypeScriptCompileBlocked>true</TypeScriptCompileBlocked>
```
to the first PropertyGroup element in .csproj file.

# Building Angular:
First install all npm packages:

    npm install

Build your Angular app by npm scripts commands:


| npm script | comment |
| ------ | ------ |
| npm run build  | build Angular app without AOT |
| npm run build:prod | build Angular app for production with AOT and minimization |
| npm run wp  | build Angular app in watch mode without AOT |
| npm run clean | clean output webpack folder |
| npm run reset  | clean everything including node_modules |
| npm run tslint  | inting Angular app |
| npm run test  | testing Angular app by karma and jasmine |
| npm run e2e  | e2e testing Angular app by protractor |
| npm run srcmap  | investigate resulting webpack chunck, change for correct filename |


 Project support angular-cli generation, testing and linting.

 Go to ./src/app folder and run for example

    ng generate component test


 It require to install angular-cli globally:

    npm install -g @angular/cli


# Authentication

This application use JWT authentication.  Call /oauth/token endpoint for authentication.
Websocket also has support for JWT validation.

`Remove from keys.json with your secret key from control version!!`

# Websocket

Websocket has basic realization of invoking server methods from client and client methods from server.

This application use basic realization of Identity framework without EntityFrameowork or any other real DB. You can change working logic with DB in CustomUserManager and CustomUserStore classes.

Use BCrypt for password hashing in BCryptPasswordHasher class.
