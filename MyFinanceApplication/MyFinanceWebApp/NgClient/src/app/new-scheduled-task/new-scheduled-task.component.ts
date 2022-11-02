import { Component, Input, OnInit } from '@angular/core';
import { BasicOption, ScheduleTaskRequestType, ScheduleTaskView } from '../automatic-tasks/automatic-tasks.model';
import { GlobalVariables } from '../global-variables';

@Component({
  selector: 'app-new-scheduled-task',
  templateUrl: './new-scheduled-task.component.html',
  styleUrls: ['./new-scheduled-task.component.css']
})
export class NewScheduledTaskComponent implements OnInit {

  @Input() scheduleTaskView!: ScheduleTaskView;
  public accounts: BasicOption[] = [];
  public currencies: BasicOption[] = [];
  public spendingTypes: BasicOption[] = [];
  public daysOfWeek: BasicOption[] = [];
  public minMonthDay: number = 1;
  public maxMonthDay: number = 27;
  public monthDayPlaceholder = `Value between ${this.minMonthDay} and ${this.maxMonthDay}`;

  constructor(private globalVariables: GlobalVariables) {
    console.log('baseUrl: ', globalVariables.baseUrl);
  }

  ngOnInit(): void {
    this._resetFields();
  }

  public goToView(): void {
    if (this.scheduleTaskView) {
      this._resetFields();
      this.scheduleTaskView.activeView = ScheduleTaskRequestType.View;
    }
  }

  public submit(f: any){
    console.log('Submit: ', f.value);
  }

  private _resetFields() {
    this._loadTestAccounts();
    this._loadTestCurrencies();
    this._loadTestSpendingTypes();
    this._loadDaysOfWeeks();
  }

  private _loadDaysOfWeeks(){
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

  private _loadTestSpendingTypes(){
    this.spendingTypes = [];
    for(let i = 1; i <= 6; i++){
      this.spendingTypes.push({
        id: i,
        name: `Type ${i}`
      });
    }
  }

  private _loadTestCurrencies(){
    this.currencies = [];
    this.currencies.push({
      id: 1,
      name: "Colones"
    })
    this.currencies.push({
      id: 2,
      name: "Dolares"
    })
  }

  private _loadTestAccounts() {
    const loadedAccounts = [];
    for (let i = 1; i <= 10; i++) {
      loadedAccounts.push({
        id: i,
        name: `Account ${i}`
      })
    }

    this.accounts = loadedAccounts;
  }
}
