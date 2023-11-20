import { Ch5Common } from "../ch5-common/ch5-common";
import { Ch5CommonInput } from "../ch5-common-input/ch5-common-input";
import { Ch5Button } from "../ch5-button/ch5-button";
import { ICh5FormAttributes } from "../_interfaces/ch5-form";
import { TCh5ButtonType } from "../_interfaces/ch5-button/types";
export declare class Ch5Form extends Ch5Common implements ICh5FormAttributes {
    static SUBMIT_LABEL: string;
    static CANCEL_LABEL: string;
    static SUBMIT_TYPE: string;
    static CANCEL_TYPE: string;
    primaryCssClass: string;
    cssClassPrefix: string;
    private _hideSubmitButton;
    private _submitButtonLabel;
    private _submitButtonIcon;
    private _submitButtonStyle;
    private _submitButtonType;
    private _hideCancelButton;
    private _cancelButtonLabel;
    private _cancelButtonIcon;
    private _cancelButtonStyle;
    private _cancelButtonType;
    private _submitId;
    private _cancelId;
    private _inputElements;
    private _submitButton;
    private _cancelButton;
    private _submitShouldBeDisable;
    private _cancelShouldBeDisabled;
    private _customCancelButtonRef;
    private _customSubmitButtonRef;
    ready: Promise<void>;
    readonly inputElements: Ch5CommonInput[];
    submitButton: Ch5Button;
    cancelButton: Ch5Button;
    hideSubmitButton: boolean | string;
    hidesubmitbutton: boolean | string;
    submitButtonLabel: string;
    submitButtonIcon: string;
    submitButtonStyle: string;
    submitButtonType: TCh5ButtonType;
    hideCancelButton: boolean | string;
    cancelButtonLabel: string;
    cancelButtonIcon: string;
    cancelButtonStyle: string;
    cancelButtonType: TCh5ButtonType;
    submitId: string;
    cancelId: string;
    private setCustomCancelBtn();
    private setCustomSubmitBtn();
    constructor();
    connectedCallback(): void;
    disconnectedCallback(): void;
    static readonly observedAttributes: string[];
    attributeChangedCallback(attr: string, oldValue: string, newValue: string): void;
    protected initAttributes(): void;
    protected attachEventListeners(): void;
    protected removeEvents(): void;
    submit(): void;
    cancel(): void;
    private _linkInputElements();
    private _getInputElements();
    private _initFormButtons();
    private _linkFormButtons();
    private _checkIfCancelOrSubmitShouldBeDisabled();
    private checkIfCustomSubmitShouldBeDisabled(disable);
    private checkIfCustomCancelShouldBeDisabled(disable);
    private _createButton(label, type, formType, disable?);
    private _onClickSubmitButton();
    private _onClickCancelButton();
    private _upgradeProperty(prop);
}
