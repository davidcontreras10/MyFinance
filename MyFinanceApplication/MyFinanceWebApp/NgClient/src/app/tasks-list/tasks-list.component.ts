import { Component, Input, Output, OnInit, EventEmitter } from '@angular/core';
import { IAutomaticTask } from '../automatic-tasks/automatic-tasks.model';


@Component({
  selector: 'app-tasks-list',
  templateUrl: './tasks-list.component.html',
  styleUrls: ['./tasks-list.component.css']
})
export class TasksListComponent {
  @Input() tasks: IAutomaticTask[];
  @Output() selectedChanged = new EventEmitter();

  constructor(){
    this.tasks = []
  }

  public onSelection(selectedOption: IAutomaticTask){
    this.selectedChanged.emit(selectedOption);
  }
}
