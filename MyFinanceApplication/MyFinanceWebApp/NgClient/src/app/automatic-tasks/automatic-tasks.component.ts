import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { MyFinanceService } from '../services/my-finance.service';
import { AutomaticTaskType, IAutomaticTask, ScheduleTaskRequestType, ScheduleTaskView, SpInAutomaticTask, TaskStatus, TransferAutomaticTask } from './automatic-tasks.model';

@Component({
  selector: 'app-automatic-tasks',
  templateUrl: './automatic-tasks.component.html',
  styleUrls: ['./automatic-tasks.component.css']
})
export class AutomaticTasksComponent implements OnInit,OnChanges {
  @Input() scheduleTaskView!: ScheduleTaskView;
  public selectedTask!: IAutomaticTask;
  public loadedTasks!: IAutomaticTask[];

  constructor(private service: MyFinanceService){
  }

  public onSelectedTaskChanged(selectedOption: IAutomaticTask): void {
    this.selectedTask = selectedOption;
  }

  ngOnChanges(changes: SimpleChanges): void {
    console.log('on changes: ', changes);
  }

  ngOnInit(): void {
    // this.loadedTasks = this._loadSampleTasks();
    this.service.getScheduledTasks()
      .subscribe({
        next: this._setLoadedTasks.bind(this)
      })
  }

  public goToNew(): void {
    if (this.scheduleTaskView) {
      this.scheduleTaskView.activeView = ScheduleTaskRequestType.New;
    }
  }

  private _setLoadedTasks(data: IAutomaticTask[]){
    this.loadedTasks = data;
  }

  private _loadSampleTasks(): IAutomaticTask[] {
    const tasks: IAutomaticTask[] = [];
    for (let i = 1; i <= 10; i++) {
      let newTask = null;
      let isEven = i % 2 === 0;
      if (i < 5) {
        newTask = new SpInAutomaticTask();
        newTask.isSpendTrx = !isEven;
      } else {
        newTask = new TransferAutomaticTask();
        newTask.toAccountName = `DestinationAcc_${i}`;
      }

      newTask.id = i.toString();
      newTask.accountName = `Account ${i}`;
      newTask.amount = i * 10;
      newTask.currencySymbol = '$';
      newTask.description = `Para ahorro ${i}`;
      if (i < 8) {
        newTask.latestStatus = TaskStatus.Succeeded;
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
