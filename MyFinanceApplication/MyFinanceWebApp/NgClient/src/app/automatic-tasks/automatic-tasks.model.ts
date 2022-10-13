export enum AutomaticTaskType {
    Unknown = 0,
    SpIn = 1,
    Trasnfer = 2
}

export enum SpInTrxType {
    Unknown = 0,
    Spend = 1,
    Income = 2
}

export enum TaskStatus {
    Unknown = 0,
    Created = 1,
    Succeded = 2,
    Failed = 3
}

export interface IAutomaticTask {
    id: string,
    name: string,
    accountName: string,
    amount: number,
    currencySymbol: string,
    latestStatus: TaskStatus,
    getTaskType(): AutomaticTaskType;
    getTaskDesc(): string;
}


export interface ExecutedTask {
    executedDate: Date;
    status: TaskStatus;
}

export class SpInAutomaticTask implements IAutomaticTask {
    getTaskDesc(): string {
        return `${this.currencySymbol}${this.amount} ${this.trxType === SpInTrxType.Income ? 'Income' : 'Spend'} every tenth of the month`;
    }
    latestStatus: TaskStatus = TaskStatus.Unknown;
    id: string = "";
    name: string = "";
    accountName: string = "";
    amount: number = 0;
    currencySymbol: string = "";
    trxType: SpInTrxType = SpInTrxType.Unknown;
    getTaskType(): AutomaticTaskType {
        return AutomaticTaskType.SpIn;
    }
}

export class TransferAutomaticTask implements IAutomaticTask {
    getTaskDesc(): string {
        return `${this.currencySymbol}${this.amount} to ${this.toAccountName} every sixteenth of the month`;
    }
    latestStatus: TaskStatus = TaskStatus.Unknown;
    id: string = "";
    name: string = "";
    accountName: string = "";
    amount: number = 0;
    currencySymbol: string = "";
    toAccountName: string = "";
    getTaskType(): AutomaticTaskType {
        return AutomaticTaskType.Trasnfer;
    }
}