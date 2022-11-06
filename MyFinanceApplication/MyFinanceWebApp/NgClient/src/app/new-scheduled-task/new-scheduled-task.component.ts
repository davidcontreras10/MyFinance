import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { AutomaticTaskType, BasicOption, ScheduleTaskRequestType, ScheduleTaskView, UserSelectAccount } from '../automatic-tasks/automatic-tasks.model';
import { MyFinanceService } from '../services/my-finance.service';
import { ViewChild } from '@angular/core';
import { BasicNewScheduledTask, TransferNewScheduledTask } from '../services/models';
import { ThemePalette } from '@angular/material/core';
import { ProgressSpinnerMode } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-new-scheduled-task',
  templateUrl: './new-scheduled-task.component.html',
  styleUrls: ['./new-scheduled-task.component.css']
})
export class NewScheduledTaskComponent implements AfterViewInit {

  @Input() scheduleTaskView!: ScheduleTaskView;
  public userAccounts!: UserSelectAccount[];
  public destinationAccounts: BasicOption[] = [];
  public currencies: BasicOption[] = [];
  public spendingTypes: BasicOption[] = [];
  public daysOfWeek: BasicOption[] = [];
  public minMonthDay: number = 1;
  public maxMonthDay: number = 27;
  public monthDayPlaceholder = `Value between ${this.minMonthDay} and ${this.maxMonthDay}`;
  @ViewChild('f') form: any;

  color?: ThemePalette = 'primary';
  mode: ProgressSpinnerMode = 'indeterminate';
  value?: number;
  displayProgressSpinner = false;
  spinnerWithoutBackdrop = false;

  constructor(private myFinanceService: MyFinanceService) {
  }
  ngAfterViewInit(): void {
    console.log('form: ', this.form);
    this._initalLoad();
  }

  ngOnInit(): void {
    // this._initalLoad();
  }

  public goToView(): void {
    if (this.scheduleTaskView) {
      this._resetFieldsInitalState();
      this.scheduleTaskView.activeView = ScheduleTaskRequestType.View;
    }
  }

  public submit(f: any) {
    console.log('Submit: ', f);
    const trxType = this._readTrxType();
    if (f.valid && trxType > 0) {
      if (trxType === 1 || trxType === 2) {
        this._submitBasicTrx();
      }
      if (trxType === 3) {
        this._submitTransferTrx();
      }
    }
  }

  public onSelectedAccountChanged(event: any) {
    this._resetTypeAndAccountFields();
    this._loadTypeAndAccountFields();
    this.onCurrencyChanged();
  }

  public onCurrencyChanged() {
    this.destinationAccounts = [];
    const currencyId = this._readCurrencyId();
    const trxTypeId = this._readTrxType();
    const accountPeriodId = this._readAccountPeriodId();
    if (currencyId > 0 && accountPeriodId > 0 && trxTypeId === 3) {
      this.myFinanceService.getDestinationAccounts(accountPeriodId, currencyId)
        .subscribe((data) => {
          this.destinationAccounts = data;
        });
    }
    else {
      console.log(`perioId: ${accountPeriodId} -- currencyId: ${currencyId} -- trxType: ${trxTypeId}`);
    }
  }

  private _submitBasicTrx() {
    const model = this._readAddBasicModel();
    if (model) {
      this.myFinanceService.createBasic(model)
        .subscribe((response => {
          console.log('added basic successfully: ', response);
          this.form.reset();
        }));
    }
  }

  private _submitTransferTrx() {
    const model = this._readAddTransferModel();
    if (model) {
      this.myFinanceService.createTransfer(model)
        .subscribe((response => {
          console.log('added transfer successfully: ', response);
          this.form.reset();
        }));
    }
  }

  private _initalLoad() {
    this._resetTypeAndAccountFields();
    this._loadDaysOfWeeks();
    this._loadUserAccounts();
  }

  private _resetFieldsInitalState() {
    this._resetTypeAndAccountFields();
    this._loadTypeAndAccountFields();
  }

  private _loadTypeAndAccountFields() {
    const trxType = this._readTrxType();
    if (trxType > 0) {
      this._loadAddBasicTrxData();
    }
  }

