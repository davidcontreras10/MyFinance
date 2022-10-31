import { Component, Input, OnInit } from '@angular/core';
import { BasicOption, ScheduleTaskRequestType, ScheduleTaskView } from '../automatic-tasks/automatic-tasks.model';

@Component({
  selector: 'app-new-scheduled-task',
  templateUrl: './new-scheduled-task.component.html',
  styleUrls: ['./new-scheduled-task.component.css']
})
export class NewScheduledTaskComponent implements OnInit {

  @Input() scheduleTaskView!: ScheduleTaskView;
  public accounts: BasicOption[] = [];

  constructor() {
  }

  ngOnInit(): void {
    console.log('_resetFields NewScheduledTaskComponent');
    this._resetFields();
  }

  public goToView(): void {
    if (this.scheduleTaskView) {
      this._resetFields();
      this.scheduleTaskView.activeView = ScheduleTaskRequestType.View;
    }
  }

  private _resetFields() {
    this._loadTestAccounts();
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
    console.log('Accounts: ', this.accounts);
  }

  update(e: any) {
    console.log('Selected: ', e.target.value);
  }

}
