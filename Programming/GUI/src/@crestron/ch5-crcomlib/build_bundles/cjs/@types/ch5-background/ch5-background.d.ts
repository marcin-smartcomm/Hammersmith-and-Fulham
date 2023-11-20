import { Ch5Common } from './../ch5-common/ch5-common';
import { ICh5BackgroundAttributes } from './../_interfaces/ch5-background/i-ch5-background-attributes';
import { TCh5BackgroundScale, TCh5BackgroundRepeat } from './../_interfaces/ch5-background/types';
export interface IVideoResponse {
    action: string;
    id: number;
    top: number;
    left: number;
    width: number;
    height: number;
}
export declare class Ch5Background extends Ch5Common implements ICh5BackgroundAttributes {
    static SCALE: TCh5BackgroundScale[];
    static REPEAT: TCh5BackgroundRepeat[];
    static REFRESHRATE: number;
    static IMGBGCOLOR: string;
    primaryCssClass: string;
    parentCssClassPrefix: string;
    canvasCssClassPrefix: string;
    private _elCanvas;
    private _canvasList;
    private _imgUrls;
    private _elImages;
    private _bgColors;
    private _bgIdx;
    private _interval;
    private _videoRes;
    private _isVisible;
    private _videoDimensions;
    private _isRefilled;
    private lastRefillTime;
    private lastCutTime;
    private lastClearCutBGTimeout;
    private _url;
    private _backgroundColor;
    private _repeat;
    private _scale;
    private _refreshRate;
    private _videoCrop;
    private _imgBackgroundColor;
    private _transitionEffect;
    private _transitionDuration;
    private _receiveStateRefreshRate;
    private _subReceiveRefreshRate;
    private _receiveStateUrl;
    private _subReceiveUrl;
    private _receiveStateBackgroundColor;
    private _subReceiveBackgroundColor;
    private _videoSubscriptionId;
    private _canvasSubscriptionId;
    url: string;
    backgroundColor: string;
    repeat: TCh5BackgroundRepeat;
    scale: TCh5BackgroundScale;
    refreshRate: number | string;
    videoCrop: string;
    imgBackgroundColor: string;
    transitionEffect: string;
    transitionDuration: string;
    receiveStateRefreshRate: string;
    receiveStateUrl: string;
    receiveStateBackgroundColor: string;
    constructor();
    connectedCallback(): void;
    private isTimeToRefill(lastRefillTime);
    private videoSubsriptionCallBack(response);
    doSubscribeVideo(): void;
    disconnectedCallback(): void;
    static readonly observedAttributes: string[];
    attributeChangedCallback(attr: string, oldValue: string, newValue: string): void;
    unsubscribeFromSignals(): void;
    protected initAttributes(): void;
    protected updateBackground(): void;
    private getBackgroundUrl(values);
    private getBackgroundColor(values);
    private scaleToFill(img, canvas, ctx);
    private scaleToFit(img, canvas, ctx);
    private scaleToStretch(img, canvas, ctx);
    private updateBgImageScale(img, canvas, ctx);
    private updateBgImageRepeat(img, canvas, ctx);
    private canvasTemplate(count);
    private manageVideoInfo(response);
    private isScrollBar(elm, dir);
    private setCanvasDimensions(canvas);
    private updateCanvasDimensions();
    private createCanvas();
    private setBgTransition();
    private setBgImage();
    private setBgColor();
    private updateBgColor(color, ctx);
    private changeBackground(count);
    private updateBgImage(img, ctx);
    private refillBackground();
    private clearRectBackground();
}