  private _loadAddBasicTrxData() {
    const accountPeriodId = this._readAccountPeriodId();
    if (accountPeriodId > 0) {
      this.myFinanceService.getAddSpendData(accountPeriodId)
        .subscribe((addSimpleTrxModel) => {
          this.currencies = addSimpleTrxModel.supportedCurrencies;
          this.spendingTypes = addSimpleTrxModel.spendTypeViewModels;
          this.destinationAccounts = [];
        })
    }
  }

  private _readCurrencyId(): number {
    const formCurrencyId = this.form?.value?.currency;
    return this._readNumberValue(formCurrencyId);
  }

  private _readTrxType(): number {
    const formTrxType = this.form?.value?.trxType;
    return this._readNumberValue(formTrxType);
  }

  private _readAccountPeriodId(): number {
    const formAccPerId = this.form?.value?.accountSelected?.accountPeriodId;
    return this._readNumberValue(formAccPerId);
  }

  private _readNumberValue(numValue: any): number {
    return numValue && !isNaN(numValue) ? Number(numValue) : NaN;
  }

  private _readAmount(): number {
    const amount = this.form.value.amount;
    return this._readNumberValue(amount);
  }

  private _readFreqType(): number {
    const frqType = this.form.value?.frqType;
    return this._readNumberValue(frqType);
  }

  private _readAccountId(): number {
    const accountId = this.form.value.accountSelected?.accountId;
    return this._readNumberValue(accountId);
  }

  private _readToAccountId(): number {
    const accountId = this.form.value.destinationAccount;
    return this._readNumberValue(accountId);
  }

  private _readSpendTypeId() {
    const spendType = this.form.value.spendType;
    return this._readNumberValue(spendType);
  }

  private _readDay(): number {
    const frqType = this._readFreqType();
    if (frqType === 1) {
      return this._readNumberValue(this.form.value?.dayOfMonth);
    }
    if (frqType === 2) {
      return this._readNumberValue(this.form.value?.dayOfWeek);
    }

    return NaN;
  }

  private _readIsSpendTrx(): boolean | null {
    const trxType = this._readTrxType();
    if (trxType === 1) {
      return true;
    }

    if (trxType === 2) {
      return false;
    }

    return null;
  }

  private _readAddBasicModel(): BasicNewScheduledTask | null {
    if (this.form.invalid) {
      return null;
    }

    const days = [];
    days.push(this._readDay())
    return {
      amount: this._readAmount(),
      currencyId: this._readCurrencyId(),
      days: [this._readDay()],
      description: this.form.value.description,
      isSpendTrx: this._readIsSpendTrx(),
      frequencyType: this._readFreqType(),
      spendTypeId: this._readSpendTypeId(),
      accountId: this._readAccountId()
    };
  }

  private _readAddTransferModel(): TransferNewScheduledTask | null {
    if (this.form.invalid) {
      return null;
    }

    const days = [];
    days.push(this._readDay())
    return {
      amount: this._readAmount(),
      currencyId: this._readCurrencyId(),
      days: [this._readDay()],
      description: this.form.value.description,
      frequencyType: this._readFreqType(),
      spendTypeId: this._readSpendTypeId(),
      accountId: this._readAccountId(),
      toAccountId: this._readToAccountId()
    };
  }

  private _resetTypeAndAccountFields() {
    this.currencies = [];
    this.spendingTypes = [];
    this.destinationAccounts = [];
  }

  private _loadUserAccounts() {
    this.userAccounts = [];
    this.displayProgressSpinner = true;
    this.myFinanceService.getUserAccounts()
      .subscribe((accounts) => {
        this.userAccounts = accounts;
        this.displayProgressSpinner = false;
      });
  }

  private _loadDaysOfWeeks() {
    this.daysOfWeek = [];
    this.daysOfWeek.push({
      id: 0,
      name: 'Sunday'
    });
    this.daysOfWeek.push({
      id: 1,
      name: 'Monday'
    });
    this.daysOfWeek.push({
      id: 2,
      name: 'Tuesday'
    });
    this.daysOfWeek.push({
      id: 3,
      name: 'Wednesday'
    });
    this.daysOfWeek.push({
      id: 4,
      name: 'Thursday'
    });
    this.daysOfWeek.push({
      id: 5,
      name: 'Friday'
    });
    this.daysOfWeek.push({
      id: 6,
      name: 'Saturday'
    });
  }
}

