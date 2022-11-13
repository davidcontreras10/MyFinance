import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { ThemePalette } from '@angular/material/core';
import { ProgressSpinnerMode } from '@angular/material/progress-spinner';
import { MyFinanceService } from '../services/my-finance.service';
import { AutomaticTaskType, IAutomaticTask, ScheduleTaskRequestType, ScheduleTaskView, SpInAutomaticTask, TaskStatus, TransferAutomaticTask } from './automatic-tasks.model';

@Component({
  selector: 'app-automatic-tasks',
  templateUrl: './automatic-tasks.component.html',
  styleUrls: ['./automatic-tasks.component.css']
})
export class AutomaticTasksComponent implements OnInit, OnChanges {
  public selectedTask!: IAutomaticTask;
  public loadedTasks!: IAutomaticTask[];

  spinnerColor?: ThemePalette = 'primary';
  spinnerMode: ProgressSpinnerMode = 'indeterminate';
  spinnerValue?: number;
  displayProgressSpinner = false;
  spinnerWithoutBackdrop = false;


  constructor(private service: MyFinanceService, public scheduleTaskView: ScheduleTaskView) {
  }

  public onSelectedTaskChanged(selectedOption: IAutomaticTask): void {
    this.selectedTask = selectedOption;
  }

  ngOnChanges(changes: SimpleChanges): void {
  }

  ngOnInit(): void {
    this._reloadScheduledTasks();
  }

  public goToNew(): void {
    if (this.scheduleTaskView) {
      this.scheduleTaskView.activeView = ScheduleTaskRequestType.New;
    }
  }

  public onTaskModelChanged() {
    this._reloadScheduledTasks();
  }

  private _setLoadedTasks(data: IAutomaticTask[]) {
    this.loadedTasks = [];
    this.loadedTasks = data;
  }

  private _reloadScheduledTasks() {
    this.service.getScheduledTasks()
      .subscribe(data => {
        this._setLoadedTasks(data);
      })
  }
}
