"use strict";
// Copyright (C) 2018 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement
// under which you licensed this source code.
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.Ch5Cli = void 0;
var commander_1 = __importDefault(require("commander"));
var chalk_1 = __importDefault(require("chalk"));
var Ch5ArchiveCli_1 = require("@crestron/ch5-utilities-cli/build/cli/Ch5ArchiveCli");
var Ch5DeployCli_1 = require("@crestron/ch5-utilities-cli/build/cli/Ch5DeployCli");
var clear = require('clear');
var figlet = require('figlet');
var packageJson = require('@crestron/ch5-utilities-cli/package.json');
var buildVersion = packageJson.version || 'VERSION_NOT_READ';
var Ch5Cli = /** @class */ (function () {
    function Ch5Cli() {
        this._archiveCli = new Ch5ArchiveCli_1.Ch5ArchiveCli();
        this._deployCli = new Ch5DeployCli_1.Ch5DeployCli();
    }
    Ch5Cli.prototype.run = function () {
        var program = new commander_1.default.Command();
        program
            .version(buildVersion)
            .description("CH5 utilities CLI");
        this._archiveCli.setupArchiveCommand(program);
        this._deployCli.setupDeployCommand(program);
        // error on unknown commands
        program.on('command:*', function () {
            console.error('Invalid command: %s\nSee --help for a list of available commands.', program.args.join(' '));
            process.exit(1);
        });
        program.parse(process.argv);
        if (!process.argv.slice(2).length) {
            clear();
            console.log(chalk_1.default.blue(figlet.textSync("Crestron CH5 Utilities \u00A9 " + new Date().getFullYear(), {
                font: 'Standard',
                horizontalLayout: 'full',
                verticalLayout: 'full'
            })));
            program.outputHelp();
        }
    };
    return Ch5Cli;
}());
exports.Ch5Cli = Ch5Cli;
