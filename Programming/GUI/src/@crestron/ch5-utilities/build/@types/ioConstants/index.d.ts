export default class IoConstants {
    static touchScreenReloadCommand: string;
    static touchScreenUpdateCommand: string;
    static readonly touchScreenSftpDirectory: string;
    static controlSystemReloadCommand: string;
    static readonly controlSystemSftpDirectory: string;
    static tempExtension: string;
    static defaultExtension: string;
    static temporaryArchiveDir: string;
    static defaultUser: string;
    static defaultPassword: string;
    static readonly AppUiManifestContent: string;
    static addingExtraParamsToManifest: string;
    static connectedToDeviceAndUploading: string;
    static connectViaSsh: string;
    static connectionClosed: string;
    static connectionEnded: string;
    static sftUserConsoleLabel: string;
    static sftPasswordConsoleLabel: string;
    static NOENT: string;
    static ENOENT: string;
    static noIndexFile: string;
    static noManifestAndCreateWithDir: string;
    static noManifestAndCreate: string;
    static wrongAppType: string;
    static noConfiguration: string;
    static errorConfiguration: string;
    static errorProjectName: string;
    static errorDirectoryName: string;
    static errorOutputDirectoryName: string;
    static somethingWentWrong: string;
    static helpCommand: string;
    static deploymentComplete: string;
    static archiveCreatedWithSize(projectName: string, extension: string, size: number): string;
    static errorWithMessage(message: string): string;
    static directoryCreated(directoryName: string): string;
    static directoryDoesNotExist(directoryName: string): string;
    static noArchiveFile(archiveFilePath: string): string;
    static sendReloadCommandToDevice(device: string): string;
    static errorOnConnectingToHostWithError(host: string, errorMessage: string): string;
    static hashingError(errorMessage: string): string;
    static getMetadataFilePath(projectName: string, directory?: string): string;
    static getAppUiManifestFilePath(sourceDirectory: string): string;
    static getTempArchiveFilePath(projectName: string, extension?: string, directory?: string): string;
    static checkingDirectoryExists(directoryName: string): string;
    static createdDirectory(directoryName: string): string;
    static fileDoesNotExist(fileName: string): string;
    static invalidContractFile(fileName: string): string;
    static deviceOutput(data: string): string;
    static deviceError(data: string): string;
}
