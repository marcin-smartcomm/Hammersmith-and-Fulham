export interface ICallback {
    arguments: Array<string | {}>;
    reference: string;
}
export default class HtmlCallback {
    protected _pattern: RegExp;
    protected _callbacks: ICallback[];
    protected _context: HTMLElement;
    constructor(context: HTMLElement, callbacks: string);
    run(target: Event | HTMLElement | undefined): void;
    callbacks: ICallback[];
    context: HTMLElement;
    protected prepareCallbacks(callbacks: string): void;
    protected getNestedMethod(_nestedObject: string, ref?: {
        [key: string]: any;
    }): (() => void) | undefined;
    protected isNativeMethod(methodReference: (() => void)): boolean;
}
