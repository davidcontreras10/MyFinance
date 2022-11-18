import { Component, Input, Output, OnInit, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { IAutomaticTask, ScheduleTaskRequestType, TaskStatus } from '../automatic-tasks/automatic-tasks.model';

@Component({
  selector: 'app-tasks-list',
  templateUrl: './tasks-list.component.html',
  styleUrls: ['./tasks-list.component.css']
})
export class TasksListComponent implements OnChanges {
  TaskStatus = TaskStatus;

  selectedTasks!: IAutomaticTask[];

  @Input() tasks: IAutomaticTask[];
  @Output() selectedChanged = new EventEmitter();

  constructor() {
    this.tasks = []
  }

  ngOnChanges(changes: SimpleChanges): void {
    this._verifySelected();
  }

  public onSelection(matModel: any) {
    const selectedOption = matModel.selectedOptions.selected[0].value as IAutomaticTask;
    this.selectedChanged.emit(selectedOption);
  }

  onSelectedTasksChanged(){
    if(this.selectedTasks && this.selectedTasks.length > 0){
      this.selectedChanged.emit(this.selectedTasks[0]);
    }
    else{
      this.selectedChanged.emit(null);
    }
  }

  public getStatus(status: TaskStatus) {
    return TaskStatus[status];
  }

  private _verifySelected(){
    const selectedTask = this.selectedTasks && this.selectedTasks.length > 0 ? this.selectedTasks[0] : null;
    if(selectedTask && !this.tasks.some(t=>t.id === selectedTask.id)){
      this.selectedTasks = [];
      this.selectedChanged.emit(null);
    }
  }
}
