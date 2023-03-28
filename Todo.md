## Features

- [x] Check for an NVM_HOME path and capture in install
- [ ] Error if a current version is not set
- [x] Check for a .nvmrc file
- [x] Add a logger so I can control how things are output
- [x] nvm install to add --use flag to use immediately
- [ ] track global installs through npm
- [ ] opt to replay all global installs
- [ ] logging needs to be diagnostic, information, off
- [x] allow installing lts
- [ ] allow creation of nvmrc
- [ ] reinstall tools (ps1 files) / reset path var
- [ ] allow for multi os support
- [ ] generate bash files for non-powershell environments

# Bugs

- [x] when "latest" is specified in nvm use, don't show "specified version is not installed" unless it isn't
- [ ] figure out how to get the install directory into the current process
- [x] Not using correct folder when installing
- [x] nvm run not working as expected

# Commands

- [x] install
- [x] install --use
- [x] list
- [x] use
- [x] run
- [x] run --version
- [x] uninstall 
