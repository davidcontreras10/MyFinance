import { Component, Input, Output, OnInit, EventEmitter } from '@angular/core';
import { IAutomaticTask, ScheduleTaskRequestType, TaskStatus } from '../automatic-tasks/automatic-tasks.model';

@Component({
  selector: 'app-tasks-list',
  templateUrl: './tasks-list.component.html',
  styleUrls: ['./tasks-list.component.css']
})
export class TasksListComponent {
  TaskStatus = TaskStatus;

  @Input() tasks: IAutomaticTask[];
  @Output() selectedChanged = new EventEmitter();

  constructor() {
    this.tasks = []
  }

  public onSelection(selectedOption: IAutomaticTask) {
    this.selectedChanged.emit(selectedOption);
  }

  public getStatus(status: TaskStatus) {
    return TaskStatus[status];
  }
  
}
