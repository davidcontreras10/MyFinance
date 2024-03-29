import { Injectable } from "@angular/core";

export enum AutomaticTaskType {
    Unknown = 0,
    SpIn = 1,
    Trasnfer = 2
}

export enum TaskStatus {
    Unknown = 0,
    Created = 1,
    Succeeded = 2,
    Failed = 3
}

export enum FrequencyType{
    Unknown = 0,
    Monthly = 1,
    Weekly = 2
}

export interface ExecutedTaskResult{
    status: TaskStatus,
    errorMsg: string
}

export interface IAutomaticTask {
    id: string,
    description: string,
    accountId: number,
    accountName: string,
    amount: number,
    currencySymbol: string,
    lastExecutedStatus: TaskStatus,
    frequencyType: FrequencyType,
    taskType: AutomaticTaskType,
    days: number[]
}

export interface UserSelectAccount {
    accountId: number,
    accountName: string,
    accountPeriodId: number
}

export enum ScheduleTaskRequestType {
    Unknown = 0,
    View = 1,
    New = 2
}

export interface BasicOption {
    id: number,
    name: string
}

@Injectable({
    providedIn: 'root',
  })
export class SpinnerController{
    showSpinnerModal: boolean = false;

    public enableSpinner(){
        this.showSpinnerModal = true;
    }

    public disableSpinner(){
        this.showSpinnerModal = false;
    }
}

@Injectable({
    providedIn: 'root',
  })
export class ScheduleTaskView {
    activeView: ScheduleTaskRequestType = ScheduleTaskRequestType.Unknown;
}

export interface ExecutedTask {
    executedDate: Date;
    status: TaskStatus;
    message: string;
}

export class SpInAutomaticTask implements IAutomaticTask {
    // getTaskDesc(): string {
        //     return `${this.currencySymbol}${this.amount} ${!this.isSpendTrx ? 'Income' : 'Spend'} every tenth of the month`;
    // }

    accountId!: number;
    lastExecutedStatus: TaskStatus = TaskStatus.Unknown;
    id: string = "";
    description: string = "";
    accountName: string = "";
    amount: number = 0;
    currencySymbol: string = "";
    isSpend!: boolean;
    frequencyType!: FrequencyType;
    taskType!: AutomaticTaskType;
    days!: number[];
}

export class TransferAutomaticTask implements IAutomaticTask {
    // getTaskDesc(): string {
        //     return `${this.currencySymbol}${this.amount} to ${this.toAccountName} every sixteenth of the month`;
    // }

    accountId!: number;
    lastExecutedStatus: TaskStatus = TaskStatus.Unknown;
    id: string = "";
    description: string = "";
    accountName: string = "";
    amount: number = 0;
    currencySymbol: string = "";
    toAccountName: string = "";
    frequencyType!: FrequencyType;
    taskType!: AutomaticTaskType;
    days!: number[];
}