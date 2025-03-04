<p align="center">
  <img src="https://kenticoprod.azureedge.net/kenticoblob/crestron/media/crestron/generalsiteimages/crestron-logo.png">
</p>
 
# CH5 - Creston Components library (CrComLib) - Getting Started

#### Continuous Integration and Deployment Status

| DEV NIGHTLY - latest-dev | Status |
| ------ | ----------- |
| Build Pipeline |![Build status](https://dev.azure.com/crestron-mobile-devops/MobileApps/_apis/build/status/Blackbird/CoreBuild/CH5ComponentLibrary?branchName=dev)
| Release Pipeline | ![Deployment status](https://vsrm.dev.azure.com/crestron-mobile-devops/_apis/public/Release/badge/0403b700-ab40-43cd-9990-961924c561bc/38/108) |
| NPM | ![npm (tag)](https://img.shields.io/npm/v/@crestron/ch5-crcomlib/latest-dev) |

| MASTER-QE - latest-qe | Status |
| ------ | ----------- |
| Build Pipeline |![Build status](https://dev.azure.com/crestron-mobile-devops/MobileApps/_apis/build/status/Blackbird/CoreBuild/CH5ComponentLibrary?branchName=master)
| Release Pipeline | ![Deployment status](https://vsrm.dev.azure.com/crestron-mobile-devops/_apis/public/Release/badge/0403b700-ab40-43cd-9990-961924c561bc/38/94) |
| NPM | ![npm (tag)](https://img.shields.io/npm/v/@crestron/ch5-crcomlib/latest-qe) |

## See Crestron developer website for documentation 
https://www.crestron.com/developer
Search for CRESTRON HTML5 USER INTERFACE 

## Generated folders

- build_bundles/_module_type_     - contains files generated by webpack (and the typescript compiler for the esm modules)
- compiled_bundles/_module_type_  - contains files generated by the typescript compiler (tsc)
- docs/html


Where _module_type_ is:

- *umd* - UMD (contains CommonJs, AMD, and also creates a global property in the window object).
- *cjs* - CommonJs
- *esm* - ES Modules, ES6.
- *amd* - Asynchronous Module Definition

## Activating extra informmation in the browser console

### For ch5 components 

Ch5 components will display additional info in the browser console if they have a debug attribute defined. For example:
```<ch5-button debug label="Btn1"></ch5-button>```

### For ch5 custom attributes (dta-ch5-...)

Additional information will be displayed in the browser console if the element has a debug attribute. For example:
```<div debug data-ch5-show="a_signal">Hello</div>```

### For the bridge-related functions/methods

In order to display additional information, you must first enable this using the methods from Ch5Debug:
* getConfig - Returns the current configuration: all keys that can be set and their current value. A key enables debug info 
for a method/function
* loadConfig(cfg) - Loads a new config (replaces the previous one)
* enableAll() - Enables all keys. Will display all debug info available. (The debug info that uses Ch5Debug, the info 
from ch5 components, and custom attributes will not be affected)
* disablesAll() - Disables all keys
* setConfigKeyValue(key:string, value:boolean) - Changes the value of a key

