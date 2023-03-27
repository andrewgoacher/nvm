# nvm-dotnet

A dotnet tool that aims to make managing node versions simple, without the need for super user access

This is still a work in progress, currently only  supports x64-win

## Overview

This project exists primarily because I need something like this for work.  My admin account and local account are different, and not only does nvm require elevated permissions, but when elevated, the install and usage run under a different user!  This makes nvm a no-go when I'm using my works machine.
This version of nvm runs via dotnet tool.  It manages multiple versions of node.js without ever needing to elevate to an admin account.  It does not use symlinks and it defaults to running in your local (app data) directory.


## Installation

`dotnet tool install --global --add-source https://nuget.pkg.github.com/andrewgoacher/ nvm --version 1.0.0` 

Running the above command will install nvm as a dotnet tool.
All config is installed to `%AppData\nvm-dotnet%`.  By default, all installations of nodejs will also reside here.
The config file is plain JSON and can be edited.  The installation location of node.js versions can be changed if needed.  It will also adopt your current nvm dir if you have an NVM_HOME environment variable.

## Usage

* `nvm install lastest|node` installs the latest version of node into the system
* `nvm install lts` installs the latest long term support (lts) version of node into the system
* `nvm install 1.2.3` installs node version 1.2.3 into the system
* `nvm install <version> --use` installs latest version of node and sets as the current version
* `nvm uninstall 1.2.3` removes the specified version of node from the system
    * If the current version is uninstalled, then no version will be set
* `nvm list` shows all installed versions of node
* `nvm use 1.2.3|latest|node|lts` sets the specified version of node as the current version
* `nvm eun "npm install"` runs the command under the currently used version of node (unless .nvmrc is in the directory)
* `nvm run 1.2.3 ""npm install""` runs the command specified under the specified version of node.

Currently only really supported for windows because that's what I'm working on right now.

## Running this locally

1. Run `dotnet pack` from the project root to build the local nuget package
2. Install this (globally) with `dotnet tool install --global --add-source .\src\nvm\nupkg\ nvm`
3. Uinstall this with `dotnet tool uninstall --global nvm`
