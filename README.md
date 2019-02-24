<img src="https://github.com/lsieben97/Interpic/blob/master/src/Interpic.Icons/Icons/Logo.png" width="400" height="325"/>


# Interpic
Semi-automatic interactive reference documentation generator.  

Interpic (short for INteractive PICture) is a Semi-automatic interactive reference documentation generator primarily focussed towards web applications. (allthough with the right extensions it should be possible to generate documentation for any kind of project)  

The concept of interpic comes from html image maps: the ablilty for a user to click on a control on an image to find more information about that control.

## Project status
This project is still a proof of concept. there is a lot to be done before v1.0  

what follows is a non-exhaustive list of things to be done.
- [x] More complete settings system (helper settings, user controls) 
- [x] Multi-language documentation support
- [x] Multiple web drivers support (firefox, Chrome)
- [ ] Web Behaviours support ( list of actions before executing the intended action of the studio. (usefull for systems where a login is required to view specific pages))
- [ ] Working extension system (loading unloading, importing)
- [ ] Open source licenses system for extensions to specify. (will be shown on the about screen)
- [x] Background tasks system (build on top of `Interpic.AsyncTasks.AsyncTask`)
- [x] Maximum amount of 1 selenium instance per session per driver.
- [x] Implement all menu bar items.
- [x] Custom menu items for extensions.
- [ ] Better logging usage.
- [x] Logger access in developer menu.
- [ ] Offline mode where actions that require network access are forbidden.
- [ ] Screenshot viewer which shows the screen shot of the page and optionally overlays the sections / controls.
- [x] Workspace folder where all projects will be stored.


## Project structure
The codebase consists of the following assemblies:
### Interpic.Studio
The main studio assembly containing the studio application.
### Interpic.UI
Contains style files as well as all icons used by the studio.
### Interpic.Extensions
Contains the extension system. as well as the `IStudioEnvironment` class which the studio implements.
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
