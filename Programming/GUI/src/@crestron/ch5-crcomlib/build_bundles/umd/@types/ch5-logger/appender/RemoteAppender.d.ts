import { AbstractAppender } from "./AbstractAppender";
import { LogMessage } from "../helpers/index";
import { Logger } from "../logger";
import { TAppenderConfig } from "../types";
export declare class RemoteAppender extends AbstractAppender {
    private static _instance;
    private webSocket;
    private _requestService;
    private _address;
    static getInstance(sendLogTimeOffsetInMiliseconds: number, appenderConfig: TAppenderConfig): RemoteAppender;
    closeSocketConnection(): void;
    private constructor();
    log(data: LogMessage): void;
    resetIP(hostname: string, port: string, secure?: boolean): void;
    private setIP(appenderConfig);
    configObserver(helper: Logger, hasFilterConfig: boolean): void;
    protected initialiseRequest(uri: string): void;
}
