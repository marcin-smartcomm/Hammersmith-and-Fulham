import { Ch5Common } from "../ch5-common/ch5-common";
import { Ch5Signal } from "../ch5-core";
import { ICh5VideoAttributes } from "../_interfaces/ch5-video/i-ch5-video-attributes";
import { IPUBLISHEVENT, IBACKGROUND } from '../_interfaces/ch5-video/types/t-ch5-video-publish-event-request';
import { Observable } from "rxjs";
export declare type TSignalType = Ch5Signal<string> | Ch5Signal<number> | Ch5Signal<boolean> | null;
export declare type TSignalTypeT = string | number | boolean | any;
export declare class Ch5Video extends Ch5Common implements ICh5VideoAttributes {
    static PLAY_LABEL: string;
    static STOP_LABEL: string;
    static BUTTON_TYPE: string;
    static EVENT_LIST: Observable<Event>;
    static videoControls: string;
    static VIDEO_STYLE_CLASS: string;
    primaryVideoCssClass: string;
    preLoaderCssClass: string;
    loaderCssClass: string;
    fullScreenStyleClass: string;
    showControl: string;
    fullScreenBodyClass: string;
    private videoErrorMessages;
    private snapShotInfoMap;
    private appBgTimer;
    private wasAppBackGrounded;
    private appCurrentStatus;
    private scrollableElm;
    private isSlidemoved;
    private performanceMap;
    errorEvent: Event;
    ch5UId: number;
    private exitFullScreenIcon;
    private fullScreenIcon;
    private vid;
    private videoCanvasElement;
    private vidControlPanel;
    private controlFullScreen;
    private fullScreenOverlay;
    private snapShotTimer;
    private exitTimer;
    private subscriptionEventList;
    private context;
    private sizeObj;
    private position;
    private retryCount;
    private errorCount;
    private selectObject;
    private _snapShotRefreshRate;
    private _indexId;
    private _userId;
    private _snapShotUserId;
    private _password;
    private _snapShotPassword;
    private _aspectRatio;
    private _stretch;
    private _snapShotUrl;
    private _url;
    private _sourceType;
    private _size;
    private _zIndex;
    private _controls;
    private _sendEventSnapShotLastUpdateTime;
    private _receiveStateVideoCount;
    private _receiveStatePlay;
    private _receiveStateSelect;
    private _receiveStateSnapShotURL;
    private _receiveStateUrl;
    private _receiveStateSourceType;
    private _receiveStateSnapShotRefreshRate;
    private _receiveStateUserId;
    private _receiveStateSnapShotUserId;
    private _receiveStatePassword;
    private _receiveStateSnapShotPassword;
    private subReceiveStatePlay;
    private subReceiveStateUrl;
    private subReceiveStateSelect;
    private subReceiveStateSourceType;
    private subReceiveStateSnapShotUrl;
    private subReceiveStateSnapShotRefreshRate;
    private subReceiveStateUserId;
    private subReceiveStateSnapShotUserId;
    private subReceiveStatePassword;
    private subReceiveStateSnapShotPassword;
    private subReceiveStateVideoCount;
    private videoTop;
    private _sigNameSendOnClick;
    private videoLeft;
    private responseObj;
    private firstTime;
    private lastRequestStatus;
    private autoHideControlPeriod;
    private originalPotraitVideoProperties;
    private originalLandscapeVideoProperties;
    private originalVideoProperties;
    private oldReceiveStateSelect;
    private receiveStateAttributeCount;
    private requestID;
    private lastResponseStatus;
    private isSwipeInterval;
    private backgroundInterval;
    private isVideoPublished;
    private isOrientationChanged;
    private isFullScreen;
    private isVideoReady;
    private isInitialized;
    private playValue;
    private fromReceiveStatePlay;
    private _sigNameSelectionChange;
    private _sigNameSelectionSourceType;
    private _sigNameSnapShotUrl;
    private _sigNameSelectionUrl;
    private _sigNameEventState;
    private _sigNameErrorCode;
    private _sigNameErrorMessage;
    private _sigNameRetryCount;
    private _sigNameResolution;
    private _sigNameSnapShotStatus;
    private _sigNameSnapShotLastUpdateTime;
    private isPotraitMode;
    private lastLoadedImage;
    private isIntersectionObserve;
    private vidleft;
    private vidTop;
    private isAlphaBlend;
    private controlTimer;
    private controlTop;
    private controlLeft;
    private scrollTimer;
    private isExitFullscreen;
    private isPositionChanged;
    private oldResponseStatus;
    private oldResponseId;
    private subsCsigAppCurrentSate;
    private subsCsigAppBackgrounded;
    private receivedStateSelect;
    private vCountFlag;
    private maxVideoCount;
    private lastRequestUrl;
    private fromExitFullScreen;
    private videoTagId;
    private orientationCount;
    private landscapeOrientationTimeout;
    private previousXPos;
    private previousYPos;
    private observeInterval;
    private _protocol;
    constructor();
    disconnectedCallback(): void;
    videoStartObjJSON(actionStatus: string, uId: number, xPosition: number, yPosition: number, width: number, height: number, zIndex: number, alphaBlend: boolean, startTime: number, endTime: number): IPUBLISHEVENT;
    videoStopObjJSON(actionStatus: string, uId: number): IPUBLISHEVENT;
    videoBGObjJSON(actionStatus: string, id: string, xPosition: number, yPosition: number, width: number, height: number): IBACKGROUND;
    indexId: string;
    aspectRatio: string;
    stretch: string;
    userId: string;
    snapShotUserId: string;
    password: string;
    snapShotPassword: string;
    url: string;
    zIndex: string;
    controls: string;
    sourceType: string;
    snapShotUrl: string;
    size: string;
    snapShotRefreshRate: string;
    protocol: string;
    sendEventOnClick: string;
    sendEventState: string;
    sendEventSelectionChange: string;
    sendEventSelectionSourceType: string;
    sendEventSelectionURL: string;
    sendEventSnapShotURL: string;
    sendEventErrorCode: string;
    sendEventErrorMessage: string;
    sendEventRetryCount: string;
    sendEventResolution: string;
    sendEventSnapShotStatus: string;
    sendEventSnapShotLastUpdateTime: string;
    receiveStateVideoCount: string;
    receiveStatePlay: string;
    receiveStateSelect: string;
    receiveStateSourceType: string;
    receiveStateSnapShotRefreshRate: string;
    receiveStateSnapShotURL: string;
    receiveStateUrl: string;
    receiveStateUserId: string;
    receiveStateSnapShotUserId: string;
    receiveStatePassword: string;
    receiveStateSnapShotPassword: string;
    static readonly observedAttributes: string[];
    attributeChangedCallback(attr: string, oldValue: string, newValue: any): void;
    unsubscribeFromSignals(): void;
    connectedCallback(): void;
    removeIfTagExist(): void;
    processUri(): void;
    videoIntersectionObserver(): void;
    private updateAppBackgroundStatus();
    private appCurrentBackgroundStatus();
    private createCanvas();
    private getAllSnapShotData(videoCount);
    private clearAllSnapShots();
    private manageControls();
    private loadImageWithAutoRefresh();
    private setErrorMessages();
    private subscribeVideos(index);
    private matchAttributeResponse(attributeCount, responseCount);
    private switchSnapShotOnSelect(activeIndex);
    private sendEvent(signalName, signalValue, signalType);
    private unSubscribeVideos(selectObject);
    private setCanvasDimensions(width, height);
    private initializeVideo();
    private calculateSnapShotPositions();
    private drawSnapShot(videoImage);
    private snapShotOnLoad();
    private unsubscribeRefreshImage();
    private clearSnapShot();
    private videoScenariosCheck(playVal);
    private exitFullScreen();
    private handleTouchMoveEvent(ev);
    private enterFullScreen();
    private autoHideControls();
    private videoCP(event);
    private observePositionChangesAfterScrollEnds();
    private positionChange();
    private removeEvents();
    private orientationChanged();
    private orientationChange();
    private _getDisplayWxH(aRatio, width, height);
    private calculatePositions();
    private drawCanvas(video);
    private cutCanvas2DisplayVideo(context);
    private performanceDuration(key, duration, action);
    private publishVideoEvent(actionType);
    private videoStartRequest(actionType);
    private videoStopRequest(actionType);
    private errorResponse(error);
    private videoResponse(response);
    private clearOldResponseData();
    private hideFullScreenIcon();
    private showFullScreenIcon();
    private orientationChangeComplete();
    private calculatePillarBoxPadding(availableWidth, displayWidth);
    private calculateLetterBoxPadding(availableHeight, displayHeight);
    private setControlDimension();
    private fullScreenCalculation();
    private calculation(video);
    private rfc3339TimeStamp();
    private pad(n);
    private timezoneOffset(offset);
    protected attachEventListeners(): void;
    protected initAttributes(): void;
}
