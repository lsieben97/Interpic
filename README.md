<img src="https://github.com/lsieben97/Interpic/blob/master/src/Interpic.Icons/Icons/Logo.png" width="400" height="325"/>


# Interpic
Semi-automatic interactive reference documentation generator.  

Interpic (short for INteractive PICture) is a Semi-automatic interactive reference documentation generator primarily focussed towards web applications. (allthough with the right extensions it should be possible to generate documentation for any kind of project)  

The concept of interpic comes from html image maps: the ablilty for a user to click on a control on an image to find more information about that control.

## Project status
This project is in beta. there is a lot to be done before v1.0  

### Todo before V1.0
- [ ] Add context menu to manual tree.
- [ ] Add rename action for all manual items.
- [ ] Change selectors to be able to present a UI to select 1 or more manual items and expose through `IStudioEnvironment`.
- [ ] Add Migrations. Migrations are a way of advancing a manual to a new version of the software that the manual is for.
- [ ] Add migration previews. Migration previews are a way for a project type provider to 'peek' at the changes in the software and generate a changes report. The user can then accept, remove or save the report for later use.
- [ ] Add version history through migrations.
- [ ] Move behaviours into the studio. The studio will manage behaviours while extension specific implementations provide custom functionality.
- [ ] Add more behaviours to the base package.
- [ ] Allow extensions to be able to add menu items to menus form other extensions.
- [ ] Add screenshot viewer with highlights of sections / controls.
- [ ] provide an extra package with more detailed actions. (will also test the extension system)
- [ ] implement the home tab.
- [ ] add editors. An editor is a window that floats on top of a textbox and provides help with inputting text in the output format of the project.
- [ ] Add HTML output type together with web studio. Web studio is a quick way of creating html template files. complex editing tasks will be forwarded to external editors like VS-Code or Atom.
- [ ] Add CLI interface so a manual can be generated as part of the build process of an application.
- [ ] Add Bulk actions extension to privide optional bulk actions to the studio. (will also test the extension system)

### After V1.0
- [ ] Behaviour scripts, micro programming language to specify the actions of a behaviour.
- [ ] WPF support, via appium?
- [ ] investigate if Android / IOS support is possible.

### Old checklist
what follows is a non-exhaustive list of things to be done.
- [x] More complete settings system (helper settings, user controls) 
- [x] Multi-language documentation support
- [x] Multiple web drivers support (firefox, Chrome)
- [x] Web Behaviours support ( list of actions before executing the intended action of the studio. (usefull for systems where a login is required to view specific pages))
- [ ] Working extension system (loading unloading, importing)
- [x] Background tasks system (build on top of `Interpic.AsyncTasks.AsyncTask`)
- [x] Maximum amount of 1 selenium instance per session per driver.
- [x] Implement all menu bar items.
- [x] Custom menu items for extensions.
- [x] Logger access in developer menu.
- [x] Offline mode where actions that require network access are forbidden.
- [x] Workspace folder where all projects will be stored.


## Project structure
The codebase consists of the following assemblies:
### Interpic.Studio
The main studio assembly containing the studio application.
### Interpic.UI
Contains style files as well as all icons used by the studio.
### Interpic.Models
Contains the object model interpic uses.
### Interpic.Alerts
Contains various alert dialogs the studio uses.
### Interpic.AsyncTasks
Contains Interpic's asynchronous task framework.
### Interpic.Settings
Contains Interpic's settings framework.
### Interpic.Web
The build-in web project type. build like an interpic extension. It's loaded in by default.
### Interpic.Web.Selenium
Contains the selenium wrapper used to communicate with selenium.
### Interpic.Builders.MarkdownMKDocs
Contains the `IProjectBuilder` implementation to generate markdown files for use with MKDocs.
### Interpic.Utils
Contains various utilities.
