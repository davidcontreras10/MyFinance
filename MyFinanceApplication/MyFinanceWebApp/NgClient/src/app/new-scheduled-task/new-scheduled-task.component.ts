import { Component, Input, OnInit } from '@angular/core';
import { BasicOption, ScheduleTaskRequestType, ScheduleTaskView, UserSelectAccount } from '../automatic-tasks/automatic-tasks.model';
import { MyFinanceService } from '../services/my-finance.service';
import { ViewChild } from '@angular/core';

@Component({
  selector: 'app-new-scheduled-task',
  templateUrl: './new-scheduled-task.component.html',
  styleUrls: ['./new-scheduled-task.component.css']
})
export class NewScheduledTaskComponent implements OnInit {

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

  constructor(private myFinanceService: MyFinanceService) {
  }

  ngOnInit(): void {
    this._initalLoad();
  }

  public goToView(): void {
    if (this.scheduleTaskView) {
      this._resetFields();
      this.scheduleTaskView.activeView = ScheduleTaskRequestType.View;
    }
  }

  public submit(f: any) {
    console.log('Submit: ', f.value);
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


  private _initalLoad() {
    this._resetTypeAndAccountFields();
    this._loadDaysOfWeeks();
    this._loadUserAccounts();
  }

  private _loadDestinationAccounts() {

  }

  private _resetFields() {
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
    return formCurrencyId && !isNaN(formCurrencyId) ? Number(formCurrencyId) : NaN;
  }

  private _readTrxType(): number {
    const formTrxType = this.form?.value?.trxType;
    return formTrxType && !isNaN(formTrxType) ? Number(formTrxType) : NaN;
  }

  private _readAccountPeriodId(): number {
    const formAccPerId = this.form?.value?.accountSelected?.accountPeriodId;
    return formAccPerId && !isNaN(formAccPerId) ? Number(formAccPerId) : NaN;
  }

  private _resetTypeAndAccountFields() {
    this.currencies = [];
    this.spendingTypes = [];
    this.destinationAccounts = [];
  }

  private _loadUserAccounts() {
    this.userAccounts = [];
    this.myFinanceService.getUserAccounts()
      .subscribe((accounts) => {
        this.userAccounts = accounts;
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

