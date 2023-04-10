# nvm-dotnet

*Another* node.js version manager for windows.

<details>
<summary>Why another version manager?</summary>

This manager doesn't rely on Environment variables or super user access.

[Corey Butlers](https://github.com/coreybutler/nvm-windows) nvm is still the recommended tool, but I can't use it on my work laptop.  I wanted to see how hard it would be to build a new tool while also adding a few additional features.

</details>

**Still a work in progress**

## Overview

Allows the management of multiple versions of node.js for developers who use dotnet tools.

This shouldn't be used for super old versions (io.js, Node 3 < )

Data is stored in your local AppData (Windows) folder, and no symlinks are used.  As such, you should not need administrator rights to use.

The following features add additional benefit while running multiple versions.

1. A version declared in an nvmrc file will be used regardless of the current version set via `nvm use`.
2. Running `nvm run --version "<command>"` will allow a command to be run in any version regardless of the current version set via `nvm use`.
3. `nvm use` does not need administrator rights.

## Building

Currently built using dotnet 6.
No additional tools should be required.

Use `dotnet pack` to build the nuget package


## Installation

Currently not hosted on any nuget repository.

1. Build the package using `dotnet pack`
2. Run `dotnet tool install --global --add-source ./src/nvm/nupkg nvm --version <version>`

### First time running

The first time a version is installed, a set of powershell scripts are installed into `%AppData%\nvm-dotnet`.

You will need to close and reopen your terminal for them to be reflected in the path.

## Usage

* `nvm install lastest|node` installs the latest version of node into the system
* `nvm install lts` installs the latest long term support (lts) version of node into the system
* `nvm install 1.2.3|v1.2.3` installs node version 1.2.3 into the system
* `nvm install <version> --use` installs latest version of node and sets as the current version

* `nvm uninstall 1.2.3|v1.2.3` removes the specified version of node from the system
    * If the current version is uninstalled, then no version will be set

* `nvm list` shows all installed versions of node

* `nvm use 1.2.3|v1.2.3|latest|node|lts` sets the specified version of node as the current version

* `nvm run "npm install"` runs the command under the currently used version of node (unless .nvmrc is in the directory)
* `nvm run ""npm install"" --version 1.2.3|v1.2.3` runs the command specified under the specified version of node.

Currently only really supported for windows because that's what I'm working on right now.
