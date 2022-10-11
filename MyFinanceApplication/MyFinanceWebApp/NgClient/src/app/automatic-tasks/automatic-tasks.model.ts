export enum AutomaticTaskType {
    Unknown = 0,
    SpIn = 1,
    Trasnfer = 2
}

export enum SpInTrxType{
    Unknown = 0,
    Spend = 1,
    Income = 2
}

export interface IAutomaticTask {
    id: string,
    name: string,
    accountName: string,
    amount: number,
    currencySymbol: string,
    getTaskType() : AutomaticTaskType;
}


export class SpInAutomaticTask implements IAutomaticTask {
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

export class TransferAutomaticTask implements IAutomaticTask{
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