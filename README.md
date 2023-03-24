# nvm

A utility that allows for the management of multple versions of node, npm and npx.
This does not require admin rights.

## Commands:

* `nvm install lastest|node` installs the latest version of node into the system
* `nvm install 1.2.3` installs node version 1.2.3 into the system
* `nvm install latest --use` installs latest version of node and sets as the current version
* `nvm uninstall 1.2.3` removes the specified version of node from the system
    * If the current version is uninstalled, then no version will be set
* `nvm list` shows all installed versions of node
* `nvm use 1.2.3|latest|node` sets the specified version of node as the current version
* `nvm run 1.2.3 -- npm install` runs the command specified after -- under the specified version of node.

### Additional features
* `nvm install --tools` installs the scripts that wrap node tools (node, npm etc)
* nvm will track all installs of global tooling

Currently only really supported for windows because that's what I'm working on right now.

## Running this locally

1. Run `dotnet pack` from the project root to build the local nuget package
2. Install this (globally) with `dotnet tool install --global --add-source .\src\nvm\nupkg\ nvm`
3. Uinstall this with `dotnet tool uninstall --global nvm`
