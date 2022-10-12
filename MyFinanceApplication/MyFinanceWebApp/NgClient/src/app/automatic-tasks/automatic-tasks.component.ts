import { Component, OnInit } from '@angular/core';
import { AutomaticTaskType, IAutomaticTask, SpInAutomaticTask, SpInTrxType, TaskStatus, TransferAutomaticTask } from './automatic-tasks.model';

@Component({
  selector: 'app-automatic-tasks',
  templateUrl: './automatic-tasks.component.html',
  styleUrls: ['./automatic-tasks.component.css']
})
export class AutomaticTasksComponent implements OnInit {
  public selectedTask!: IAutomaticTask;
  public loadedTasks!: IAutomaticTask[];

  public onSelectedTaskChanged(selectedOption: IAutomaticTask): void {
    this.selectedTask = selectedOption;
  }

  constructor() {
  }

  ngOnInit(): void {
    this.loadedTasks = this._loadSampleTasks();
  }

  private _loadSampleTasks(): IAutomaticTask[] {
    const tasks: IAutomaticTask[] = [];
    for (let i = 1; i <= 10; i++) {
      let newTask = null;
      let isEven = i % 2 === 0;
      if (i < 5) {
        newTask = new SpInAutomaticTask();
        newTask.trxType = isEven ? SpInTrxType.Spend : SpInTrxType.Income;
      } else {
        newTask = new TransferAutomaticTask();
        newTask.toAccountName = `DestinationAcc_${i}`;
      }

      newTask.id = i.toString();
      newTask.accountName = `Account ${i}`;
      newTask.amount = i * 10;
      newTask.currencySymbol = '$';
      newTask.name = `Para ahorro ${i}`;
      if (i < 8) {
        newTask.latestStatus = TaskStatus.Succeded;
      } else if (i < 9) {
        newTask.latestStatus = TaskStatus.Created;
      } else {
        newTask.latestStatus = TaskStatus.Failed;
      }

      tasks.push(newTask);
    }

    return tasks;
  }
}
